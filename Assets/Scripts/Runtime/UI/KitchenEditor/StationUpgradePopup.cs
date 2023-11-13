using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Runtime.UI.KitchenEditor
{
    public class StationUpgradePopup : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _name;
        [SerializeField]
        private TextMeshProUGUI _level;
        [SerializeField]
        private TextMeshProUGUI _description;
        [SerializeField]
        private TextMeshProUGUI _primaryStat;
        [SerializeField]
        private TextMeshProUGUI _secondaryStat;
        [SerializeField]
        private TextMeshProUGUI _cost;
        [SerializeField]
        private Button _upgradeButton;

        public void ChangeName(string _newName)
        {
            if (_name != null)
                _name.text = _newName;
        }
        public void ChangeLevel(string _newLevel)
        {
            if (_level != null)
                _level.text = "LEVEL " + _newLevel;
        }
        public void ChangeDescription(string _newDescription)
        {
            if(_description != null)
                _description.text = _newDescription;
        }
        public void ChangeAccidentChanceStat(string _newValue)
        {
            if (_primaryStat != null)
                _primaryStat.text = _newValue + "%";
        }
        public void ChangeProcessingTimeStat(string _newValue)
        {
            if (_secondaryStat != null)
                _secondaryStat.text = "-" + _newValue + "%";
        }
        public void ChangeCost(string _newValue)
        {
            if (_cost != null)
                _cost.text = _newValue;
        }

        public void ToggleButton(bool _value)
        {
            if(_upgradeButton != null)
                _upgradeButton.interactable = _value;
        }
    }
}
