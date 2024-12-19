using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class TriggerManager
    {
        private DialogueScriptableObject[] dialogues;
        private TriggerPanel triggerPanel;

        public TriggerManager(DialogueScriptableObject[] dialogues, TriggerPanel triggerPanel)
        {
            this.dialogues = dialogues;
            this.triggerPanel = triggerPanel;
        }

        public void ShowTriggers(Action<string> onClick)
        {
            triggerPanel.ShowTriggers(dialogues, onClick);
        }
    }
}
