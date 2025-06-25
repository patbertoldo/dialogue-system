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
        
        public Dictionary<System.Type, TScriptable> LoadedScriptableObjects = new();

        public async UniTask LoadAsync()
        {
            foreach (var addressable in AddressableDatabase)
            {
                var handler = await Addressables.LoadAssetAsync<TScriptable>(addressable);
                LoadedScriptableObjects.Add(handler.GetType(), handler);
            }
        }

        public abstract bool HasAddressableOfName(string name);
    }
}
