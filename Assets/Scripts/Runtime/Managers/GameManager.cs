using System.Threading.Tasks;
using GeneralScriptableObjects.SceneData;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance = null;
        public static GameManager Instance => instance;
        
        [SerializeField] 
        private VoidEventChannel[] _returnToMenuEventChannel;
        
        [SerializeField] 
        private LoadEventChannel _loadEventChannel;
        
        [SerializeField] 
        private GameSceneSO _sceneToLoad;
        
        [SerializeField] private AssetReferenceT<PlayerDataContainer> _playerDataContainerAssetRef;
        private PlayerDataContainer _playerDataContainer;
        
        [SerializeField] private AssetReferenceT<VoidEventChannel> _onPlayerDataLoaded;
        
        private AsyncOperationHandle<PlayerDataContainer> _playerDataContainerLoadHandle;
        private AsyncOperationHandle<VoidEventChannel> _onPlayerDataLoadedHandle;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                instance = this;
            }

            TinySauce.OnGameStarted();
            LoadDataContainers();
            
            foreach (var eventChannel in _returnToMenuEventChannel)
            {
                eventChannel.onEventRaised += ReturnToMenu;
            }
        }
        
        private async void LoadDataContainers()
        {
            _playerDataContainerLoadHandle = _playerDataContainerAssetRef.LoadAssetAsync<PlayerDataContainer>();
            _playerDataContainerLoadHandle.Completed += _handle => _playerDataContainer = _handle.Result;

            while (!_playerDataContainerLoadHandle.IsDone)
            {
                await Task.Delay(50);
            }

            await _playerDataContainer.LoadPlayerData();
            
            DataLoaded = true;

            _onPlayerDataLoadedHandle = _onPlayerDataLoaded.LoadAssetAsync<VoidEventChannel>();
            _onPlayerDataLoadedHandle.Completed += _handle => _handle.Result.RaiseEvent();
        }

        private void OnDestroy()
        {
            Addressables.Release(_playerDataContainerLoadHandle);
            Addressables.Release(_onPlayerDataLoadedHandle);
            
            foreach (var eventChannel in _returnToMenuEventChannel)
            {
                eventChannel.onEventRaised -= ReturnToMenu;
            }
        }

        private void OnApplicationPause(bool _pauseStatus)
        {
            if (_pauseStatus)
            {
                TinySauce.OnGameFinished(_playerDataContainer.PlayerScore.Score);
            }
            else
            {
                TinySauce.OnGameStarted();
            }
        }

        private void ReturnToMenu()
        {
            _loadEventChannel.RaiseEvent(_sceneToLoad, true, true);
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        public PlayerDataContainer PlayerDataContainer => _playerDataContainer;

        public bool DataLoaded { get; private set; }
    }
}
