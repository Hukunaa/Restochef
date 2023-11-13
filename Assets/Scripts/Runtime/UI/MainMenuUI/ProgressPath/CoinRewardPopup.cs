using ScriptableObjects.DataContainers;
using TMPro;
using UnityEngine;

namespace Runtime.UI.MainMenuUI.ProgressPath
{
    public class CoinRewardPopup : RewardPopup
    {
        [SerializeField] 
        private TMP_Text _coinsEarnedText;
        
        public override void InitializePopup(RewardItem _rewardItem)
        {
            _coinsEarnedText.text = _rewardItem.CoinsReward.ToString();
        }
    }
}