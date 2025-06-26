using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "Default", menuName = "Dialogue System/Commands/Default")]
    public class DefaultCommand : DialogueCommand
    {
        private int endCharIndex;
        private TMP_Text tmpText;

        [Range(1, 200), Tooltip("Text reveal speed in milliseconds.")]
        public float Speed = 20f;
        
        public override void Initialise(DialogueBlock dialogueBlock, DialogueContainer dialogueContainer, string commandValue)
        {
            tmpText = dialogueContainer.DescriptionText;
            
            if (int.TryParse(commandValue, out endCharIndex) == false)
            {
                Debug.LogError($"Failed to parse {commandValue} as int");
            }
        }

        public override async UniTask Execute(CancellationToken token)
        {
            Debug.Log($"Play Text Command at {Speed}ms.");
            
            int index = tmpText.maxVisibleCharacters;

            while (index < endCharIndex)
            {
                //Debug.Log(index);
                tmpText.maxVisibleCharacters = index;
                index++;
                await UniTask.WaitForSeconds(Speed * Time.deltaTime, cancellationToken: token);
            }
        }
    }
}
