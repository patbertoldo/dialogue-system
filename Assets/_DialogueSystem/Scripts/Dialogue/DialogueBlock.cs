using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Dialogue
{
    public enum DialogueAlignment
    {
        LEFT,
        RIGHT
    }
    
    [Serializable]
    public class DialogueBlock
    {
        public DialogueAlignment Alignment;
        public Emotions Emotion;
        public DialogueCharacterScriptableObject DialogueCharacter;
        [TextArea(3, 10)]
        public string Description;
        [Tooltip("The speed at which the text appears, in milliseconds. 20 is roughly normal speed. Lower is faster.")]
        [Range(1, 200)]
        public int TextSpeed = 20;
        public List<CommandData> CommandDatas = new();
        [TextArea(3, 10)]
        public string DescriptionSanitized;

        public void BuildCommandData(DialogueCommandDatabase commandDatabase)
        {
            // Create command data for every part of the Description text.
            CommandDatas.Clear();

            // Handles markups
            StringBuilder markupBuilder = new();
            // Builds the final description without the custom markup. It should still include other markups such as italics <i> </i>.
            StringBuilder sanitizedBuilder = new();

            bool encounteredMarkup = false;
            
            // Loop through the characters to detect markup.
            var charArray = Description.ToCharArray();

            int indexBeforeMarkup = -1;
            int lastCustomMarkupEndIndex = 0;

            for (int charIndex = 0; charIndex < charArray.Length; charIndex++)
            {
                var character = charArray[charIndex];
                
                sanitizedBuilder.Append(character);
                
                if (character == '<')
                {
                    encounteredMarkup = true;

                    markupBuilder.Append(character);

                    indexBeforeMarkup = charIndex - 1;
                    
                    continue;
                }

                if (encounteredMarkup)
                {
                    markupBuilder.Append(character);

                    if (character == '>')
                    {
                        encounteredMarkup = false;
                        
                        // Prepare markup.
                        var markupBuilderString = markupBuilder.ToString();
                        var markupStripped = GetMarkupStripped(markupBuilderString);
                        var markupValue = GetMarkupValue(markupBuilderString);

                        if (commandDatabase.HasAddressableOfName(markupStripped))
                        //if (DialogueManager.GetCustomMarkupNames().Contains(markupStripped))
                        {   
                            lastCustomMarkupEndIndex = charIndex;

                            // Clear the custom markup from the sanitized builder.
                            sanitizedBuilder.Remove(sanitizedBuilder.Length - markupBuilder.Length, markupBuilder.Length);
                            
                            // If the index before the markup wasn't the end of another markup, we have text to play.
                            if (indexBeforeMarkup != -1 && charArray[indexBeforeMarkup] != '>')
                            {
                                AddCommandData("Default", (sanitizedBuilder.Length - 1).ToString());
                            }

                            AddCommandData(markupStripped, markupValue);
                        }
                        
                        markupBuilder.Clear();
                    }
                }
            }
            
            // If there were no markups or
            // If the last character wasn't a markup.
            if (CommandDatas.Count == 0 || lastCustomMarkupEndIndex < charArray.Length - 1)
                AddCommandData("Default", (sanitizedBuilder.Length - 1).ToString());

            DescriptionSanitized = sanitizedBuilder.ToString();
        }

        private void AddCommandData(string name, string value)
        {
            Debug.Log($"Added CommandData: [{name},{value}]");
            
            CommandDatas.Add(new CommandData(name, value));
        }
        
        private string GetMarkupStripped(string markup)
        {
            string strippedMarkup = markup.Contains('=')
                ? markup.Substring(1, markup.IndexOf('=') - 1)
                : markup.Substring(1, markup.IndexOf('>') - 1);
            return strippedMarkup;
        }
        
        private string GetMarkupValue(string markup)
        {
            if (!markup.Contains('='))
                return "";
            
            int indexAfterEquals = markup.IndexOf('=') + 1;
            int length = markup.Length - 1 - indexAfterEquals;  
            string result = markup.Substring(indexAfterEquals, length);
            return result;
        }
    }
}