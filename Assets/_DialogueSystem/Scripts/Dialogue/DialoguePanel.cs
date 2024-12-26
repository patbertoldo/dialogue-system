using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;

namespace Dialogue
{
    public class DialoguePanel : Panel
    {
        [SerializeField] private DialogueContainer dialogueContainerLeft;
        [SerializeField] private DialogueContainer dialogueContainerRight;
        [SerializeField] private Button continueButton;

        private Action onContinue;
        
        DialogueContainer currentDialogueContainer;
        DialogueContainer previousDialogueContainer;

        public void SetContinueEvent(Action onContinue)
        {
            this.onContinue = onContinue;
            continueButton.onClick.AddListener(() => this.onContinue.Invoke());
        }
        
        public void InitialiseDialogue(DialogueBlock dialogueBlockLeft, DialogueBlock dialogueBlockRight)
        {
            Show();

            dialogueContainerLeft.Initialise(dialogueBlockLeft);
            dialogueContainerRight.Initialise(dialogueBlockRight);
        }

        public void NextDialogue(DialogueBlock dialogueBlock)
        {
            currentDialogueContainer = dialogueBlock.Alignment == DialogueAlignment.LEFT
                ? dialogueContainerLeft
                : dialogueContainerRight;
            
            currentDialogueContainer.PlayFocus(dialogueBlock);

            // Unfocus the previous dialogue if needed.
            if (previousDialogueContainer != null)
            {
                if (previousDialogueContainer != currentDialogueContainer)
                {
                    previousDialogueContainer.PlayUnfocus();
                }
            }
            
            previousDialogueContainer = currentDialogueContainer;
        }

        public void SetDialogueTextOnActiveDialogue(string text)
        {
            currentDialogueContainer.SetText(text);
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
        
        #endregion Effects
    }
}