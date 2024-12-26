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

        public void SetContinueEvent(Action onContinue)
        {
            this.onContinue = onContinue;
            continueButton.onClick.AddListener(() => this.onContinue.Invoke());
        }
        
        public void InitialiseDialogue(DialogueBlock dialogueBlock)
        {
            Show();
            
            currentDialogueContainer =
                dialogueBlock.Alignment == DialogueAlignment.LEFT ?
                dialogueContainerLeft :
                dialogueContainerRight;

            currentDialogueContainer.Initialise(dialogueBlock);
        }

        public void SetDialogueText(string text)
        {
            currentDialogueContainer.SetText(text);
        }

        public void Skip()
        {
            onContinue.Invoke();
        }
    }
}