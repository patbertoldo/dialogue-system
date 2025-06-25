using UnityEngine;
using UnityEditor;

namespace Dialogue.Editor
{
    [CustomEditor(typeof(DialogueScriptableObject))]
    public class DialogueScriptableObjectEditor : UnityEditor.Editor
    {
        private const string urlButtonText = "See Supported Text Colors";

        private const string textMeshProURL =
            "https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/RichTextColor.html";
        
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button(urlButtonText))
            {
                Application.OpenURL(textMeshProURL);
            }
            
            DrawDefaultInspector();
        }
    }
}
