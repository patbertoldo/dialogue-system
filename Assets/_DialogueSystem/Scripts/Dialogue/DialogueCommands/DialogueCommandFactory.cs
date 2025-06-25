using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Dialogue
{
    public static class DialogueCommandFactory
    {
        //public static 
        
        
        
        
        
        
        
        
        
        
        // private const string play = "play";
        // // Custom Markups
        // private const string show = "show";
        // private const string hide = "hide";
        // private const string shake = "shake";
        // private const string wait = "wait";
        // private const string speed = "speed";
        // private const string emotion = "emotion";
        
        private static Dictionary<string, Func<IDialogueCommand>> dialogueCommands = new()
        {
            
            // { play, DialogueCommand<float>.Create<PlayTextCommand> },
            // { show, DialogueCommand<float>.Create<ShowCommand> },
            // { hide, DialogueCommand<float>.Create<HideCommand> },
            // { shake, DialogueCommand<float>.Create<ShakeCommand> },
            // { wait, DialogueCommand<float>.Create<WaitCommand> },
            // { speed, DialogueCommand<float>.Create<SpeedCommand> },
            // { emotion, DialogueCommand<Emotions>.Create<EmotionCommand> },
        };
        
        public static List<IDialogueCommand> BuildCommands(string text)
        {
            // Create a command for every markup found. And a command for non-markup up text too!
            var commands = new List<IDialogueCommand>();

            StringBuilder markupBuilder = new();

            bool encounteredMarkup = false;
            
            // Loop through the characters to detect markup.
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
                   
                    // End index can never be negative.
                    if (charIndex > 0)
                        endIndex = charIndex - 1;
                    
                    // Did we parse through text that we can play?
                    if (startIndex < endIndex)
                    {
                        commands.Add(CreateDialogueCommandFromMarkup("<play>"));

                        startIndex = endIndex;
                    }
                    
                    continue;
                }

                if (encounteredMarkup)
                {
                    markupBuilder.Append(character);

                    if (character == '>')
                    {
                        encounteredMarkup = false;
                        
                        // Build command from the markup.
                        var newDialogueCommand = CreateDialogueCommandFromMarkup(markupBuilder.ToString());
                        
                        if (newDialogueCommand != null)
                            commands.Add(newDialogueCommand);
                        
                        // Set indexes
                        startIndex = charIndex;
                        
                        markupBuilder.Clear();
                    }
                }
            }

            return commands;
        }
        
        private static string GetMarkupStripped(string markup)
        {
            string strippedMarkup = markup.Contains('=')
                ? markup.Substring(1, markup.IndexOf('=') - 1)
                : markup.Substring(1, markup.IndexOf('>') - 1);
            return strippedMarkup;
        }

        private static T GetMarkupValue<T>(this T type, string markup) where T : IDialogueCommand
        {
            if (!markup.Contains('='))
            {
                return default;
            }
            
            int indexAfterEquals = markup.IndexOf('=') + 1;
            int length = markup.Length - 1 - indexAfterEquals;  
            string result = markup.Substring(indexAfterEquals, length);
            return (T)Convert.ChangeType(result, typeof(T));
        }

        private static IDialogueCommand CreateDialogueCommandFromMarkup(string markup)
        {
            string markupStripped = GetMarkupStripped(markup);

            if (!dialogueCommands.ContainsKey(markupStripped))
            {
                return null;
            }
            
            var newDialogueCommand = dialogueCommands[markupStripped].Invoke();
            //newDialogueCommand.Value = GetMarkupValue()
            //newDialogueCommand.Configure(GetMarkupValue(newDialogueCommand, markupStripped));
            
            Debug.Log($"Created {newDialogueCommand.GetType()}.");
            return newDialogueCommand;
        }
    }
}
