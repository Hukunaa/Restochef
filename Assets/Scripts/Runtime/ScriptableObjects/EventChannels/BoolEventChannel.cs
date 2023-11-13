using UnityEngine;
using UnityEngine.Events;

namespace Runtime.ScriptableObjects.EventChannels
{
    [CreateAssetMenu(fileName = "Bool event channel", menuName = "ScriptableObjects/Events/BoolEventChannel", order = 0)]
    public class BoolEventChannel : ScriptableObject
    {
        public UnityAction<bool> onEventRaised;

        public void RaiseEvent(bool val)
        {
            onEventRaised?.Invoke(val);
        }
    }
}