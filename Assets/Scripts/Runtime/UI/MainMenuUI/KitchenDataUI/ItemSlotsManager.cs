using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.UI.MainMenuUI.KitchenDataUI
{

    public abstract class ItemSlotsManager : MonoBehaviour
    {
        [SerializeField] 
        protected List<ItemSlot> _slots;

        [SerializeField] private AssetReference _mandatorySlotAssetRef;
        [SerializeField] private AssetReference _optionalSlotAssetRef;

        protected GameObject _mandatorySlotPrefab;
        protected GameObject _optionalSlotPrefab;
        
        private AsyncOperationHandle<GameObject> _mandatorySlotHandle;
        private AsyncOperationHandle<GameObject> _optionalSlotHandle;

        protected PlayerDataContainer _playerDataContainer;
        
        [SerializeField] protected VoidEventChannel _slotsGeneratedEventChannel;
        
        private void Awake()
        {
            StartCoroutine(LoadAssetsAsync());
        }

        private IEnumerator LoadAssetsAsync()
        {
            _mandatorySlotHandle = _mandatorySlotAssetRef.LoadAssetAsync<GameObject>();
            _mandatorySlotHandle.Completed += _handle => _mandatorySlotPrefab = _handle.Result;
            
            _optionalSlotHandle = _optionalSlotAssetRef.LoadAssetAsync<GameObject>();
            _optionalSlotHandle.Completed += _handle => _optionalSlotPrefab = _handle.Result;

            while (!_mandatorySlotHandle.IsDone || !_optionalSlotHandle.IsDone)
            {
                yield return null;
            }

            StartCoroutine(Initialize());
        }

        private void OnDestroy()
        {
            Addressables.Release(_mandatorySlotHandle);
            Addressables.Release(_optionalSlotHandle);
        }

        private IEnumerator Initialize()
        {
            while (GameManager.Instance == null || !GameManager.Instance.DataLoaded)
            {
                yield return null;
            }

            _playerDataContainer = GameManager.Instance.PlayerDataContainer;

            ClearSlots();

            GenerateSlots();
        }

        protected abstract void GenerateSlots();

        private void ClearSlots()
        {
            for (int i = _slots.Count - 1; i >= 0; i--)
            {
                var slot = _slots[i];
                slot.ItemInSlot.RemoveItem();
                _slots.Remove(slot);
                Destroy(slot);
            }
        }
        
        public void RearrangeSlots(ItemSlot _slot)
        {
            RearrangeSlots(_slots.IndexOf(_slot));
        }

        public void RearrangeSlots(int index)
        {
            int nextSlot = index + 1;
            if (nextSlot > _slots.Count - 1)
                return;

            if(_slots[nextSlot].Filled)
            {
                _slots[index].AssignItem(_slots[nextSlot].ItemInSlot);
                _slots[nextSlot].ClearSlot();
                RearrangeSlots(nextSlot);
            }
        }

        public ItemSlot FindAvailableSlot()
        {
            return _slots.FirstOrDefault(_slot => !_slot.Filled);
        }

        public ItemSlot FindSlotWithItem(InventoryItemUI _item)
        {
            return _slots.FirstOrDefault(_slot => _slot.ItemInSlot == _item);
        }
    }
}