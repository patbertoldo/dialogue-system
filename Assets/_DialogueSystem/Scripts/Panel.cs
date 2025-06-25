using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        
        public virtual async UniTask Show()
        {
            gameObject.SetActive(true);
            
            if (canvasGroup)
            {
                canvasGroup.alpha = 0f;
                await canvasGroup.DOFade(fadeIn, fadeTime);
            }
        }

        public virtual async UniTask Hide()
        {
            if (canvasGroup)
            {
                await canvasGroup.DOFade(fadeOut, fadeTime)
                    .OnComplete(() => gameObject.SetActive(false));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
