using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "_DialogueCommandDatabase", menuName = "Dialogue System/Commands/Dialogue Command Database", order = -1)]
    public class DialogueCommandDatabase : ScriptableObjectDatabase<DialogueCommand, DialogueCommandAssetReference>
    {
        public override async UniTask LoadAsync()
        {
            foreach (var addressable in AddressableDatabase)
            {
                var handler = await Addressables.LoadAssetAsync<DialogueCommand>(addressable);
                LoadedScriptableObjects.Add(handler.name, handler);
            }
        }

        public override bool HasAddressableOfName(string name)
        {
            foreach (var addressable in AddressableDatabase)
                if (addressable.editorAsset.name == name)
                    return true;
            
            return false;
        }

        public DialogueCommand GetCommandInstanceOfName(string commandName)
        {
            if (LoadedScriptableObjects.TryGetValue(commandName, out DialogueCommand command))
            {
                return ScriptableObject.Instantiate(command);
            }
            
            Debug.LogError($"Failed to create a new instance of {commandName}");
            return null;
        }
    }
}
