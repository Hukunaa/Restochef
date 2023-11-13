using Runtime.DataContainers.Stats;
using Runtime.ScriptableObjects.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.ScriptableObjects.EventChannels
{
    [CreateAssetMenu(fileName = "RecipeEventChannel", menuName = "ScriptableObjects/Events/RecipeEventChannel", order = 0)]
    public class RecipeEventChannel : ScriptableObject
    {
        public UnityAction<Recipe> onEventRaised;

        public void RaiseEvent(Recipe val)
        {
            onEventRaised?.Invoke(val);
        }
    }
}