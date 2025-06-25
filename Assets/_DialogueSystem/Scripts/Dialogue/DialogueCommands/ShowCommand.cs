using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "Show", menuName = "Dialogue System/Commands/Show")]
    public class ShowCommand : DialogueCommand
    {
        private AudioSource audioSource;
        private CanvasGroup canvasGroup;
        
        private const float fadeIn = 1f;
        public float Duration = 0.25f;

        public void Initialise(DialogueContainer dialogueContainer)
        {
            audioSource = dialogueContainer.AudioSource;
            canvasGroup = dialogueContainer.CanvasGroup;
        }

        public override async UniTask Execute()
        {
            Debug.Log($"Play Show Command for {Duration} seconds.");
            
            audioSource.Play();
            
            await canvasGroup.DOFade(fadeIn, Duration);
        }
    }
}