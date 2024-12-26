using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class TriggerManager
    {
        private DialogueScriptableObjectAssetReference[] dialogueAddressables;
        private TriggerPanel triggerPanel;

        public TriggerManager(DialogueScriptableObjectAssetReference[] dialogueAddressables, TriggerPanel triggerPanel)
        {
            this.dialogueAddressables = dialogueAddressables;
            this.triggerPanel = triggerPanel;
        }

        public void ShowTriggers(Action<DialogueScriptableObjectAssetReference> onClick)
        {
            triggerPanel.ShowTriggers(dialogueAddressables, onClick);
        }
    }
}
