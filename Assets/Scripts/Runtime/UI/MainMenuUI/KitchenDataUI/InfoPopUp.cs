using System;
using UnityEngine;

namespace Runtime.UI.MainMenuUI.KitchenDataUI
{
    public class InfoPopUp : MonoBehaviour
    {
        [Header("Components ref")]
        [SerializeField]
        private Canvas _popUpCanvas;
        
        [SerializeField] 
        private CanvasGroup _canvasGroup;

        protected virtual void Awake()
        {
            _popUpCanvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            HidePopUp();
        }

        protected virtual void ShowPopUp()
        {
            _popUpCanvas.enabled = true;
            _canvasGroup.interactable = true;
        }

        public virtual void HidePopUp()
        {
            _popUpCanvas.enabled = false;
            _canvasGroup.interactable = false;
        }
    }
}