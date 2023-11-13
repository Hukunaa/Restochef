using Runtime.ScriptableObjects.Gameplay;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.ScriptableObjects.EventChannels
{
    [CreateAssetMenu(fileName = "OnIngredientProcessEventChannel", menuName = "ScriptableObjects/Events/IngredientProcessEventChannel", order = 0)]
    public class IngredientProcessEventChannel : ScriptableObject
    {
        public UnityAction<Ingredient> onEventRaised;

        public void RaiseEvent(Ingredient val)
        {
            onEventRaised?.Invoke(val);
        }
    }
}