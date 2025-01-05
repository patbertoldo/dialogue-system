using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialoguePanel : Panel
    {
        [SerializeField] private DialogueContainer dialogueContainerPrefab;
        [SerializeField] private Transform dialogueContainerParent;
        [SerializeField] private Button continueButton;

        [SerializeField] private Transform startPosition;
        [SerializeField] private Transform endPosition;
        
        [SerializeField] DialogueContainer[] dialogueContainers;
        
        DialogueContainer currentDialogueContainer;

        private Action onContinue;

        private DialogueContainer GetDialogueContainer(DialogueContainerState state)
        {
            foreach (var dialogueContainer in dialogueContainers)
            {
                if (dialogueContainer.DialogueContainerState == state)
                {
                    return dialogueContainer;
                }
            }
            return null;
        }

        public void SetContinueEvent(Action onContinue)
        {
            this.onContinue = onContinue;
            continueButton.onClick.AddListener(() => this.onContinue.Invoke());
        }

        public void PlayDialogue(DialogueBlock dialogueBlock)
        {
            currentDialogueContainer = GetDialogueContainer(DialogueContainerState.OFF);
            var dialogueToUnfocus = GetDialogueContainer(DialogueContainerState.FOCUSED);
            var dialogueToFinish = GetDialogueContainer(DialogueContainerState.UNFOCUSED);

            currentDialogueContainer.Initialise(dialogueBlock);
            currentDialogueContainer.PlayFocus(dialogueBlock, startPosition.localPosition);

            dialogueToUnfocus?.PlayUnfocus(endPosition.localPosition);
            dialogueToFinish?.PlayFinished();
        }

        public override void Hide()
        {
            CompleteDialogue();

            base.Hide();
        }

        public void CompleteDialogue()
        {
            currentDialogueContainer = null;
            
            foreach (var dialogueContainer in dialogueContainers)
            {
                dialogueContainer.PlayFinished();
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