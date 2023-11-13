using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runtime.DataContainers;
using System;
using Runtime.Managers;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.Utility;

namespace Runtime.Gameplay
{
    [Serializable]
    public class Upgradable
    {
        [SerializeField]
        private UpgradableData _data;

        public event Action OnUpgrade;

        public void InitUpgradable()
        {
            StationSlotStatEntry _table = DataLoader.LoadStationStat(Data.Level);
            StationSlotStatEntry _tableNextLevel = DataLoader.LoadStationStat(Data.Level + 1);
            _data.AccidentChance = _table.accident_bonus;
            _data.ProcessTimeBonus = _table.processing_time_bonus;
            _data.CostToUpgrade = _tableNextLevel.cost;
        }
        public bool CanUpgrade(int _payment)
        {
            StationSlotStatEntry _tableNextLevel = DataLoader.LoadStationStat(Data.Level + 1);
            if (_payment < _tableNextLevel.cost || _data.Level >= _data.MaxLevel)
                return false;
            else
                return true;
        }

        public void Upgrade()
        {
            _data.Level += 1;
            _data.MaxLevel = 10;
            StationSlotStatEntry _table = DataLoader.LoadStationStat(Data.Level);
            StationSlotStatEntry _tableNextLevel = DataLoader.LoadStationStat(Data.Level + 1);
            _data.AccidentChance = _table.accident_bonus;
            _data.ProcessTimeBonus = _table.processing_time_bonus;
            _data.CostToUpgrade = _tableNextLevel.cost;
            OnUpgrade?.Invoke();
        }

        public bool IsMaxLevel { get => _data.Level >= _data.MaxLevel; }
        public UpgradableData Data { get => _data; }
    }
}

