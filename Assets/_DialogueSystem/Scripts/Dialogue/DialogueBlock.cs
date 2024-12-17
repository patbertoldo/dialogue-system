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
        [SerializeField] private string description;
    }
}