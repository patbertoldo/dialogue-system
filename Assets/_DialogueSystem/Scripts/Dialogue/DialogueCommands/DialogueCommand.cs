using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dialogue
{
    public abstract class DialogueCommand : ScriptableObject
    {
        public abstract UniTask Execute();
    }
}
