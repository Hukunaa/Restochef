using Runtime.DataContainers.Stats;
using Runtime.ScriptableObjects.EventChannels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Runtime.ScriptableObjects.DataContainers;

namespace Runtime.UI.MainMenuUI.KitchenDataUI
{
    public class ChefInfoPopUp : InfoPopUp
    {
        private ChefData _currentChef;

        [SerializeField] private RectTransform _popup;
        [SerializeField] private Image _cardBackground;
        [SerializeField] private TMP_Text _chefNameText;
        [SerializeField] private TMP_Text _chefLevelText;
        [SerializeField] private SlicedFilledImage _chefProgressionBar;
        [SerializeField] private TMP_Text _xpText;
        [SerializeField] private TMP_Text _dexterityText;
        [SerializeField] private TMP_Text _detailText;
        [SerializeField] private TMP_Text _intuitionText;
        
        [Header("Asset References")]
        [SerializeField] private RarityColors _rarityColors;
        
        [Header("Listening to")]
        [SerializeField] private ChefDataEventChannel _onShowChefDataEventChannel;

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

        private void ShowChefInfo(ChefData _chefData)
        {
            _currentChef = _chefData;
            InitialiseUI();
            ShowPopUp();
            _cardBackground.color = _rarityColors.Values[(int)_currentChef.Rarity];
            _popup.DOAnchorPosY(-1000, 0.0f);
            _popup.DOAnchorPosY(_popupYOffset, 0.2f);
        }
        public override void HidePopUp()
        {
            _popup.DOAnchorPosY(-1000, 0.2f).OnComplete(() => base.HidePopUp());
        }

        private void InitialiseUI()
        {
            _chefNameText.text = _currentChef.ChefName;
            _chefLevelText.text = _currentChef.LevelData.Level.ToString();
            _chefProgressionBar.fillAmount = _currentChef.LevelData.LevelCompletionPercentage;
            _xpText.text = $"{_currentChef.LevelData.CurrentXP.ToString()}/{_currentChef.LevelData.CurrentLevelXPGoal.ToString()}";
            _dexterityText.text = _currentChef.Skills[0].Level.ToString();
            _detailText.text = _currentChef.Skills[1].Level.ToString();
            _intuitionText.text = _currentChef.Skills[2].Level.ToString();
        }
    }
}