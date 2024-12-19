using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public enum DialogueState
    {
        NONE,
        IN_PROGRESS,    // When text is in an animating state
        FINISHED        // When text has finished animating.
    }
    
    public class DialogueManager
    {
        private DialogueScriptableObject[] dialogues;
        private DialoguePanel dialoguePanel;
        
        private DialogueScriptableObject currentDialogue;
        private DialogueState currentState;
        private int currentIndex;

        public DialogueManager(DialogueScriptableObject[] dialogues, DialoguePanel dialoguePanel)
        {
            this.dialogues = dialogues;
            this.dialoguePanel = dialoguePanel;

            currentDialogue = null;
            currentState = DialogueState.NONE;
            currentIndex = 0;
            
            dialoguePanel.SetContinueEvent(ContinueDialogue);
        }

        public void TriggerDialogue(string dialogueName)
        {
            Debug.Log($"Trigger: {dialogueName}");
            
            currentDialogue = GetDialogueByName(dialogueName);
            currentState = DialogueState.IN_PROGRESS;
            
            dialoguePanel.ShowDialogue(currentDialogue.DialogueBlocks[currentIndex]);
            
            // Mock finished state for now.
            currentState = DialogueState.FINISHED;
        }

        private void ContinueDialogue()
        {
            // Continue to the next dialogue block if the current index is finished.
            if (currentState == DialogueState.FINISHED)
            {
                // Continue if there is another index.
                if (currentIndex < currentDialogue.DialogueBlocks.Length - 1)
                {
                    currentState = DialogueState.IN_PROGRESS;
                    currentIndex++;
            
                    dialoguePanel.ShowDialogue(currentDialogue.DialogueBlocks[currentIndex]);
                }
                // Finished.
                else
                {
                    dialoguePanel.Hide();

                    ResetDialogue();
                }
            }
            // Skip progress and finish current dialogue.
            else
            {
                currentState = DialogueState.FINISHED;

                dialoguePanel.Skip();
            }
        }

        private DialogueScriptableObject GetDialogueByName(string dialogueName)
        {
            foreach (var dialogue in dialogues)
            {
                if (dialogue.name == dialogueName)
                    return dialogue;
            }
            
            Debug.LogError($"Failed to load: {dialogueName}");
            return null;
        }

        private void ResetDialogue()
        {
            currentDialogue = null;
            currentState = DialogueState.NONE;
            currentIndex = 0;
        }
    }
}