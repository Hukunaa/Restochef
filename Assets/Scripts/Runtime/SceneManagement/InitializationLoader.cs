using System.Collections;
using GeneralScriptableObjects.SceneData;
using Runtime.Managers;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Runtime.SceneManagement
{
	/// <summary>
	/// This class is responsible for starting the game by loading the persistent managers scene 
	/// and raising the event to load the Main Menu
	/// </summary>

	public class InitializationLoader : MonoBehaviour
	{
		[SerializeField] 
		private GameSceneSO _managersScene = default;
		
		[SerializeField] 
		private GameSceneSO _menuScene = default;
		
		[SerializeField] 
		private GameSceneSO _tutorialScene = default;
		
		[SerializeField] 
		private AssetReference _menuLoadChannel = default;
		
		[SerializeField] 
		private AssetReference _tutorialLoadChannel = default;
		
		[Header("Listening to")]
		[SerializeField] 
		private VoidEventChannel _startInitializationEventChannel;
		
		private LoadEventChannel _loadEventChannel;
		private AsyncOperationHandle<LoadEventChannel> _loadEventChannelHandle;

		private void Awake()
		{
			_startInitializationEventChannel.onEventRaised += Initialize;
		}

		private void OnDestroy()
		{
			if (!_loadEventChannelHandle.IsValid()) return;
			Addressables.Release(_loadEventChannelHandle);
			
		}

		public void Initialize()
		{
			_startInitializationEventChannel.onEventRaised -= Initialize;
			_managersScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += LoadFirstScene;
		}

		private void LoadFirstScene(AsyncOperationHandle<SceneInstance> _handle)
		{
			StartCoroutine(LoadFirstSceneCoroutine());
		}
		
		private IEnumerator LoadFirstSceneCoroutine()
		{
			while (GameManager.Instance == null || !GameManager.Instance.DataLoaded)
			{
				yield return null;
			}

			if (GameManager.Instance.PlayerDataContainer.TutorialData.ShiftTutorialComplete)
			{
				_loadEventChannelHandle = _menuLoadChannel.LoadAssetAsync<LoadEventChannel>();
				_loadEventChannelHandle.Completed += _handle =>
				{
					_loadEventChannel = _handle.Result;
					_loadEventChannel.RaiseEvent(_menuScene, true);
				};
			}

			else
			{
				_loadEventChannelHandle = _tutorialLoadChannel.LoadAssetAsync<LoadEventChannel>();
				_loadEventChannelHandle.Completed += _handle =>
				{
					_loadEventChannel = _handle.Result;
					_loadEventChannel.RaiseEvent(_tutorialScene, true);
				};
			}
			
			SceneManager.UnloadSceneAsync(0);
		}
	}
}
