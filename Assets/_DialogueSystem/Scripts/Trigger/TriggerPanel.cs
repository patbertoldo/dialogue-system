using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class TriggerPanel : Panel
    {
        [SerializeField] private GameObject triggerDialoguePrefab;
        [SerializeField] private Transform container;
        [SerializeField] private Button triggerButton;

        public void ShowTriggers(DialogueScriptableObjectAssetReference[] dialogueAssets, Action<DialogueScriptableObjectAssetReference> onClick)
        {
            Show();
            
            foreach (var dialogueAsset in dialogueAssets)
            {
                var newButton = Instantiate(triggerDialoguePrefab, container);
                newButton.GetComponent<TriggerButton>().Initiate(dialogueAsset, onClick);
            }
        }
    }
}
