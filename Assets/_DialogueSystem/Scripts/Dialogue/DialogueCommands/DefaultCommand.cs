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
        private int startCharIndex;
        private int endCharIndex;
        private TMP_Text tmpText;

        [Range(1, 200), Tooltip("Text reveal speed in milliseconds.")]
        public float Speed = 20f;
        
        public void Initialise(TMP_Text tmpText, int endCharIndex)
        {
            this.tmpText = tmpText;
            this.endCharIndex = endCharIndex;
        }

        public override async UniTask Execute()
        {
            Debug.Log($"Play Text Command at {Speed}ms.");

            int index = startCharIndex;

            while (index < endCharIndex)
            {
                //Debug.Log(index);
                tmpText.maxVisibleCharacters = index;
                index++;
                await UniTask.WaitForSeconds(Speed * Time.deltaTime);
            }
        }
    }
}
