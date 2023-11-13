using Runtime.DataContainers.Stats;
using Runtime.Managers.GameplayManager;
using Runtime.ScriptableObjects.DataContainers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class ChefXPItem : MonoBehaviour
    {
        [SerializeField] private SlicedFilledImage _progressBar;
        [SerializeField] private Image _chefPortrait;
        [SerializeField] private Image _cardBackground;
        [SerializeField] private TMP_Text _chefName;
        [SerializeField] private TMP_Text _chefLevel;
        [SerializeField] private TMP_Text _xpEarned;
        [SerializeField] private TMP_Text _levelUp;

        [Header("Asset References")]
        [SerializeField]
        private RarityColors _rarityColors;

        private int _levelBeforeXp;

        private ChefData _chef;
        
        public void Initialize(ChefData _chefData, ShiftRewardManager _shiftRewardManager)
        {
            _chef = _chefData;
            _chefPortrait.sprite = _chef.ChefSettings.ChefHeadPortrait;
            _cardBackground.color = _rarityColors.Values[(int)_chef.Rarity];
            _chefName.text = _chef.ChefSettings.ChefName;
            _xpEarned.text = $"+ {_shiftRewardManager.ChefXpEarned}";
            _levelBeforeXp = _chefData.LevelData.Level;
        }

        public void SetXpInfo()
        {
            _chefLevel.text = _chef.LevelData.Level.ToString();
            if (_chef.LevelData.Level > _levelBeforeXp)
            {
                _levelUp.gameObject.SetActive(true);
            }

            _progressBar.fillAmount = _chef.LevelData.LevelCompletionPercentage;
        }
    }
}