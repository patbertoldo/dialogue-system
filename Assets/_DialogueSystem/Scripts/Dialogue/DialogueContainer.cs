using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueContainer : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image portrait;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Animator animator;
        
        // Animations
        private const string flyIn = "FlyIn";
        private const string flyOut = "FlyOut";
        private const string focus = "Focus";
        private const string unfocus = "Unfocus";
        private const string off = "Off";

        public void Initialise(DialogueBlock dialogueBlock)
        {
            if (dialogueBlock == null)
            {
                animator.SetTrigger(off);
                return;
            }
            
            portrait.sprite = dialogueBlock.DialogueCharacter.GetPortrait(dialogueBlock.Emotion);

            nameText.text = dialogueBlock.DialogueCharacter.Name;
            descriptionText.text = "";
            
            animator.SetTrigger(flyIn);
        }

        public void PlayFocus(DialogueBlock dialogueBlock)
        {
            portrait.sprite = dialogueBlock.DialogueCharacter.GetPortrait(dialogueBlock.Emotion);

            nameText.text = dialogueBlock.DialogueCharacter.Name;

            audioSource.clip = dialogueBlock.DialogueCharacter.GetAudioClip(dialogueBlock.Emotion);
            audioSource.Play();
            
            if (canvasGroup.alpha != 1)
                animator.SetTrigger(focus);
        }

        public void PlayUnfocus()
        {
            animator.SetTrigger(unfocus);
        }

        public void SetText(string text)
        {
            descriptionText.text = text;
        }
        
        #region Animation Events

        public void FlyInComplete()
        {
            
        }
        
        public void FlyOutComplete()
        {
            
        }
        
        public void FocusComplete()
        {
            
        }
        
        public void UnfocusComplete()
        {
            
        }
        
        #endregion Animation Events
    }
}
