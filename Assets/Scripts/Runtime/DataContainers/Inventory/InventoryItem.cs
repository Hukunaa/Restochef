using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.DataContainers.Inventory
{
    [Serializable]
    public enum ITEM_TYPE
    {
        STORAGE,
        STATION,
        TABLE,
        CHEF,
        RECIPE
    }

    [Serializable]
    public class InventoryItem
    {
        [SerializeField]
        private string _itemName;
        [SerializeField]
        private ITEM_TYPE _itemType;
        [SerializeField]
        private int _id;
        [SerializeField]
        ItemData _itemData;

        public InventoryItem(int _itemID, string _name, ITEM_TYPE _type, ItemData _data)
        {
            _itemName = _name;
            _itemType = _type;
            _id = _itemID;
            _itemData = _data;
        }

        public ITEM_TYPE ItemType { get => _itemType; }
        public string ItemName { get => _itemName; }
        public int Id { get => _id; }
        public ItemData ItemData { get => _itemData; }
    }
}
