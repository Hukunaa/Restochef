using System.Linq;
using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers;
using ScriptableObjects.DataContainers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.MainMenuUI.ProgressPath
{
    public class ChefRewardPopup : RewardPopup
    {
        [SerializeField] private TMP_Text _chefNameText;
        [SerializeField] private Image _chefPortraitImage;
        [SerializeField] private TMP_Text _chefDexterityStatText;
        [SerializeField] private TMP_Text _chefDetailStatText;
        [SerializeField] private TMP_Text _chefIntuitionStatText;
        
        [SerializeField] 
        private RarityColors _rarityColors;
        
        public override void InitializePopup(RewardItem _rewardItem)
        {
            var chef = GameManager.Instance.PlayerDataContainer.ChefRewardData.FirstOrDefault(x =>
                x.ChefID == _rewardItem.ChefID);

            if (chef == null)
            {
                Debug.LogWarning($"No chef with the ID {_rewardItem.ChefID} could be found in the Chef Rewards.");
                return;
            }

            _chefNameText.text = chef.ChefName;
            _chefPortraitImage.sprite = chef.ChefSettings.ChefHeadPortrait;
            _chefDexterityStatText.text = chef.Skills[0].Level.ToString();
            _chefDetailStatText.text = chef.Skills[1].Level.ToString();
            _chefIntuitionStatText.text = chef.Skills[2].Level.ToString();
            RarityColor = _rarityColors.Values[(int)chef.Rarity];
        }
        
        public Color RarityColor { get; private set; }
    }
}