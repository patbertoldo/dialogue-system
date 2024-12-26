using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Dialogue
{
    /// <summary>
    /// Dialogue State is for each piece of dialogue in a dialogue block, so we know when that dialogue
    /// is animating a task, or has finished animating and is ready to move on.
    /// </summary>
    public enum DialogueState
    {
        NONE,           // When text and tasks are started.
        PLAY,           // When text is in an animating state.
        SKIPPED,        // When in player input force finishes the text animation.
        FINISHED        // When text has finished animating.
    }
    
    public class DialogueManager
    {
        // References
        private DialoguePanel dialoguePanel;
        
        // States
        private DialogueScriptableObject currentDialogue;
        private DialogueState currentState;
        private int currentIndex;
        
        // Tasks
        private AsyncOperationHandle<DialogueScriptableObject> currentDialogueHandle;
        
        private CancellationTokenSource animationCancellation;
        private StringBuilder animatingBuilder;
        
        // Custom Markup
        private const string markupWait = "wait";
        private const string markupSpeed = "speed";
        private const string markupShake = "shake";
        private const string markupShow = "show";
        private const string markupHide = "hide";
        
        public DialogueManager(DialoguePanel dialoguePanel)
        {
            this.dialoguePanel = dialoguePanel;

            currentDialogue = null;
            currentState = DialogueState.NONE;
            currentIndex = 0;
            
            animationCancellation = new CancellationTokenSource();
            animatingBuilder = new StringBuilder();
            
            dialoguePanel.SetContinueEvent(ContinueDialogue);
        }

        #region Dialogue Handling
        
        public async void OpenDialogue(DialogueScriptableObjectAssetReference dialogueAddressable)
        {
            animationCancellation = new CancellationTokenSource();

            currentDialogueHandle = Addressables.LoadAssetAsync<DialogueScriptableObject>(dialogueAddressable);
            currentDialogue = await currentDialogueHandle;
            
            // Get the first dialogue for the left side of the conversation and the right.
            // For dialogues that only have one side, pass null for the other side.
            DialogueBlock dialogueBlockLeft = currentDialogue.GetFirstInstanceOfAlignment(DialogueAlignment.LEFT);
            DialogueBlock dialogueBlockRight = currentDialogue.GetFirstInstanceOfAlignment(DialogueAlignment.RIGHT);
            
            dialoguePanel.InitialiseDialogue(dialogueBlockLeft, dialogueBlockRight);

            var task = UniTask.Delay(100, cancellationToken: animationCancellation.Token).SuppressCancellationThrow().GetAwaiter();
            
            task.OnCompleted(PlayDialogue);
        }

        /// <summary>
        /// UI event.
        /// </summary>
        private void ContinueDialogue()
        {
            switch (currentState)
            {
                case DialogueState.PLAY:
                {
                    animationCancellation.Cancel();
                    break;
                }
                case DialogueState.SKIPPED:              
                case DialogueState.FINISHED:
                {
                    FinishDialogue();
                    break;
                }
            }
        }

        private async void PlayDialogue()
        {
            currentState = DialogueState.PLAY;
            
            // I want to allow tasks to be cancelled, but once they are I don't think I can recycle
            // the source, so each new play creates a new source.
            animationCancellation = new CancellationTokenSource();
                    
            var dialogueBlock = currentDialogue.DialogueBlocks[currentIndex];
            dialoguePanel.NextDialogue(dialogueBlock);

            try
            {
                await AnimateText(dialogueBlock.Description, dialogueBlock.TextSpeed, animationCancellation.Token);
                
                currentState = DialogueState.FINISHED;
            }
            catch (Exception e)
            {
                SkipDialogue();
            }
        }

        private void SkipDialogue()
        {
            currentState = DialogueState.SKIPPED;
            
                    
            var dialogueBlock = currentDialogue.DialogueBlocks[currentIndex];

            dialoguePanel.SetDialogueTextOnActiveDialogue(dialogueBlock.Description);
        }

        private void FinishDialogue()
        {
            currentState = DialogueState.FINISHED;
            
            currentIndex++;

            if (currentIndex >= currentDialogue.DialogueBlocks.Length)
            {
                CloseDialogue();
            }
            else
            {
                PlayDialogue();
            }
        }

        private void CloseDialogue()
        {
            dialoguePanel.Hide();

            ResetDialogue();
        }
        
        #endregion Dialogue Handling
        
        #region Task Handling
        
        private async UniTask AnimateText(string text, int textSpeed, CancellationToken cancellationToken)
        {
            animatingBuilder.Clear();
            StringBuilder markupBuilder = new StringBuilder();

            bool encounteredMarkup = false;
            
            foreach (var character in text.ToCharArray())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (encounteredMarkup)
                {
                    animatingBuilder.Append(character);
                    markupBuilder.Append(character);

                    if (character == '>')
                    {
                        encounteredMarkup = false;

                        var markup = markupBuilder.ToString();
                        var markupStripped = GetMarkupStripped(markup);
                        
                        // If true, strip our custom tags from the animatingBuilder as TMP only hides its own tags.
                        if (markupStripped == markupShow)
                        {
                            animatingBuilder.Remove(animatingBuilder.Length - markup.Length, markup.Length);

                            dialoguePanel.ShowEffectOnActiveDialogue();
                        }
                        else if (markupStripped == markupHide)
                        {
                            animatingBuilder.Remove(animatingBuilder.Length - markup.Length, markup.Length);

                            dialoguePanel.HideEffectOnActiveDialogue();
                        }
                        else if (markupStripped == markupShake)
                        {
                            animatingBuilder.Remove(animatingBuilder.Length - markup.Length, markup.Length);

                            dialoguePanel.ShakeEffectOnActiveDialogue();
                        }
                        else if (markupStripped == markupWait)
                        {
                            animatingBuilder.Remove(animatingBuilder.Length - markup.Length, markup.Length);
                            int milliSeconds = GetMarkupValue(markup) * 1000;
                            await UniTask.Delay(milliSeconds, cancellationToken: animationCancellation.Token);
                        }
                        else if (markupStripped == markupSpeed)
                        {
                            animatingBuilder.Remove(animatingBuilder.Length - markup.Length, markup.Length);

                            textSpeed = GetMarkupValue(markup);
                        }

                        markupBuilder.Clear();
                    }
                    continue;
                }

                if (character == '<')
                {
                    encounteredMarkup = true;
                    
                    animatingBuilder.Append(character);
                    markupBuilder.Append(character);
                    continue;
                }
                
                await UniTask.Delay(textSpeed, cancellationToken: cancellationToken);

                animatingBuilder.Append(character);
                dialoguePanel.SetDialogueTextOnActiveDialogue(animatingBuilder.ToString());
            }
        }

        private string GetMarkupStripped(string markup)
        {
            string strippedMarkup = markup;

            if (markup.Contains('='))
            {
                strippedMarkup = markup.Substring(1, markup.IndexOf('=') - 1);
            }
            else
            {
                strippedMarkup = markup.Substring(1, markup.IndexOf('>') - 1);
            }

            return strippedMarkup;
        }

        private int GetMarkupValue(string markup)
        {
            int indexAfterEquals = markup.IndexOf('=') + 1;
            int length = markup.Length - 1 - indexAfterEquals;  
            var result = markup.Substring(indexAfterEquals, length);
            return int.Parse(result);
        }

        private async UniTask CustomMarkup(string text)
        {
            // It would be great to move all the markup if statements out of AnimateText and into here.
            // I'm not sure how I would return the different values I need for the animate text speed 
            // and delay.
        }
        
        #endregion Task Handling
        
        #region Data Handling

        private void ResetDialogue()
        {
            currentDialogueHandle.Release();
            currentDialogue = null;
            currentState = DialogueState.NONE;
            currentIndex = 0;
            
            animationCancellation.Dispose();
            animationCancellation = null;
        }
        
        #endregion
    }
}