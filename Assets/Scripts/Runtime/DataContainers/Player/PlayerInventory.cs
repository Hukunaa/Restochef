using Runtime.DataContainers.Inventory;
using Runtime.Utility;
using System.Collections.Generic;

namespace Runtime.DataContainers.Player
{
    public class PlayerInventory
    {
        private List<InventoryItem> _items;

        public void LoadInventory()
        {
            //Need to gather list of items from server
            _items = DataLoader.LoadInventory();
        }

        public void SaveInventory()
        {
            DataLoader.SaveInventory(_items);
        }

        public void AddItem(InventoryItem _item)
        {
            _items.Add(_item);
        }
        public void RemoveItem(int _index)
        {
            _items.RemoveAt(_index);
        }

        public void RemoveItem(InventoryItem _item)
        {
            _items.Remove(_item);
        }
        
        public List<InventoryItem> Items { get => _items; }
    }
}
