using System.Collections.Generic;
using Runtime.DataContainers.Stats;
using UnityEngine;
using System.Linq;

namespace Runtime.UI.MainMenuUI.KitchenDataUI
{
    public class BrigadeManager : InventoryItemUIManager
    {
        private ChefItemUI _chefItemUIPrefab;

        private List<ChefItemUI> _chefItems = new List<ChefItemUI>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _playerDataContainer.onChefAddedToCollection -= OnChefCollectionChanged;
        }

        protected override void OnPrefabLoaded(GameObject _prefab)
        {
            _chefItemUIPrefab = _prefab.GetComponent<ChefItemUI>();
        }

        protected override void CreateItems()
        {
            foreach (var chefData in _playerDataContainer.PlayerChefs)
            {
                CreateChefItem(chefData);
            }

            _playerDataContainer.onChefAddedToCollection += OnChefCollectionChanged;
            SortItems();
        }

        private void CreateChefItem(ChefData chefData)
        {
            var chefItemUI = Instantiate(_chefItemUIPrefab, _unusedItemContainer, false);
            chefItemUI.SetInventoryItemManager(this);
            chefItemUI.Initialize(chefData);
            _chefItems.Add(chefItemUI);
        }


        protected override void SetSavedConfig()
        {
            foreach (var chefItem in _chefItems)
            {
                if (_playerDataContainer.SelectedKitchenData.brigade.Contains(chefItem.ChefData.ChefID))
                {
                    chefItem.UseItem();
                }
            }
        }

        public override void OnItemAdded(InventoryItemUI _item)
        {
            var chefItem = (ChefItemUI)_item;
            _playerDataContainer.SelectedKitchenData.AddChefToBrigade(chefItem.ChefData);
            SortItems();
        }

        public override void OnItemRemoved(InventoryItemUI _item)
        {
            var chefItem = (ChefItemUI)_item;
            _playerDataContainer.SelectedKitchenData.RemoveChefFromBrigade(chefItem.ChefData);
            SortItems();
        }

        public void OnChefCollectionChanged(ChefData _chefData)
        {
            CreateChefItem(_chefData);
        }

        public void SortItems()
        {
            //Little hack to force copy the list and not update the current one as it would break the enumeration
            List<ChefItemUI> _array = _chefItems.ToArray().ToList();

            _array.Sort(Utility.SortingHelper.CompareByRarity);
            for (int i = 0; i < _chefItems.Count; ++i)
            {
                _array[i].transform.SetSiblingIndex(i);
            }
        }
    }
}