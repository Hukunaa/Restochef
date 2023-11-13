using System.Collections;
using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.UI.MainMenuUI.KitchenDataUI
{
    public abstract class InventoryItemUIManager : MonoBehaviour
    {
        [SerializeField] protected AssetReferenceT<GameObject> _chefItemUIAssetRef;
        protected AsyncOperationHandle<GameObject> _handle;
        
        private InventoryItemUI _selectedItem;

        private InventoryItemUI[] _usedItem;

        [SerializeField] protected ItemSlotsManager _itemSlotsManager;
        [SerializeField] protected Transform _unusedItemContainer;

        [SerializeField] protected VoidEventChannel _slotGeneratedEventChannel;
        
        protected PlayerDataContainer _playerDataContainer;

        [Header("Broadcasting on")]
        [SerializeField] private VoidEventChannel _onTabInitializedEventChannel;
        
        private void Awake()
        {
            _slotGeneratedEventChannel.onEventRaised += Initialize;
        }

        private void Initialize()
        {
            _handle = _chefItemUIAssetRef.LoadAssetAsync<GameObject>();
            _handle.Completed += handle =>
            {
                OnPrefabLoaded(handle.Result);
                StartCoroutine(InitializeCoroutine());
            };
        }

        private void OnDisable()
        {
            if (_selectedItem != null)
            {
                _selectedItem.DeselectItem();
            }
        }

        protected abstract void OnPrefabLoaded(GameObject _prefab);

        private IEnumerator InitializeCoroutine()
        {
            while (GameManager.Instance == null || GameManager.Instance.DataLoaded == false)
            {
                yield return null;
            }

            _playerDataContainer = GameManager.Instance.PlayerDataContainer;

            CreateItems();
            
            yield return null;
            
            SetSavedConfig();
            
            _onTabInitializedEventChannel.RaiseEvent();
        }

        protected abstract void CreateItems();

        protected abstract void SetSavedConfig();

        
        public void UpdateSelectedItem(InventoryItemUI _item)
        {
            if (_selectedItem != null)
            {
                _selectedItem.DeselectItem();
            }

            _selectedItem = _item;
        }

        public bool UseItem(InventoryItemUI _item)
        {
            var availableSlots = _itemSlotsManager.FindAvailableSlot();
            if (availableSlots == null) return false;
            
            availableSlots.AssignItem(_item);
            OnItemAdded(_item);
            return true;
        }

        public bool RemoveItem(InventoryItemUI _item)
        {
            var slot = _itemSlotsManager.FindSlotWithItem(_item);

            if (slot == null) return false;
            slot.ClearSlot();

            _item.transform.SetParent(_unusedItemContainer, false);
            OnItemRemoved(_item);
            _itemSlotsManager.RearrangeSlots(slot);
            return true;
        }
        
        protected virtual void OnDestroy()
        {
            Addressables.Release(_handle);
            _slotGeneratedEventChannel.onEventRaised -= Initialize;
        }

        public abstract void OnItemAdded(InventoryItemUI _item);
        public abstract void OnItemRemoved(InventoryItemUI _item);
    }
}