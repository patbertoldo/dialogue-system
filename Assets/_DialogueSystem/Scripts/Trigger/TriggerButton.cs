using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class TriggerButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text buttonText;

        public void Initiate(string text, Action<string> onClick)
        {
            button.onClick.AddListener(() => onClick?.Invoke(text));
            buttonText.text = text;
        }
    }
}
