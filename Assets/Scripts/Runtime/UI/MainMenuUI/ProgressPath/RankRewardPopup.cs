using ScriptableObjects.DataContainers;
using TMPro;
using UnityEngine;

namespace Runtime.UI.MainMenuUI.ProgressPath
{
    public class RankRewardPopup : RewardPopup
    {
        [SerializeField]
        private TMP_Text _maxStations;
        [SerializeField]
        private TMP_Text _maxStationsLevel;
        [SerializeField]
        private TMP_Text _maxChefs;
        [SerializeField]
        private TMP_Text _menuSize;
        [SerializeField]
        private TMP_Text _bonusPoints;
    
        public override void InitializePopup(RewardItem _rewardItem)
        {
            RewardHeaderText = _rewardItem.Title;
            _maxStations.text = _rewardItem.KitchenRankReward.MaxStations.ToString();
            _maxStationsLevel.text = _rewardItem.KitchenRankReward.MaxStationsLVL.ToString();
            _maxChefs.text = _rewardItem.KitchenRankReward.MaxChefs.ToString();
            _menuSize.text = _rewardItem.KitchenRankReward.MenuSize.ToString();
            _bonusPoints.text = _rewardItem.KitchenRankReward.BonusPoints.ToString() + "%";
        }
    }
}
