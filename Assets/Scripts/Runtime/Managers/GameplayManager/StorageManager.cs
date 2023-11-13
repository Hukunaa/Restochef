using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Gameplay;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.Managers.GameplayManager
{
    public class StorageManager : MonoBehaviour
    {
        private Dictionary<StorageType, List<Storage>> _storages = new Dictionary<StorageType, List<Storage>>();
        public Dictionary<StorageType, List<Storage>> Storages => _storages;

        [Header("Listening to")]
        [SerializeField] 
        private VoidEventChannel _onSceneGenerated;
        
        [Header("Broadcasting on")]
        [SerializeField] 
        private VoidEventChannel _onStoragesInitialized;
        
        [SerializeField]
        private bool _logDebugMessage;

        private PlayerDataContainer _playerDataContainer;

        private void Awake()
        {
            _onSceneGenerated.onEventRaised += InitializeStorages;
        }
        
        private void OnDestroy()
        {
            _onSceneGenerated.onEventRaised -= InitializeStorages;
        }
        
        private void InitializeStorages()
        {
            StartCoroutine(InitializeStoragesCoroutine());
        }

        private IEnumerator InitializeStoragesCoroutine()
        {
            while (GameManager.Instance == null || !GameManager.Instance.DataLoaded)
            {
                yield return null;
            }

            _playerDataContainer = GameManager.Instance.PlayerDataContainer; 
            
            GetSceneStorages();
            
            DispatchIngredients();
            _onStoragesInitialized.RaiseEvent();
        }
        
        private void GetSceneStorages()
        {
            var storages = FindObjectsOfType<Storage>();

            foreach (var storage in storages)
            {
                if (!_storages.ContainsKey(storage.StorageType))
                {
                    _storages[storage.StorageType] = new List<Storage>();
                }
                _storages[storage.StorageType].Add(storage);
            }
        }
        
        private void DispatchIngredients()
        {
            foreach (var requiredIngredient in _playerDataContainer.SelectedKitchenData.RawIngredients)
            {
                if (_storages.All(_s => _s.Key != requiredIngredient.StorageType))
                {
                     DebugHelper.PrintDebugMessage($"No storage of type {requiredIngredient.StorageType}. Can't dispatch ingredient", _logDebugMessage);
                     continue;
                }
                var storages = _storages[requiredIngredient.StorageType].ToArray();
                foreach (var storage in storages)
                {
                    DebugHelper.PrintDebugMessage($"Stored {requiredIngredient.name} in {storage.StorageName}", _logDebugMessage);
                    storage.StoreIngredient(requiredIngredient);
                }
            }
        }

        private Storage[] GetStoragesOfType(StorageType _storageType)
        {
            if (!_storages.ContainsKey(_storageType) || _storages[_storageType].Count == 0)
            {
                DebugHelper.PrintDebugMessage($"Storage Manager has no entry for storage of type {_storageType.name}", _logDebugMessage);
                return null;
            }
            
            return _storages[_storageType].ToArray();
        }

        public Storage GetClosestStorage(StorageType _storageType, Vector3 pos)
        {
            var storages = GetStoragesOfType(_storageType);

            if (storages == null) return null;
            
            if (storages.Length == 1)
            {
                return storages[0];
            }

            var orderedStorages = storages.OrderBy(s => (s.transform.position - pos).sqrMagnitude).ToArray();
            
            return orderedStorages[0];
        }
    }
}