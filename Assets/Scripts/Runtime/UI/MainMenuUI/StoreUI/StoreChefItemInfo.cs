using DG.Tweening;
using Runtime.Enums;
using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.EventChannels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.MainMenuUI.StoreUI
{
    public class StoreChefItemInfo : StoreItemInfo
    {
        [SerializeField] 
        private RectTransform _popup;
        
        [SerializeField] 
        private Image _cardBackground;
        
        [SerializeField] 
        private TMP_Text _chefNameText;
       
        [SerializeField] 
        private TMP_Text _chefLevelText;
        
        [SerializeField] 
        private SlicedFilledImage _chefProgressionBar;
        
        [SerializeField] 
        private TMP_Text _xpText;
        
        [SerializeField] 
        private TMP_Text _dexterityText;
        
        [SerializeField] 
        private TMP_Text _detailText;
        
        [SerializeField] 
        private TMP_Text _intuitionText;
        
        [Header("Asset References")]
        [SerializeField] 
        private RarityColors _rarityColors;
        
        [Header("Listening on")]
        [SerializeField] 
        private StoreChefItemEventChannel _onShowChefDataEventChannel;

        private StoreChefItem _currentChefItem;
        private float _popupYOffset;

        protected override void Awake()
        {
            base.Awake();
            _onShowChefDataEventChannel.onEventRaised += ShowChefInfo;
            _popupYOffset = _popup.anchoredPosition.y;
        }

        private void OnDestroy()
        {
            _onShowChefDataEventChannel.onEventRaised -= ShowChefInfo;
        }

        private void ShowChefInfo(StoreChefItem _chefItem)
        {
            _currentChefItem = _chefItem;
            InitialiseUI();
            ShowPopUp();
            _cardBackground.color = _rarityColors.Values[(int)_currentChefItem.ChefData.Rarity];
            _popup.DOAnchorPosY(-1000, 0.0f);
            _popup.DOAnchorPosY(_popupYOffset, 0.2f);
        }
        public override void HidePopUp()
        {
            _popup.DOAnchorPosY(-1000, 0.2f).OnComplete(() => base.HidePopUp());
        }

        private void InitialiseUI()
        {
            var chefData = _currentChefItem.ChefData;
            _chefNameText.text = chefData.ChefName;
            _chefLevelText.text = chefData.LevelData.Level.ToString();
            _chefProgressionBar.fillAmount = chefData.LevelData.LevelCompletionPercentage;
            _xpText.text = $"{chefData.LevelData.CurrentXP.ToString()}/{chefData.LevelData.CurrentLevelXPGoal.ToString()}";
            _dexterityText.text = chefData.Skills[0].Level.ToString();
            _detailText.text = chefData.Skills[1].Level.ToString();
            _intuitionText.text = chefData.Skills[2].Level.ToString();
            string priceText = _currentChefItem.Price == 0 ? FreeItemText : _currentChefItem.Price.ToString();
            _priceText.text = priceText;
            _purchaseButton.interactable = CanPay();
        }

        public override void OnBuyButtonClicked()
        {
            _currentChefItem.PurchaseItem();
            HidePopUp();
        }

        protected override bool CanPay()
        {
            return GameManager.Instance.PlayerDataContainer.Currencies.CanPay(ECurrencyType.SoftCurrency,
                _currentChefItem.Price);
        }
    }
}