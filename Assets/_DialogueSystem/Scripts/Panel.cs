using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Dialogue
{
    public class Panel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        private const float fadeIn = 1f;
        private const float fadeOut = 0f;
        private const float fadeTime = 0.15f;
        
        public virtual void Show()
        {
            gameObject.SetActive(true);
            
            if (canvasGroup)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(fadeIn, fadeTime);
            }
        }

        public virtual void Hide()
        {
            if (canvasGroup)
            {
                canvasGroup.DOFade(fadeOut, fadeTime)
                    .OnComplete(() => gameObject.SetActive(false));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
