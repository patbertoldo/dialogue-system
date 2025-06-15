using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public enum DialogueAlignment
    {
        LEFT,
        RIGHT
    }
    
    [Serializable]
    public class DialogueBlock
    {
        public DialogueAlignment Alignment;
        public Emotions Emotion;
        public DialogueCharacterScriptableObject DialogueCharacter;
        [TextArea(3, 10)]
        public string Description;
        [Tooltip("The speed at which the text appears, in milliseconds. 20 is roughly normal speed. Lower is faster.")]
        [Range(1, 200)]
        public int TextSpeed = 20;
    }
}