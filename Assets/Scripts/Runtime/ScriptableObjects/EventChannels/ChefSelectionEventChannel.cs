using Runtime.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.ScriptableObjects.EventChannels
{
    [CreateAssetMenu(fileName = "OnChefSelectedEventChannel", menuName = "ScriptableObjects/Events/ChefSelectionEventChannel", order = 0)]
    public class ChefSelectionEventChannel : ScriptableObject
    {
        public UnityAction<Chef> onEventRaised;

        public void RaiseEvent(Chef val)
        {
            onEventRaised?.Invoke(val);
        }
    }
}