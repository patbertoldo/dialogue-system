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
        NONE,
        IN_PROGRESS,    // When text is in an animating state.
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
            
            currentState = DialogueState.IN_PROGRESS;
            
            dialoguePanel.InitialiseDialogue(currentDialogue.DialogueBlocks[currentIndex]);

            await AnimateText(currentDialogue.DialogueBlocks[currentIndex].Description, animationCancellation.Token).SuppressCancellationThrow();
        }

        private async void ContinueDialogue()
        {
            // Continue if the current index is finished.
            if (currentState == DialogueState.FINISHED)
            {
                // Start the next dialogue if there is another index.
                if (currentIndex < currentDialogue.DialogueBlocks.Length - 1)
                {
                    currentState = DialogueState.IN_PROGRESS;
                    currentIndex++;
            
                    dialoguePanel.InitialiseDialogue(currentDialogue.DialogueBlocks[currentIndex]);
                    await AnimateText(currentDialogue.DialogueBlocks[currentIndex].Description, animationCancellation.Token).SuppressCancellationThrow();
                }
                // Finished.
                else
                {
                    CloseDialogue();
                }
            }
            // Skip progress and finish current dialogue.
            else
            {
                animationCancellation.Cancel();
                
                dialoguePanel.SetDialogueText(currentDialogue.DialogueBlocks[currentIndex].Description);
                
                currentState = DialogueState.FINISHED;
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
                //cancellationToken.ThrowIfCancellationRequested();
                if (cancellationToken.IsCancellationRequested)
                    return;
                // // If player skips, break early.
                // if (currentState == DialogueState.FINISHED)
                // {
                //     dialoguePanel.SetDialogueText(text);
                //
                //     return;
                // }
                
                await UniTask.Delay(20, cancellationToken: cancellationToken).SuppressCancellationThrow();

                stringBuilder.Append(character);
                dialoguePanel.SetDialogueText(stringBuilder.ToString());
            }
            
            currentState = DialogueState.FINISHED;
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
        }
        
        #endregion
    }
}