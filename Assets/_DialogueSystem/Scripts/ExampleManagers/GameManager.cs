using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    /// <summary>
    /// The GameManager holds onto references,
    /// super simple DI that creates managers and injects their dependencies,
    /// and manages global actions.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TriggerPanel triggerPanel;
        [SerializeField] private DialoguePanel dialoguePanel;
        
        [SerializeField] private DialogueScriptableObject[] dialogues;
        
        private TriggerManager triggerManager;
        private DialogueManager dialogueManager;
        
        private void Awake()
        {
            // Managers
            triggerManager = new TriggerManager(dialogues, triggerPanel);
            dialogueManager = new DialogueManager(dialogues, dialoguePanel);
            
            // Actions
            triggerManager.ShowTriggers(OnTriggerDialogueByName);
        }

        #region Actions

        private void OnTriggerDialogueByName(string dialogueName)
        {
            dialogueManager.TriggerDialogue(dialogueName);
        }
        
        #endregion
    }
}
