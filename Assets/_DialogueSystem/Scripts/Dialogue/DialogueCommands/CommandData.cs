using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dialogue
{
    [Serializable]
    public struct CommandData
    {
        public string Name;
        public string Value;

        public CommandData(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
