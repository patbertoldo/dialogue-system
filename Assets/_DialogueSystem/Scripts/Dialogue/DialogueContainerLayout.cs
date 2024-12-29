using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueContainerLayout : MonoBehaviour
    {
        [SerializeField] private Image portrait;
        [SerializeField] private TMP_Text descriptionText;
        
        public Image Portrait => portrait;
        public TMP_Text DescriptionText => descriptionText;
    }
}
