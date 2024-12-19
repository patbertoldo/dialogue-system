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
        [SerializeField] private DialogueAlignment alignment;
        [SerializeField] private Emotions emotion;
        [SerializeField] private DialogueCharacterScriptableObject dialogueCharacter;
        [TextArea(3, 10)]
        [SerializeField] private string description;
        
        public DialogueAlignment Alignment => alignment;
        public Emotions Emotion => emotion;
        public DialogueCharacterScriptableObject DialogueCharacter => dialogueCharacter;
        public string Description => description;
    }
}