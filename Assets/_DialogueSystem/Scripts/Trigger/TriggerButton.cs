using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class TriggerButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text buttonText;

        public void Initiate(DialogueScriptableObjectAssetReference dialogueAsset, Action<DialogueScriptableObjectAssetReference> onClick)
        {
            button.onClick.AddListener(() => onClick?.Invoke(dialogueAsset));
            #if UNITY_EDITOR
            buttonText.text = dialogueAsset.editorAsset.name;
            #endif
        }
    }
}
