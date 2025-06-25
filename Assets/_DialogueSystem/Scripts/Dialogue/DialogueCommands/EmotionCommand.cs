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

        public void Instantiate(DialogueBlock dialogueBlock, Image portrait, AudioSource audioSource)
        {
            this.dialogueBlock = dialogueBlock;
            this.portrait = portrait;
            this.audioSource = audioSource;
        }

        public override async UniTask Execute()
        {
            Debug.Log($"Play Emotion Command with sound: {WithSound}");
            
            portrait.sprite = dialogueBlock.DialogueCharacter.GetPortrait(Emotion);

            if (WithSound)
            {
                audioSource.clip = dialogueBlock.DialogueCharacter.GetAudioClip(Emotion);
                audioSource.Play();
            }

            await UniTask.WaitForEndOfFrame();
        }
    }
}