using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "Hide", menuName = "Dialogue System/Commands/Hide")]
    public class HideCommand : DialogueCommand
    {
        private CanvasGroup canvasGroup;
        
        private const float fadeOut = 0f;
        public float Duration = 0.25f;

        public void Initialise(CanvasGroup canvasGroup)
        {
            this.canvasGroup = canvasGroup;
        }

        public override async UniTask Execute()
        {
            Debug.Log($"Play Hide Command for {Duration} seconds.");
            
            await canvasGroup.DOFade(fadeOut, Duration);
        }
    }
}