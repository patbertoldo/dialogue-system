using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        private int textSpeed;
        private bool skip;
        
        // Custom Markup
        private const string markupShow = "show";
        private const string markupHide = "hide";
        private const string markupShake = "shake";
        private const string markupWait = "wait";
        private const string markupSpeed = "speed";
        private const string markupEmotion = "emotion";
        
        public DialogueManager(DialoguePanel dialoguePanel)
        {
            this.dialoguePanel = dialoguePanel;

            currentDialogue = null;
            currentState = DialogueState.NONE;
            currentIndex = 0;
            
            animatingBuilder = new StringBuilder();
            markupBuilder = new StringBuilder();
            textSpeed = 0;
            skip = false;
            
            dialoguePanel.SetContinueEvent(ContinueDialogue);
        }

        #region Dialogue Handling
        
        public async void OpenDialogue(DialogueScriptableObjectAssetReference dialogueAddressable)
        {
            currentDialogueHandle = Addressables.LoadAssetAsync<DialogueScriptableObject>(dialogueAddressable);
            currentDialogue = await currentDialogueHandle;
            
            dialoguePanel.Show();

            await UniTask.Delay(100);
            
            PlayDialogue();
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
            var dialogueBlock = currentDialogue.DialogueBlocks[currentIndex];

            currentState = DialogueState.PLAY;
            textSpeed = dialogueBlock.TextSpeed;
            skip = false;
            // Each new dialogue needs a new cancellation token. It doesn't seem like tokens that have been
            // cancelled can be recycled.
            animationCancellation = new CancellationTokenSource();

            bool isSameCharacter = false;
            if (currentIndex > 0)
            {
                isSameCharacter = dialogueBlock.DialogueCharacter ==
                                  currentDialogue.DialogueBlocks[currentIndex - 1].DialogueCharacter;
            }
            
            dialoguePanel.PlayDialogue(dialogueBlock);
            
            await BuildDialogueText(dialogueBlock, isSameCharacter);

            dialoguePanel.SetCompletedOnActiveDialogue();
            
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

        private async UniTask BuildDialogueText(DialogueBlock dialogueBlock, bool isSameCharacter)
        {
            animatingBuilder.Clear();
            markupBuilder.Clear();

            bool encounteredMarkup = false;

            var name = dialogueBlock.DialogueCharacter.Name;
            var nameColor = dialogueBlock.DialogueCharacter.NameColor;
            
            // When it's a new character, add their name to the start of the text. Add bold and color.
            if (!isSameCharacter)
                animatingBuilder.Append($"<b><color={nameColor}>{name}:</color></b> ");
            
            foreach (var character in dialogueBlock.Description.ToCharArray())
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
                        
                        await TryUniTask(CustomMarkupEffects(markup, markupStripped));

                        animatingBuilder.Remove(animatingBuilder.Length - markup.Length, markup.Length);
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

        private T GetMarkupValue<T>(string markup)
        {
            int indexAfterEquals = markup.IndexOf('=') + 1;
            int length = markup.Length - 1 - indexAfterEquals;  
            string result = markup.Substring(indexAfterEquals, length);
            return (T)Convert.ChangeType(result, typeof(T));
        }

        private async UniTask CustomMarkupEffects(string markup, string markupStripped)
        {
            switch (markupStripped)
            {
                case markupShow:
                {
                    dialoguePanel.ShowEffectOnActiveDialogue();
                    break;
                }
                case markupHide:
                {
                    dialoguePanel.HideEffectOnActiveDialogue();
                    break;
                }
                case markupShake:
                {
                    dialoguePanel.ShakeEffectOnActiveDialogue();
                    break;
                }
                case markupWait:
                {
                    int milliSeconds = (int)(GetMarkupValue<float>(markup) * 1000);
                            
                    if (!skip)
                    {
                        await TryUniTask(UniTask.Delay(milliSeconds, cancellationToken: animationCancellation.Token));
                    }
                    break;
                }
                case markupSpeed:
                {
                    textSpeed = GetMarkupValue<int>(markup);
                    break;
                }
                case markupEmotion:
                {
                    Emotions emotion = Enum.Parse<Emotions>(GetMarkupValue<string>(markup).ToUpper());
                    dialoguePanel.EmotionEffectOnActiveDialogue(currentDialogue.DialogueBlocks[currentIndex], emotion);
                    break;
                }
            }
        }

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
                // Ignore, cancellations are expected when the player skips.
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