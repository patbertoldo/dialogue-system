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
        private UniTask animateTextTask;
        private DialogueState currentState;
        private int currentIndex;
        
        // Tasks
        private AsyncOperationHandle<DialogueScriptableObject> currentDialogueHandle;
        
        private CancellationTokenSource animationCancellation;
        private StringBuilder stringBuilder;

        
        public DialogueManager(DialoguePanel dialoguePanel)
        {
            this.dialoguePanel = dialoguePanel;

            currentDialogue = null;
            currentState = DialogueState.NONE;
            currentIndex = 0;
            
            animationCancellation = new CancellationTokenSource();
            stringBuilder = new StringBuilder();
            
            dialoguePanel.SetContinueEvent(ContinueDialogue);
        }

        public void CleanUp()
        {
            //animationCancellation.Cancel();
            animationCancellation.Dispose();
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
                await AnimateText(dialogueBlock.Description, animationCancellation.Token);
                
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

            dialoguePanel.SetDialogueText(dialogueBlock.Description);
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
        
        private async UniTask AnimateText(string text, CancellationToken cancellationToken)
        {
            stringBuilder.Clear();
            
            foreach (var character in text.ToCharArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                await UniTask.Delay(20, cancellationToken: cancellationToken);

                stringBuilder.Append(character);
                dialoguePanel.SetDialogueText(stringBuilder.ToString());
            }
        }
        #endregion Task Handling
        
        #region Data Handling

        private void DialogueScriptableObjectAsyncCompleted(
            AsyncOperationHandle<DialogueScriptableObject> asyncOperationHandle)
        {
            
        }

        private void ResetDialogue()
        {
            currentDialogue = null;
            currentState = DialogueState.NONE;
            currentIndex = 0;
            
            animationCancellation.Dispose();
            animationCancellation = null;
        }
        
        #endregion
    }
}