using Runtime.DataContainers.Stats;
using Runtime.ScriptableObjects.EventChannels;
using System.Collections.Generic;
using Runtime.ScriptableObjects.DataContainers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.MainMenuUI.KitchenDataUI
{
    public class ChefItemUI : InventoryItemUI
    {
        [SerializeField] private Image _chefPortrait;
        [SerializeField] private TMP_Text _chefLevelText;
        [SerializeField] private SlicedFilledImage _levelProgressBar;
        [SerializeField] private Button _chefButton;
        
        [Header("Asset References")]
        [SerializeField] private RarityColors _rarityColors;
        
        [Header("Broadcasting on")]
        [SerializeField] private ChefDataEventChannel _onShowChefInfoEventChannel;

        private ChefData _chefData;
        
        public void Initialize(ChefData _chefData)
        {
            this._chefData = _chefData;
            _chefPortrait.sprite = _chefData.ChefSettings.ChefHeadPortrait;
            _chefLevelText.text = _chefData.LevelData.Level.ToString();
            _levelProgressBar.fillAmount = _chefData.LevelData.LevelCompletionPercentage;

            if (_chefButton != null)
                _chefButton.GetComponent<Image>().color = _rarityColors.Values[(int)_chefData.Rarity];
        }
        
        public override void ShowItemInfo()
        {
            DeselectItem();
            _onShowChefInfoEventChannel.RaiseEvent(_chefData);
        }

        public ChefData ChefData => _chefData;
    }
}