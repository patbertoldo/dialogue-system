using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public enum Emotions
    {
        DEFAULT,
        HAPPY,
        SAD,
        ANGRY,
        THINKING
    }
    
    [CreateAssetMenu(fileName = "NewDialogueCharacter", menuName = "Dialogue System/DialogueCharacter", order = 1)]
    public class DialogueCharacterScriptableObject : ScriptableObject
    {
        [SerializeField] private new string name;
        [Tooltip("Portraits should be ordered according to emotions.")]
        [SerializeField] private Sprite[] portraits;
        [Tooltip("Sounds should be ordered according to emotions.")]
        [SerializeField] private AudioClip[] audioClips;

        public string Name => name;
        
        public Sprite GetPortrait(Emotions emotion)
        {
            return portraits[(int)emotion];
        }

        public AudioClip GetAudioClip(Emotions emotion)
        {
            return audioClips[(int)emotion];
        }
    }
}