using Runtime.DataContainers.Stats;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.UI.MainMenuUI.StoreUI;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Managers
{
    public class ChefCustomizationRequestCatcher : MonoBehaviour
    {
        [SerializeField] 
        private ChefDataEventChannel _brigadeInfoEventChannel;

        [SerializeField] private StoreChefItemEventChannel _storeInfoEventChannel;
        

        [SerializeField]
        private UnityEvent<CustomizationSettings> _customizeChef;

        private void Awake()
        {
            _brigadeInfoEventChannel.onEventRaised += CustomizeChef;
            _storeInfoEventChannel.onEventRaised += CustomizeChef;
        }

        private void OnDestroy()
        {
            _brigadeInfoEventChannel.onEventRaised -= CustomizeChef;
            _storeInfoEventChannel.onEventRaised -= CustomizeChef;
        }

        private void CustomizeChef(ChefData _chef)
        {
            _customizeChef?.Invoke(_chef.ChefSettings.CustomizationSettings);
        }

        private void CustomizeChef(StoreChefItem _storeItem)
        {
            _customizeChef?.Invoke(_storeItem.ChefData.ChefSettings.CustomizationSettings);
        }
    }
}