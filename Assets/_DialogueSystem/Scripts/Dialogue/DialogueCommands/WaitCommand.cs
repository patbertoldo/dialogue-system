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

        public override async UniTask Execute()
        {
            Debug.Log($"Play Wait Command for {Duration} seconds.");
            
            await UniTask.WaitForSeconds(Duration);
        }
    }
}