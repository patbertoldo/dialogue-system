using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialoguePanel : Panel
    {
        [SerializeField] private Transform dialogueContainerParent;
        [SerializeField] private Button continueButton;

        [SerializeField] private Transform startPosition;
        [SerializeField] private Transform endPosition;
        
        [SerializeField] private DialogueContainer[] dialogueContainers;
        
        DialogueContainer currentDialogueContainer;

        private Action onContinue;

        public DialogueContainer GetDialogueContainer(DialogueContainerState state)
        {
            foreach (var dialogueContainer in dialogueContainers)
            {
                if (dialogueContainer.DialogueContainerState == state)
                {
                    return dialogueContainer;
                }
            }
            Debug.LogError($"No Dialogue Container found with state: {state}");
            return null;
        }

        public void SetContinueEvent(Action onContinue)
        {
            this.onContinue = onContinue;
            continueButton.onClick.AddListener(() => this.onContinue.Invoke());
        }

        public void ConfigureDialogue(DialogueBlock dialogueBlock, DialogueContainer dialogueContainer)
        {
            currentDialogueContainer = GetDialogueContainer(DialogueContainerState.OFF);
            var dialogueToUnfocus = GetDialogueContainer(DialogueContainerState.FOCUSED);
            var dialogueToFinish = GetDialogueContainer(DialogueContainerState.UNFOCUSED);

            currentDialogueContainer.Initialise(dialogueBlock);
            currentDialogueContainer.PlayFocus(dialogueBlock, startPosition.localPosition);

            dialogueToUnfocus?.PlayUnfocus(endPosition.localPosition);
            dialogueToFinish?.PlayFinished();
        }

        public override async UniTask Hide()
        {
            CompleteDialogue();

            base.Hide();
        }

        public async UniTask CompleteDialogue()
        {
            currentDialogueContainer = null;
            
            foreach (var dialogueContainer in dialogueContainers)
            {
                await dialogueContainer.PlayFinished();
            }
        }

        public void SetDialogueTextOnActiveDialogue(string text)
        {
            currentDialogueContainer.SetText(text);
        }

        public void SetCompletedOnActiveDialogue()
        {
            currentDialogueContainer.TextCompleted();
        }
        
        #region Effects

        public void ShowEffectOnActiveDialogue()
        {
            currentDialogueContainer.ShowEffect();
        }
        
        public void HideEffectOnActiveDialogue()
        {
            currentDialogueContainer.HideEffect();
        }
        
        public void ShakeEffectOnActiveDialogue()
        {
            currentDialogueContainer.ShakeEffect();
        }

        public void EmotionEffectOnActiveDialogue(DialogueBlock dialogueBlock, Emotions emotion)
        {
            currentDialogueContainer.EmotionEffect(dialogueBlock, emotion);
        }
        
        #endregion Effects
    }
}