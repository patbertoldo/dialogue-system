using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dialogue
{
    /// <summary>
    /// The TestDialogueManager holds onto references,
    /// super simple DI that creates managers and injects their dependencies,
    /// and manages global actions.
    /// </summary>
    public class TestDialogueManager : MonoBehaviour
    {
        [SerializeField] private TriggerPanel triggerPanel;
        [SerializeField] private DialoguePanel dialoguePanel;
        
        [SerializeField] private DialogueScriptableObjectAssetReference[] dialogueAddressables;
        
        private TriggerManager triggerManager;
        private DialogueManager dialogueManager;
        
        private void Awake()
        {
            // Managers
            triggerManager = new TriggerManager(dialogueAddressables, triggerPanel);
            dialogueManager = new DialogueManager(dialoguePanel);
            
            // Actions
            triggerManager.ShowTriggers(OnTriggerDialogueByName);
        }

        private void OnDestroy()
        {
            dialogueManager.CleanUp();
        }

        #region Actions

        public void OnTriggerDialogueByName(DialogueScriptableObjectAssetReference dialogueAddressable)
        {
            dialogueManager.OpenDialogue(dialogueAddressable);
        }
        
        #endregion
    }
}
