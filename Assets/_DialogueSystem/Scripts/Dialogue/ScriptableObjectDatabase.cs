using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dialogue
{
    public abstract class ScriptableObjectDatabase<TScriptable, TAddressable> : ScriptableObject
    {
        public TAddressable[] AddressableDatabase;
        
        protected Dictionary<string, TScriptable> LoadedScriptableObjects = new();

        public abstract UniTask LoadAsync();

        public abstract bool HasAddressableOfName(string name);
    }
}
