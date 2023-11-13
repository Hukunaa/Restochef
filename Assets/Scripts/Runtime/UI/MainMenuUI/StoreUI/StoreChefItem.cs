using System;
using Runtime.DataContainers.Stats;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.EventChannels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.MainMenuUI.StoreUI
{
    public class StoreChefItem : StoreItem
    {
        [SerializeField] 
        private Image _chefPortrait;
        
        [SerializeField] 
        private TMP_Text _chefLevelText;
       
        [SerializeField] 
        private SlicedFilledImage _levelProgressBar;
        
        [SerializeField] 
        private Button _chefButton;
        
        [Header("Asset References")]
        [SerializeField] 
        private RarityColors _rarityColors;
       
        [Header("Broadcasting on")]
        [SerializeField] 
        private StoreChefItemEventChannel _onShowChefInfoEventChannel;

        public event Action<StoreChefItem> OnPurchase;
        
        private ChefData _chefData;
        
        public void Initialize(ChefData _chefData, int _price)
        {
            this._chefData = _chefData;
            this._price = _price;
            _chefPortrait.sprite = _chefData.ChefSettings.ChefHeadPortrait;
            _chefLevelText.text = _chefData.LevelData.Level.ToString();
            _levelProgressBar.fillAmount = _chefData.LevelData.LevelCompletionPercentage;

            if (_chefButton != null)
                _chefButton.GetComponent<Image>().color = _rarityColors.Values[(int)_chefData.Rarity];
        }

        public override void PurchaseItem()
        {
            OnPurchase?.Invoke(this);
        }

        public override void ShowItemInfo()
        {
            DeselectItem();
            _onShowChefInfoEventChannel.RaiseEvent(this);
        }

        public ChefData ChefData => _chefData;
    }
}