using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Runtime.DataContainers;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Runtime.DataContainers.Player;

namespace Runtime.Managers
{
    [RequireComponent(typeof(WallBuilder))]
    public class KitchenLoader : MonoBehaviour
    {
        private WallBuilder _wallBuilder;
        private GameObject _kitchenParent;
        private PlayerDataContainer _playerDataContainer;
        private KitchenData _kitchenData;
        public NavBaker _navBaker;
        private List<GameObject> _instantiatedObjects;
        private List<EntityType> _instantiatedEntityTypes;
        private List<AsyncOperationHandle<GameObject>> _handles;
        public event Action OnKitchenUpdated;

        [SerializeField] private VoidEventChannel _onKitchenLoaded;
        [SerializeField] private List<AssetReferenceT<GameObject>> _addressablesPrefabs;
        [SerializeField] private bool _logDebugMessage = false;

        private int _stationsCount;
        private int _numberOfAssetsLoaded;

        private List<EntityType> _prefabs = new List<EntityType>();

        private void Start()
        {
            _handles = new List<AsyncOperationHandle<GameObject>>();
            _numberOfAssetsLoaded = 0;
            _stationsCount = 0;
            _instantiatedObjects = new List<GameObject>();
            _instantiatedEntityTypes = new List<EntityType>();
            _wallBuilder = GetComponent<WallBuilder>();
            _addressablesPrefabs.ForEach(r => 
            {
                AsyncOperationHandle<GameObject> handle = r.LoadAssetAsync<GameObject>();
                handle.Completed += OnAssetLoaded;
                _handles.Add(handle);
            });

            StartCoroutine(WaitForDataToLoad());
        }

        private void OnAssetLoaded(AsyncOperationHandle<GameObject> _object)
        {
            var loadedAsset = _object.Result.GetComponent<EntityType>();
            if (loadedAsset == null)
            {
                Debug.LogWarning("LoadedAssetDoesn't have a EntityType component");
            }
            else
            {
                _prefabs.Add(_object.Result.GetComponent<EntityType>());
            }
            _numberOfAssetsLoaded++;
            _object.Completed -= OnAssetLoaded;
        }

        private IEnumerator WaitForDataToLoad()
        {
            while(GameManager.Instance == null || !GameManager.Instance.DataLoaded)
            {
                yield return null;
            }
        
            _playerDataContainer = GameManager.Instance.PlayerDataContainer;
            _kitchenData = _playerDataContainer.SelectedKitchenData;
            KitchenLayoutManager.Instance.LoadTiles();
            LoadKitchen();
        }

        private void LoadKitchen()
        {
            StartCoroutine(LoadKitchenCoroutine());
        }

        private void OnDestroy()
        {
            _handles.ForEach(o => Addressables.Release(o));
        }

        private IEnumerator LoadKitchenCoroutine()
        {
            while (SceneManager.GetActiveScene().name == "ManagerScene")
            {
                yield return null;
            }
            
            while(_numberOfAssetsLoaded < _addressablesPrefabs.Count)
            {
                yield return null;
            }
            
            _kitchenParent = new GameObject("Kitchen");

            for (int x = 0; x < _kitchenData.kitchenSizeX; ++x)
            {
                for (int y = 0; y < _kitchenData.kitchenSizeY; ++y)
                {
                    CreateEntity(_kitchenData.tiles[x, y]);
                }
            }

            if(_navBaker != null)
            {
                _navBaker.BakeSurface();
            }
            else
            {
                LogDebugMessage("No surface to bake for nav mesh, please input a surface!");
            }

            _wallBuilder.GenerateWalls();
            CheckForKitchenUpgraded();
            _onKitchenLoaded.RaiseEvent();
        }

        public void SetEntityTo(Vector2Int _pos, string _entityType)
        {
            KitchenTile _tile = KitchenLayoutManager.Instance.GetTileData(_pos.x, _pos.y);
            _tile.SetData(_entityType);
            ReloadEntity(_tile);
            OnKitchenUpdated?.Invoke();
        }

