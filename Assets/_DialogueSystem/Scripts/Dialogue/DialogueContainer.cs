using System.Collections;
using System.Collections.Generic;
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

        public void Initialise(DialogueBlock dialogueBlock)
        {
            portrait.sprite = dialogueBlock.DialogueCharacter.GetPortrait(dialogueBlock.Emotion);

            nameText.text = dialogueBlock.DialogueCharacter.Name;
            descriptionText.text = dialogueBlock.Description;

            audioSource.clip = dialogueBlock.DialogueCharacter.GetAudioClip(dialogueBlock.Emotion);
            audioSource.Play();
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
