using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dialogue
{
    [Serializable]
    public class DialogueCommandAssetReference : AssetReferenceT<DialogueCommand>
    {
        public DialogueCommandAssetReference(string guid) : base(guid)
        {
            
        }
    }
}
