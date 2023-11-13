using Runtime.ScriptableObjects.Gameplay;
using Runtime.UI.MainMenuUI.StoreUI;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.ScriptableObjects.EventChannels
{
    [CreateAssetMenu(fileName = "StoreRecipeItemEventChannel", menuName = "ScriptableObjects/Events/StoreRecipeItemEventChannel", order = 0)]
    public class StoreRecipeItemEventChannel : ScriptableObject
    {
        public UnityAction<StoreRecipeItem> onEventRaised;

        public void RaiseEvent(StoreRecipeItem val)
        {
            onEventRaised?.Invoke(val);
        }
    }
}