        private void CheckForKitchenUpgraded()
        {
            _stationsCount = 0;
            List<GameObject> _lockedSlots = new List<GameObject>();
            foreach(GameObject o in _instantiatedObjects)
            {
                KitchenTile _tile = KitchenLayoutManager.Instance.GetTileWithEntity(o);
                if(_tile.TileType == TileType.BENCH)
                {
                    if (_tile.TileData != "locked_slot")
                        _stationsCount++;
                    else
                    {
                        Debug.Log("Found Locked slot that needs to be replaced");
                        _lockedSlots.Add(o);
                    }
                }
            }

            for (int i = _stationsCount; i < _playerDataContainer.SelectedKitchenData.maxStationSlots; ++i)
            {
                GameObject _object = _lockedSlots.Last();
                KitchenTile _tile = KitchenLayoutManager.Instance.GetTileWithEntity(_object);
                SetEntityTo(_tile.Position, "cutting_station");
                _lockedSlots.Remove(_object);

                //Init new station from level 0 to level 1
                _tile.UpgradableData.Upgrade();
            }
            _stationsCount = _playerDataContainer.SelectedKitchenData.maxStationSlots;
        }

        private void CreateEntity(KitchenTile _tile)
        {
            var prefab = _prefabs.FirstOrDefault(e => e.Type == _tile.TileData);
            if (prefab == null)
            {
                LogDebugMessage("No prefab available for this entity...");
                return;
            }

            GameObject instance = Instantiate(prefab.gameObject, new Vector3(_tile.Position.x, 0, _tile.Position.y), Quaternion.identity,
                _kitchenParent.transform);
            
            instance.transform.parent = _kitchenParent.transform;
            _instantiatedObjects.Add(instance);
            _instantiatedEntityTypes.Add(instance.GetComponent<EntityType>());
            _tile.SetEntity(instance);
            SetupEntityRotation(instance, _tile.Position.x, _tile.Position.y);
        }

        public void ReloadEntity(KitchenTile _tile)
        {
            if(_tile.LinkedEntity != null)
            {
                RemoveEntity(_tile);
            }

            CreateEntity(_tile);
        }

        private void RemoveEntity(KitchenTile _tile)
        {
            GameObject _entity = _tile.LinkedEntity;
            int _entityIndex = _instantiatedObjects.IndexOf(_tile.LinkedEntity);
            if (_entityIndex > 0)
            {
                _instantiatedObjects.RemoveAt(_entityIndex);
                _instantiatedEntityTypes.RemoveAt(_entityIndex);
            }

            if (_entity != null)
            { 
                _tile.SetEntity(null);
                Destroy(_entity);
            }
        }

        private void SetupEntityRotation(GameObject _instance, int x, int y)
        {
            KitchenTile _tile = _kitchenData.tiles[x, y];
            if (_tile.TileType == TileType.BENCH || _tile.TileType == TileType.STORAGE)
            {
                if(_tile.Entrypoint != EntryPoint.NONE)
                {
                    switch(_tile.Entrypoint)
                    {
                        case EntryPoint.RIGHT:
                            _instance.transform.forward = Vector3.right;
                            break;
                        case EntryPoint.LEFT:
                            _instance.transform.forward = -Vector3.right;
                            break;
                        case EntryPoint.UP:
                            _instance.transform.forward = Vector3.forward;
                            break;
                        case EntryPoint.DOWN:
                            _instance.transform.forward = -Vector3.forward;
                            break;
                    }
                }
            }
        }
        
        private void LogDebugMessage(string _message)
        {
            if (_logDebugMessage == false) return;
            print(_message);
        }

        public List<GameObject> InstantiatedObjects { get => _instantiatedObjects; }
        public List<EntityType> InstantiatedEntityTypes { get => _instantiatedEntityTypes; }
    }
}
