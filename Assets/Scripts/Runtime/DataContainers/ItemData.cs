using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.DataContainers
{
    [Serializable]
    public class ItemData 
    {
        //Add data structs here if needed
        [SerializeField]
        private UpgradableData _upgradableData;

        public ItemData(UpgradableData _upgradable)
        {
            _upgradableData = _upgradable;
        }
        public UpgradableData UpgradableData { get => _upgradableData; set => _upgradableData = value; }
    }
}
