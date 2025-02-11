using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue", order = 0)]
    public class DialogueScriptableObject : ScriptableObject
    {
        [SerializeField] private DialogueBlock[] dialogueBlocks;

        public DialogueBlock[] DialogueBlocks => dialogueBlocks;

        public DialogueBlock GetFirstInstanceOfAlignment(DialogueAlignment alignment)
        {
            foreach (var dialogueBlock in dialogueBlocks)
            {
                if (dialogueBlock.Alignment == alignment)
                {
                    return dialogueBlock;
                }
            }

            return null;
        }
    }
}