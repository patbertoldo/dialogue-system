using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dialogue
{
    public static class DialogueUtilities
    {
        [MenuItem("Tools/Dialogue Utilities/Generate Command IDs")]
        public static void GenerateCommandIDs()
        {
            Debug.Log("Generating command IDs");
        }

        [MenuItem("Tools/Dialogue Utilities/Build Dialogue Scriptable Command Data")]
        public static void BuildDialogueCommands()
        {
            if (TryGetAssetOfType(out DialogueCommandDatabase database))
            {
                Debug.Log("Start building command IDs");

                foreach (var dialogue in GetAssetsOfType<DialogueScriptableObject>())
                {
                    try
                    {
                        EditorUtility.SetDirty(dialogue);
                        dialogue.BuildDialogueCommandData(database);
                        AssetDatabase.SaveAssetIfDirty(dialogue);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to build command data for: {dialogue.name}");
                        throw;
                    }
                }
            
                AssetDatabase.Refresh();
            
                Debug.Log("Finish building command IDs");
            }
            else
            {
                Debug.LogError($"Failed to find a database of type {nameof(DialogueCommandDatabase)}");
            }
        }

        private static bool TryGetAssetOfType<T>(out T asset) where T : UnityEngine.Object
        {
            asset = GetAssetOfType<T>();
            return asset != null;
        }
        
        private static T GetAssetOfType<T>() where T : UnityEngine.Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:{typeof(T).Name}").FirstOrDefault()));
        }

        private static List<T> GetAssetsOfType<T>() where T : UnityEngine.Object
        {
            return AssetDatabase.FindAssets($"t:{typeof(T).Name}").ToList()
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .ToList();
        }
    }
}
