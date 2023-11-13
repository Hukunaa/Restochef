using Runtime.DataContainers.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.ScriptableObjects.EventChannels
{
    [CreateAssetMenu(fileName = "ChefDataEventChannel", menuName = "ScriptableObjects/Events/ChefDataEventChannel", order = 0)]
    public class ChefDataEventChannel : ScriptableObject
    {
        public UnityAction<ChefData> onEventRaised;

        public void RaiseEvent(ChefData val)
        {
            onEventRaised?.Invoke(val);
        }
    }
}