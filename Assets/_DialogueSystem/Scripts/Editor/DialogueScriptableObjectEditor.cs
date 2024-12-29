using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dialogue.Editor
{
    [CustomEditor(typeof(DialogueScriptableObject))]
    public class DialogueScriptableObjectEditor : UnityEditor.Editor
    {
        private const string buttonText = "Test In Play Mode";
        private const string urlButtonText = "See Supported Text Colors";

        private const string textMeshProURL =
            "https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/RichTextColor.html";
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button(buttonText))
            {
                if (!EditorApplication.isPlaying)
                {
                    EditorApplication.EnterPlaymode();
                }
                else
                {
                    // It would be cool to figure out how to get the asset reference and feed it to the dialogue manager
                    // without having to set up a playmode trigger panel, just trigger straight from the asset itself.
                    var path = AssetDatabase.GetAssetPath(target);
                    var guid = AssetDatabase.AssetPathToGUID(path);
                    var address = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid).address;
                    var assetReference = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings
                        .CreateAssetReference(address);
                    //FindObjectOfType<DialogueManager>().OpenDialogue(assetReference);
                }
            }

            if (GUILayout.Button(urlButtonText))
            {
                Application.OpenURL(textMeshProURL);
            }
        }
    }
}
