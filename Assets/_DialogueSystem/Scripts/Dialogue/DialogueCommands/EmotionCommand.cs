using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "Emotion", menuName = "Dialogue System/Commands/Emotion")]
    public class EmotionCommand : DialogueCommand
    {
        private DialogueBlock dialogueBlock;
        private Image portrait;
        private AudioSource audioSource;
        
        public Emotions Emotion;
        public bool WithSound = true;

        public override void Initialise(DialogueBlock dialogueBlock, DialogueContainer dialogueContainer, string commandValue)
        {
            this.dialogueBlock = dialogueBlock;
            portrait = dialogueContainer.Portait;
            audioSource = dialogueContainer.AudioSource;
        }

        public override async UniTask Execute(CancellationToken token)
        {
            Debug.Log($"Play Emotion Command of {Emotion} with sound: {WithSound}");
            
            portrait.sprite = dialogueBlock.DialogueCharacter.GetPortrait(Emotion);

            if (WithSound)
            {
                audioSource.clip = dialogueBlock.DialogueCharacter.GetAudioClip(Emotion);
                audioSource.Play();
            }

            await UniTask.WaitForEndOfFrame(token);
        }
    }
}