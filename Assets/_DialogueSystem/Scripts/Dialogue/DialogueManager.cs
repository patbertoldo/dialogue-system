using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
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
        PLAYING,        // When commands are executing.
        SKIPPED,        // When in player input force finishes the text animation.
        FINISHED        // When text has finished animating.
    }
    
    public class DialogueManager
    {
        // References
        private DialoguePanel dialoguePanel;
        private DialogueCommandDatabase dialogueCommandDatabase;
        
        // States
        private DialogueScriptableObject currentDialogue;
        private DialogueState currentState;
        private int currentIndex;
        
        // Commands
        private DialogueCommandManager dialogueCommandManager;
        // Where the outer list represents individual DialogueBlocks, the inner list is the commands for that block.
        private List<List<DialogueCommand>> dialogueBlockCommandLists = new ();
        
        // Tasks
        private AsyncOperationHandle<DialogueScriptableObject> currentDialogueHandle;
        private CancellationTokenSource tokenSource;
        
        // Text
        private StringBuilder animatingBuilder;
        private StringBuilder markupBuilder;
        private int textSpeed;
        private bool skip;
        
        public DialogueManager(DialoguePanel dialoguePanel, DialogueCommandDatabase dialogueCommandDatabase)
        {
            this.dialoguePanel = dialoguePanel;
            this.dialogueCommandDatabase = dialogueCommandDatabase;

            currentDialogue = null;
            currentState = DialogueState.NONE;
            currentIndex = 0;

            dialogueCommandManager = new DialogueCommandManager();
            
            
            animatingBuilder = new StringBuilder();
            markupBuilder = new StringBuilder();
            textSpeed = 0;
            skip = false;
            
            dialoguePanel.SetContinueEvent(ContinueDialogue);
        }

        #region Dialogue Handling
        
        public void LoadDialogue(DialogueScriptableObjectAssetReference dialogueAddressable)
        {
            LoadDialogueAsync(dialogueAddressable).Forget();
        }

        private async UniTask LoadDialogueAsync(DialogueScriptableObjectAssetReference dialogueAddressable)
        {
            // Load Addressable
            currentDialogueHandle = Addressables.LoadAssetAsync<DialogueScriptableObject>(dialogueAddressable);
            currentDialogue = await currentDialogueHandle;
            
            // Load Commands
            dialogueBlockCommandLists.Clear();

            foreach (var dialogueBlock in currentDialogue.DialogueBlocks)
            {
                List<DialogueCommand> dialogueCommands = new();
                
                foreach (var commandData in dialogueBlock.CommandDatas)
                {
                    dialogueCommands.Add(dialogueCommandDatabase.GetCommandInstanceOfName(commandData.Name));
                }
                
                dialogueBlockCommandLists.Add(dialogueCommands);
            }
            
            // Show Dialogue Panel
            await dialoguePanel.Show();
            
            await PlayDialogue();
        }

        /// <summary>
        /// UI event. Animating text will skip to completion, or the current dialogue will close.
        /// </summary>
        private void ContinueDialogue()
        {
            switch (currentState)
            {
                case DialogueState.PLAYING:
                {
                    skip = true;
                    tokenSource.Cancel();
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

        private async UniTask PlayDialogue()
        {
            var dialogueBlock = currentDialogue.DialogueBlocks[currentIndex];

            currentState = DialogueState.PLAYING;
            textSpeed = dialogueBlock.TextSpeed;
            skip = false;
            // Each new dialogue needs a new cancellation token. It doesn't seem like tokens that have been
            // cancelled can be recycled.
            tokenSource = new CancellationTokenSource();
            
            foreach (var dbCommands in dialogueBlockCommandLists[currentIndex])
            {
                await dbCommands.Execute(tokenSource.Token);
            }
            
            //dialogueCommandManager.BuildCommands(dialogueBlock.Description);


            bool isSameDialogueCharacter = false;
            if (currentIndex > 0)
            {
                isSameDialogueCharacter = dialogueBlock.DialogueCharacter ==
                                  currentDialogue.DialogueBlocks[currentIndex - 1].DialogueCharacter;
            }
            
            //dialoguePanel.ConfigureDialogue(dialogueBlock);

            //dialogueCommandManager.PlayCommands();
            
            //await BuildDialogueText(dialogueBlock, isSameDialogueCharacter);

            //dialoguePanel.SetCompletedOnActiveDialogue();
            
            currentState = DialogueState.FINISHED;
        }

        private void FinishDialogue()
        {
            tokenSource.Dispose();
            
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

        /// <summary>
        /// Try a UniTask only when we want it to be cancellable.
        /// </summary>
        private async UniTask TryUniTask(UniTask uniTask)
        {
            try
            {
                await uniTask;
            }
            catch (Exception e)
            {
                Debug.Log("Skipped");
                // Ignore, cancellations are expected when the player skips.
            }
        }
        
        private async UniTask<bool> TryUniTask(UniTask<bool> uniTask)
        {
            try
            {
                return await uniTask;
            }
            catch (Exception e)
            {
                // Ignore, cancellations are expected when the player skips.
                return false;
            }
        }
        
        #endregion Task Handling
        
        private void ResetDialogue()
        {
            currentDialogueHandle.Release();
            currentDialogue = null;
            currentState = DialogueState.NONE;
            currentIndex = 0;
        }
    }
}