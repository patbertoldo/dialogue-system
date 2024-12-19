using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialoguePanel : Panel
    {
        [SerializeField] private DialogueContainer dialogueContainerLeft;
        [SerializeField] private DialogueContainer dialogueContainerRight;
        [SerializeField] private Button continueButton;

        private Action onContinue;
        
        public void SetContinueEvent(Action onContinue)
        {
            this.onContinue = onContinue;
            continueButton.onClick.AddListener(() => this.onContinue.Invoke());
        }
        
        public void ShowDialogue(DialogueBlock dialogueBlock)
        {
            Show();
            
            var currentContainer =
                dialogueBlock.Alignment == DialogueAlignment.LEFT ?
                dialogueContainerLeft :
                dialogueContainerRight;

            currentContainer.Initialise(dialogueBlock);
        }

        public void Skip()
        {
            onContinue.Invoke();
        }
    }
}