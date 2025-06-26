using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dialogue
{
    public abstract class DialogueCommand : ScriptableObject
    {
        public abstract void Initialise(DialogueBlock dialogueBlock, DialogueContainer dialogueContainer, string commandValue);
        public abstract UniTask Execute(CancellationToken token);
    }
}
