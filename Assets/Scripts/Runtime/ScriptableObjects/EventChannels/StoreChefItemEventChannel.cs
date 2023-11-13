using Runtime.UI.MainMenuUI.StoreUI;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.ScriptableObjects.EventChannels
{
    [CreateAssetMenu(fileName = "StoreChefItemEventChannel", menuName = "ScriptableObjects/Events/StoreChefItemEventChannel", order = 0)]
    public class StoreChefItemEventChannel : ScriptableObject
    {
        public UnityAction<StoreChefItem> onEventRaised;

        public void RaiseEvent(StoreChefItem val)
        {
            onEventRaised?.Invoke(val);
        }
    }
}