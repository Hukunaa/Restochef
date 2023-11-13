using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.DataContainers;
using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace Runtime.Gameplay
{
    public class ChefSpawner : MonoBehaviour
    {
        [SerializeField]
        private AssetReferenceT<GameObject> _chefAssetRef;
        
        [SerializeField] 
        private KitchenLayoutManager _gridManager;
        
        [Header("Listening on")]
        [SerializeField] private VoidEventChannel _onKitchenLoaded;

        [Header("Broadcasting on")]
        [SerializeField] private VoidEventChannel _onChefSpawned;
        
        private PlayerDataContainer _playerDataContainer;
        private AsyncOperationHandle<GameObject> _chefOperationHandle;
        private GameObject _chefPrefab;

        private void Awake()
        {
            _onKitchenLoaded.onEventRaised += OnKitchenLoaded;
        }

        private void OnDestroy()
        {
            _onKitchenLoaded.onEventRaised -= OnKitchenLoaded;
        }

        private void OnKitchenLoaded()
        {
            _chefOperationHandle = _chefAssetRef.LoadAssetAsync<GameObject>();
            _chefOperationHandle.Completed += _handle =>
            {
                _chefPrefab = _handle.Result;
                StartCoroutine(SpawnChefs());
            };
        }
        
        IEnumerator SpawnChefs()
        {
            //Wait until the ShiftDataContainer is loaded. Sometimes the kitchen get loaded before the container so we must wait for the container to load.
            while (GameManager.Instance == null || !GameManager.Instance.PlayerDataContainer)
            {
                yield return null;
            }

            _playerDataContainer = GameManager.Instance.PlayerDataContainer;
            
            List<KitchenTile> spawnTiles = _gridManager.GetAllTilesOfType(TileType.WALKABLE).ToList();
            
            foreach (var chef in _playerDataContainer.SelectedKitchenData._brigadeChefs)
            {
                var spawnGridTile = GetRandomGridTile(spawnTiles.ToArray());
                spawnTiles.Remove(spawnGridTile);

                var instance = Instantiate(_chefPrefab, new Vector3(spawnGridTile.Position.x, 0, spawnGridTile.Position.y), Quaternion.LookRotation(Vector3.back)).GetComponent<Chef>();
                instance.InitializeChef(chef, spawnGridTile);
            }
            
            _onChefSpawned.RaiseEvent();
        }

        private KitchenTile GetRandomGridTile(KitchenTile[] _availableTiles)
        {
            return _availableTiles[Random.Range(0, _availableTiles.Length)];
        }
    }
}