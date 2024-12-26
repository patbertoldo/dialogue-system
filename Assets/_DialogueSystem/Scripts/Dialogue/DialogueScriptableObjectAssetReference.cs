using UnityEngine.AddressableAssets;

namespace Dialogue
{
    [System.Serializable]
    public class DialogueScriptableObjectAssetReference : AssetReferenceT<DialogueScriptableObject>
    {
        public DialogueScriptableObjectAssetReference(string guid) : base(guid)
        {
            
        }
    }
}
