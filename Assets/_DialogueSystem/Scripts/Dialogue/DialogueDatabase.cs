using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "_DialogueDatabase", menuName = "Dialogue System/Dialogue Database", order = -1)]
    public class DialogueDatabase : ScriptableObjectDatabase<DialogueScriptableObject, DialogueScriptableObjectAssetReference>
    {
        public override async UniTask LoadAsync()
        {
            foreach (var addressable in AddressableDatabase)
            {
                var handler = await Addressables.LoadAssetAsync<DialogueScriptableObject>(addressable);
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
    }
}
