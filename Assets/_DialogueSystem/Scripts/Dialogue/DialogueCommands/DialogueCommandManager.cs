using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Dialogue
{
    public class DialogueCommandManager
    {

        
        //private DialogueCommandFactory dialogueCommandFactory;

        public DialogueCommandManager()
        {
            //dialogueCommandFactory = new DialogueCommandFactory();
        }

        public void BuildCommands(string text)
        {
            // Create a command for every markup found. And for normal text too!
            //commands.Clear();

            StringBuilder markupBuilder = new();
            
            bool encounteredMarkup = false;
            
            var charArray = text.ToCharArray();
            
            // Track the indexes for playing text.
            int startIndex = 0;
            int endIndex = 0;

            for (int charIndex = 0; charIndex < charArray.Length; charIndex++)
            {
                var character = charArray[charIndex];
                if (character == '<')
                {
                    encounteredMarkup = true;

                    markupBuilder.Append(character);
                    
                    if (charIndex > 0)
                        endIndex = charIndex - 1;
                    
                    continue;
                }

                if (encounteredMarkup)
                {
                    markupBuilder.Append(character);

                    if (character == '>')
                    {
                        encounteredMarkup = false;
                        
                        var markup = markupBuilder.ToString();
                        //var markupStripped = GetMarkupStripped(markup);
                        
                        // Build Command here
                        //commands.Add(CreateDialogueCommand(markup, markupStripped));
                        
                        // Set indexes
                        startIndex = charIndex;
                    }
                }
            }
        }

        private IDialogueCommand CreateDialogueCommand(string markup, string markupStripped)
        {
            
            // switch (markupStripped)
            // {
            //     case markupShow:
            //         return DialogueCommand.Create<WaitCommand>(GetMarkupValue<float>(markup));
            //         return new WaitCommand(GetMarkupValue<float>(markup));
            // }

            return null;
        }
        
        
        

    }
}