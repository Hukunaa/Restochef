using GeneralScriptableObjects.SceneData;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Runtime.SceneManagement
{
	/// <summary>
	/// Allows a "cold start" in the editor, when pressing Play and not passing from the Initialisation scene.
	/// </summary> 
	public class EditorColdStartup : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField] private GameSceneSO _thisSceneSO;
		[SerializeField] private GameSceneSO _persistentManagersSO;
		[SerializeField] private AssetReference _notifyColdStartupChannel;
		[SerializeField] private LocationChangeEventChannel _locationChangedChannel;
		
		[SerializeField] private VoidEventChannel _onSceneReadyChannel;
		
		private bool _isColdStart;
		private void Awake()
		{
			if (!SceneManager.GetSceneByName(_persistentManagersSO.sceneReference.editorAsset.name).isLoaded)
			{
				_isColdStart = true;
			}
		}

		private void Start()
		{
			if (_isColdStart)
			{
				_persistentManagersSO.sceneReference.LoadSceneAsync(LoadSceneMode.Additive).Completed += LoadEventChannel;
			}
		}
		
		private void LoadEventChannel(AsyncOperationHandle<SceneInstance> obj)
		{
			_notifyColdStartupChannel.LoadAssetAsync<LoadEventChannel>().Completed += OnNotifyChannelLoaded;
		}

		private void OnNotifyChannelLoaded(AsyncOperationHandle<LoadEventChannel> obj)
		{
			if (_thisSceneSO != null)
			{
				obj.Result.RaiseEvent(_thisSceneSO);
			}
			else
			{
				//Raise a fake scene ready event, so the player is spawned
				_onSceneReadyChannel.RaiseEvent();
				//When this happens, the player won't be able to move between scenes because the SceneLoader has no conception of which scene we are in
			}
		}
#endif
	}
}
