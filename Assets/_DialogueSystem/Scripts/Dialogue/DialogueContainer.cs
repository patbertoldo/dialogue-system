using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public enum DialogueContainerState
    {
        OFF,
        FOCUSED,
        UNFOCUSED
    }
    
    public class DialogueContainer : MonoBehaviour
    {
        [SerializeField] private DialogueContainerLayout layoutLeft;
        [SerializeField] private DialogueContainerLayout layoutRight;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GameObject completedGO;
        
        private Image portrait;
        private TMP_Text descriptionText;

        private DialogueContainerState dialogueContainerState = DialogueContainerState.OFF;
        public DialogueContainerState DialogueContainerState => dialogueContainerState;
        
        // Tweening
        private const float fadeIn = 1f;
        private const float fadeOut = 0.75f;
        private const float fadeTime = 0.25f;

        private const float shakeStrength = 20f;
        private const int shakeVibrato = 100;
        private const float shakeTime = 1f;

        private const float moveTime = 0.25f;
        private Vector3 moveScale = new (0.9f, 0.9f, 0.9f);
        
        /// <summary>
        /// Stops multi-shake calls from occuring, which can set a new position.
        /// </summary>
        private bool isShaking = false;

        /// <summary>
        /// Hidden stops effects from happening to the container, as it is a custom markup setting <hide>.
        /// Use <show> to reset the flag.
        /// </summary>
        private bool hidden = false;

        public void Initialise(DialogueBlock dialogueBlock)
        {
            canvasGroup.alpha = 0;
            isShaking = false;
            hidden = false;
            transform.localScale = Vector3.one;
            canvasGroup.alpha = 0;
            gameObject.SetActive(true);
            completedGO.SetActive(false);
            layoutLeft.gameObject.SetActive(false);
            layoutRight.gameObject.SetActive(false);

            DialogueContainerLayout layout = dialogueBlock.Alignment == DialogueAlignment.LEFT
                ? layoutLeft
                : layoutRight;
            layout.gameObject.SetActive(true);

            portrait = layout.Portrait;
            portrait.sprite = dialogueBlock.DialogueCharacter.GetPortrait(dialogueBlock.Emotion);
            audioSource.clip = dialogueBlock.DialogueCharacter.GetAudioClip(dialogueBlock.Emotion);

            descriptionText = layout.DescriptionText;
            descriptionText.text = "";
        }

        public void PlayFocus(DialogueBlock dialogueBlock, Vector3 position)
        {
            dialogueContainerState = DialogueContainerState.FOCUSED;
            
            portrait.sprite = dialogueBlock.DialogueCharacter.GetPortrait(dialogueBlock.Emotion);
            transform.localPosition = position;
            transform.SetAsLastSibling();

            ShowEffect();
        }

        public void PlayUnfocus(Vector3 position)
        {
            dialogueContainerState = DialogueContainerState.UNFOCUSED;
            
            completedGO.SetActive(false);

            transform.DOLocalMove(position, moveTime);
            transform.DOScale(moveScale, moveTime);
            
            if (!hidden)
                canvasGroup.DOFade(fadeOut, fadeTime);
        }

        public void PlayFinished(bool forceOff = false)
        {
            dialogueContainerState = DialogueContainerState.OFF;
            
            canvasGroup.DOFade(0, fadeTime).OnComplete(() => completedGO.SetActive(false));
        }

        public void SetText(string text)
        {
            descriptionText.text = text;
        }

        public void TextCompleted()
        {
            completedGO.SetActive(true);
        }

        private void OnDisable()
        {
            canvasGroup.DOKill(true);
            transform.DOKill(true);
        }

        #region Effects

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
            if (isShaking)
                return;
            
            isShaking = true;
            transform.DOShakePosition(shakeTime, shakeStrength, shakeVibrato)
                .OnComplete(() =>
                {
                    isShaking = false;
                });
        }

        public void EmotionEffect(DialogueBlock dialogueBlock, Emotions emotion)
        {
            portrait.sprite = dialogueBlock.DialogueCharacter.GetPortrait(emotion);
            audioSource.clip = dialogueBlock.DialogueCharacter.GetAudioClip(emotion);

            audioSource.Play();
        }
        
        #endregion Effects
    }
}
