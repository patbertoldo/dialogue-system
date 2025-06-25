using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "_DialogueCommandDatabase", menuName = "Dialogue System/Commands/Dialogue Command Database", order = -1)]
    public class DialogueCommandDatabase : ScriptableObjectDatabase<DialogueCommand, DialogueCommandAssetReference>
    {
        public override bool HasAddressableOfName(string name)
        {
            foreach (var addressable in AddressableDatabase)
                if (addressable.editorAsset.name == name)
                    return true;
            
            return false;
        }
    }
}
