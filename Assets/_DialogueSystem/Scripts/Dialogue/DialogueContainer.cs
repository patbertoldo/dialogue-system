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
        private const float fadeTime = 0.25f;

        private const float shakeStrength = 20f;
        private const int shakeVibrato = 100;
        private const float shakeTime = 1f;

        /// <summary>
        /// Hidden stops effects from happening to the container, as it is a custom markup setting <hide>.
        /// Use <show> to reset the flag.
        /// </summary>
        private bool hidden = false;

        public void Initialise(DialogueBlock dialogueBlock)
        {
            canvasGroup.alpha = 0;
            hidden = false;
            
            if (dialogueBlock == null)
                return;
            
            portrait.sprite = dialogueBlock.DialogueCharacter.GetPortrait(dialogueBlock.Emotion);

            nameText.text = dialogueBlock.DialogueCharacter.Name;
            descriptionText.text = "";

            canvasGroup.alpha = fadeOut;
        }

        public void PlayFocus(DialogueBlock dialogueBlock)
        {
            portrait.sprite = dialogueBlock.DialogueCharacter.GetPortrait(dialogueBlock.Emotion);

            nameText.text = dialogueBlock.DialogueCharacter.Name;

            audioSource.clip = dialogueBlock.DialogueCharacter.GetAudioClip(dialogueBlock.Emotion);
            
            if (!hidden)
                ShowEffect();
        }

        public void PlayUnfocus()
        {
            if (!hidden)
                canvasGroup.DOFade(fadeOut, fadeTime);
        }

        public void SetText(string text)
        {
            descriptionText.text = text;
        }

        public void ShowEffect()
        {
            hidden = false;
            
            audioSource.Play();
            canvasGroup.DOFade(fadeIn, fadeTime);
        }

        public void HideEffect()
        {
            hidden = true;
            
            canvasGroup.DOFade(0, fadeTime);
        }

        public void ShakeEffect()
        {
            transform.DOShakePosition(shakeTime, shakeStrength, shakeVibrato);
        }
    }
}
