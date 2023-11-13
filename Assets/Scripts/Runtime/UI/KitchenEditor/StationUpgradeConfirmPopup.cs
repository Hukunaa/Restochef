using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Runtime.UI.KitchenEditor
{
    public class StationUpgradeConfirmPopup : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _name;

        [SerializeField]
        private TextMeshProUGUI _level;
        [SerializeField]
        private TextMeshProUGUI _levelNext;

        [SerializeField]
        private TextMeshProUGUI _primaryStat;
        [SerializeField]
        private TextMeshProUGUI _secondaryStat;
        [SerializeField]
        private TextMeshProUGUI _primaryStatNext;
        [SerializeField]
        private TextMeshProUGUI _secondaryStatNext;

        [SerializeField]
        private TextMeshProUGUI _cost;
        [SerializeField]
        private Button _upgradeButton;
        [SerializeField]
        private Button _cancelButton;

        public void ChangeName(string _newName)
        {
            _name.text = _newName;
        }
        public void ChangeLevel(int _newLevel, int _newNextLevel)
        {
            _level.text = "LEVEL " + _newLevel;
            _levelNext.text = "LEVEL " + _newNextLevel;
        }
        public void ChangeAccidentChanceStat(int _newValue, int _newNextValue)
        {
            _primaryStat.text = _newValue + "%";
            _primaryStatNext.text = _newNextValue + "%";
        }
        public void ChangeProcessingTimeStat(int _newValue, int _newNextValue)
        {
            _secondaryStat.text = "-" + _newValue + "%";
            _secondaryStatNext.text = "-" + _newNextValue + "%";
        }
        public void ChangeCost(int _newValue)
        {
            _cost.text = _newValue.ToString();
        }
    }
}
