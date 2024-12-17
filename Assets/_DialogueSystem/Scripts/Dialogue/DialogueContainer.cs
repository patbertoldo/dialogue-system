using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueContainer : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image portrait;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        
        #region Animation Events

        public void FlyInComplete()
        {
            
        }
        
        public void FlyOutComplete()
        {
            
        }
        
        public void FocusComplete()
        {
            
        }
        
        public void UnfocusComplete()
        {
            
        }
        
        #endregion Animation Events
    }
}
