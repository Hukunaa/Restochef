using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;

namespace Runtime.SceneManagement
{
	public class SceneController : MonoBehaviour
	{
		[SerializeField] private LocationSO _shiftScene;
		[SerializeField] private LocationSO _kitchenCustomizationScene;

		[SerializeField] private bool _showLoadScreen;
	
		[Header("Broadcasting on")]
		[SerializeField] private LoadEventChannel _loadLocation;
		
		public void LoadShiftScene()
		{
			_loadLocation.RaiseEvent(_shiftScene, _showLoadScreen);
		}

		public void LoadKitchenCustomizationScene()
		{
			_loadLocation.RaiseEvent(_kitchenCustomizationScene, _showLoadScreen);
			
		}
	}
}
