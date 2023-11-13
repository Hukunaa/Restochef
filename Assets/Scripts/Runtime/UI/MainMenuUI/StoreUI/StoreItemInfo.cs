using Runtime.UI.MainMenuUI.KitchenDataUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.MainMenuUI.StoreUI
{
    public abstract class StoreItemInfo : InfoPopUp
    {
        [SerializeField] 
        protected TMP_Text _priceText;
        
        [SerializeField] 
        protected Button _purchaseButton;
        
        [SerializeField] 
        private string _freeItemText = "Free";
        public abstract void OnBuyButtonClicked();

        public string FreeItemText => _freeItemText;
        
        protected abstract bool CanPay();
    }
}