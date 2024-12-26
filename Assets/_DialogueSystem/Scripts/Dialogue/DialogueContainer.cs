using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
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
        
        // Tweening
        private const float fadeIn = 1f;
        private const float fadeOut = 0.75f;
        private const float fadeTime = 0.5f;

        public void Initialise(DialogueBlock dialogueBlock)
        {
            canvasGroup.alpha = 0;

            if (dialogueBlock == null)
                return;
            
            portrait.sprite = dialogueBlock.DialogueCharacter.GetPortrait(dialogueBlock.Emotion);

            nameText.text = dialogueBlock.DialogueCharacter.Name;
            descriptionText.text = "";
            
            canvasGroup.DOFade(fadeIn, fadeTime);
        }

        public void PlayFocus(DialogueBlock dialogueBlock)
        {
            portrait.sprite = dialogueBlock.DialogueCharacter.GetPortrait(dialogueBlock.Emotion);

            nameText.text = dialogueBlock.DialogueCharacter.Name;

            audioSource.clip = dialogueBlock.DialogueCharacter.GetAudioClip(dialogueBlock.Emotion);
            audioSource.Play();
            
            canvasGroup.DOFade(fadeIn, fadeTime);
        }

        public void PlayUnfocus()
        {
            canvasGroup.DOFade(fadeOut, fadeTime);
        }

        public void SetText(string text)
        {
            descriptionText.text = text;
        }
    }
}
