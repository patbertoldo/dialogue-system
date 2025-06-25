using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

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
        
        [SerializeField] private DialogueDatabase dialogueDatabase;
        [SerializeField] private DialogueCommandDatabase dialogueCommandDatabase;
        
        private TriggerManager triggerManager;
        private DialogueManager dialogueManager;
        
        private void Awake()
        {
            // Load databases
            dialogueCommandDatabase.LoadAsync().Forget();
            
            // Leave loading the dialogue scriptables to load individually when then player selects them.
            //dialogueDatabase.LoadAsync();
            
            // Managers
            triggerManager = new TriggerManager(dialogueDatabase.AddressableDatabase, triggerPanel);
            dialogueManager = new DialogueManager(dialoguePanel, dialogueCommandDatabase);
            
            // Actions
            triggerManager.ShowTriggers(OnTriggerDialogueByName);
        }

        #region Actions

        public void OnTriggerDialogueByName(DialogueScriptableObjectAssetReference dialogueAddressable)
        {
            dialogueManager.LoadDialogue(dialogueAddressable);
        }
        
        #endregion
    }
}
