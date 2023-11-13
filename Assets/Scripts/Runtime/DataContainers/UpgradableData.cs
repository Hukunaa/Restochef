using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.DataContainers
{
    [System.Serializable]
    public class UpgradableData
    {
        [SerializeField]
        private int _level;
        [SerializeField]
        private int _maxLevel;
        [SerializeField]
        private int _costToUpgrade;
        [SerializeField]
        private int _accidentChance;
        [SerializeField]
        private int _processTimeBonus;

        public UpgradableData(int _newLevel, int _newMaxLevel)
        {
            _level = _newLevel;
            _maxLevel = _newMaxLevel;
            _costToUpgrade = 999999999;
        }

        public int Level { get => _level; set => _level = value; }
        public int MaxLevel { get => _maxLevel; set => _maxLevel = value; }
        public int CostToUpgrade { get => _costToUpgrade; set => _costToUpgrade = value; }
        public int AccidentChance { get => _accidentChance; set => _accidentChance = value; }
        public int ProcessTimeBonus { get => _processTimeBonus; set => _processTimeBonus = value; }
    }
}
