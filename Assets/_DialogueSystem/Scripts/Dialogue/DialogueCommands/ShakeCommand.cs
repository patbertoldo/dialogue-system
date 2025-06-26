using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "Shake", menuName = "Dialogue System/Commands/Shake")]
    public class ShakeCommand : DialogueCommand
    {
        private Transform transform;
        
        public float Strength = 20f;
        public int Vibrato = 100;
        public float Duration = 1f;

        public override void Initialise(DialogueBlock dialogueBlock, DialogueContainer dialogueContainer, string commandValue)
        {
            transform = dialogueContainer.transform;
        }

        public override async UniTask Execute(CancellationToken token)
        {
            Debug.Log("Play Shake Command");
            
            await transform.DOShakePosition(Duration, Strength, Vibrato)
                .OnComplete(() =>
                {
                    Debug.Log("Shake Complete!");
                }).ToUniTask(cancellationToken: token);
        }
    }
}