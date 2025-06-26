using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "Wait", menuName = "Dialogue System/Commands/Wait")]
    public class WaitCommand : DialogueCommand
    {
        public float Duration;

        public override void Initialise(DialogueBlock dialogueBlock, DialogueContainer dialogueContainer, string commandValue)
        {
        }

        public override async UniTask Execute(CancellationToken token)
        {
            Debug.Log($"Play Wait Command for {Duration} seconds.");
            
            await UniTask.WaitForSeconds(Duration, cancellationToken: token);
        }
    }
}