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
    /// is performing a task, or has finished a task and is ready to move on.
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
        
        // Text
        private StringBuilder animatingBuilder;
        private StringBuilder markupBuilder;
        private bool skip;
        
        // Custom Markup
        private const string markupWait = "wait";
        private const string markupSpeed = "speed";
        private const string markupShake = "shake";
        private const string markupShow = "show";
        private const string markupHide = "hide";
        private const string markupEmotion = "emotion";
        
        public DialogueManager(DialoguePanel dialoguePanel)
        {
            this.dialoguePanel = dialoguePanel;

            currentDialogue = null;
            currentState = DialogueState.NONE;
            currentIndex = 0;
            
            animatingBuilder = new StringBuilder();
            markupBuilder = new StringBuilder();
            skip = false;
            
            dialoguePanel.SetContinueEvent(ContinueDialogue);
        }

        #region Dialogue Handling
        
        public async void OpenDialogue(DialogueScriptableObjectAssetReference dialogueAddressable)
        {
            currentDialogueHandle = Addressables.LoadAssetAsync<DialogueScriptableObject>(dialogueAddressable);
            currentDialogue = await currentDialogueHandle;
            
            // Get the first dialogue for the left side of the conversation and the right.
            // For dialogues that only have one side, pass null for the other side.
            DialogueBlock dialogueBlockLeft = currentDialogue.GetFirstInstanceOfAlignment(DialogueAlignment.LEFT);
            DialogueBlock dialogueBlockRight = currentDialogue.GetFirstInstanceOfAlignment(DialogueAlignment.RIGHT);
            
            dialoguePanel.InitialiseDialogue(dialogueBlockLeft, dialogueBlockRight);

            var task = UniTask.Delay(100).GetAwaiter();
            
            task.OnCompleted(PlayDialogue);
        }

        /// <summary>
        /// UI event. Animating text will skip to completion, or the current dialogue will close.
        /// </summary>
        private void ContinueDialogue()
        {
            switch (currentState)
            {
                case DialogueState.PLAY:
                {
                    skip = true;
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
            skip = false;
            
            animationCancellation = new CancellationTokenSource();
            
            var dialogueBlock = currentDialogue.DialogueBlocks[currentIndex];
            dialoguePanel.NextDialogue(dialogueBlock);

            await BuildDialogueText(dialogueBlock.Description, dialogueBlock.TextSpeed);

            currentState = DialogueState.FINISHED;
        }

        private void FinishDialogue()
        {
            animationCancellation.Dispose();
            
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

        private async UniTask BuildDialogueText(string rawDialogueText, int textSpeed)
        {
            animatingBuilder.Clear();
            markupBuilder.Clear();

            bool encounteredMarkup = false;
            
            foreach (var character in rawDialogueText.ToCharArray())
            {
                if (character == '<')
                {
                    encounteredMarkup = true;
                    
                    animatingBuilder.Append(character);
                    markupBuilder.Append(character);
                    continue;
                }
                
                if (encounteredMarkup)
                {
                    animatingBuilder.Append(character);
                    markupBuilder.Append(character);

                    if (character == '>')
                    {
                        encounteredMarkup = false;

                        var markup = markupBuilder.ToString();
                        var markupStripped = GetMarkupStripped(markup);
                        
                        // It would be nice to handle this area better.
                        if (EvaluateMarkupForEffects(markupStripped, markupShow, animatingBuilder, markup))
                        {
                            dialoguePanel.ShowEffectOnActiveDialogue();
                        }
                        else if (EvaluateMarkupForEffects(markupStripped, markupHide, animatingBuilder, markup))
                        {
                            dialoguePanel.HideEffectOnActiveDialogue();
                        }
                        else if (EvaluateMarkupForEffects(markupStripped, markupShake, animatingBuilder, markup))
                        {
                            dialoguePanel.ShakeEffectOnActiveDialogue();
                        }
                        else if (EvaluateMarkupForEffects(markupStripped, markupWait, animatingBuilder, markup))
                        {
                            int milliSeconds = GetMarkupValueAsInt(markup) * 1000;
                            
                            if (!skip)
                            {
                                await TryUniTask(UniTask.Delay(milliSeconds, cancellationToken: animationCancellation.Token));
                            }
                        }
                        else if (EvaluateMarkupForEffects(markupStripped, markupSpeed, animatingBuilder, markup))
                        {
                            textSpeed = GetMarkupValueAsInt(markup);
                        }
                        else if (EvaluateMarkupForEffects(markupStripped, markupEmotion, animatingBuilder, markup))
                        {
                            Emotions emotion = Enum.Parse<Emotions>(GetMarkupValueAsString(markup).ToUpper());
                            dialoguePanel.EmotionEffectOnActiveDialogue(currentDialogue.DialogueBlocks[currentIndex], emotion);
                        }

                        markupBuilder.Clear();
                    }
                    continue;
                }

                if (!skip)
                {
                    await TryUniTask(UniTask.Delay(textSpeed, cancellationToken: animationCancellation.Token));
                }

                animatingBuilder.Append(character);
                dialoguePanel.SetDialogueTextOnActiveDialogue(animatingBuilder.ToString());
            }
        }

        private string GetMarkupStripped(string markup)
        {
            string strippedMarkup = markup.Contains('=')
                ? markup.Substring(1, markup.IndexOf('=') - 1)
                : markup.Substring(1, markup.IndexOf('>') - 1);

            return strippedMarkup;
        }

        private int GetMarkupValueAsInt(string markup)
        {
            int indexAfterEquals = markup.IndexOf('=') + 1;
            int length = markup.Length - 1 - indexAfterEquals;  
            var result = markup.Substring(indexAfterEquals, length);
            return int.Parse(result);
        }
        
        private string GetMarkupValueAsString(string markup)
        {
            int indexAfterEquals = markup.IndexOf('=') + 1;
            int length = markup.Length - 1 - indexAfterEquals;  
            return markup.Substring(indexAfterEquals, length);
        }

        private bool EvaluateMarkupForEffects(string markupStripped, string effectString, StringBuilder stringBuilder,
            string markupToRemove)
        {
            // If true, strip our custom tags from the animatingBuilder as TMP only hides its own tags.
            if (markupStripped == effectString)
            {
                stringBuilder.Remove(stringBuilder.Length - markupToRemove.Length, markupToRemove.Length);
                return true;
            }

            return false;
        }

        private async UniTask TryUniTask(UniTask uniTask)
        {
            try
            {
                await uniTask;
            }
            catch (Exception e)
            {
                // Ignore, cancellations are expected when the player skips.
            }
        }
        
        #endregion Task Handling
        
        #region Data Handling

        private void ResetDialogue()
        {
            currentDialogueHandle.Release();
            currentDialogue = null;
            currentState = DialogueState.NONE;
            currentIndex = 0;
        }
        
        #endregion
    }
}