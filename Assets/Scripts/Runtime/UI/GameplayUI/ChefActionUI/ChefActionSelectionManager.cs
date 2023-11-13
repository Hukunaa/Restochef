using Runtime.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.UI.GameplayUI.ChefActionUI
{
    public class ChefActionSelectionManager : MonoBehaviour
    {
        private Instruction _currentInstruction;

        [SerializeField] 
        private UnityEvent _onChefSelected;

        [SerializeField] 
        private UnityEvent _onChefDeselected;

        [SerializeField] 
        private UnityEvent<IngredientSelectionButton> _onIngredientSelected;
        
        [SerializeField] 
        private UnityEvent _onIngredientDeselected;

        [SerializeField] 
        private UnityEvent _onStationSelected;
        
        public void OnChefSelected(ChefSelectionButton _chef)
        {
            _currentInstruction = new Instruction();
            _currentInstruction.SelectChef(_chef.Chef);
            _onChefSelected?.Invoke();
        }

        public void OnChefDeselected()
        {
            _currentInstruction = null;
            _onChefDeselected?.Invoke();
        }

        public void OnIngredientSelected(IngredientSelectionButton _ingredientSelectionButton)
        {
            _currentInstruction.SelectIngredient(_ingredientSelectionButton.Ingredient);
            _onIngredientSelected?.Invoke(_ingredientSelectionButton);
        }

        public void OnStationSelected(StationSelectionButton _stationSelectionButton)
        {
            _currentInstruction.SelectStationAction(_stationSelectionButton.StationAction);
            SendInstruction();
            _onStationSelected?.Invoke();
        }

        public void OnIngredientDeselected()
        {
            if (_currentInstruction != null)
            {
                _currentInstruction.DeselectIngredient();
            }
            _onIngredientDeselected?.Invoke();
        }

        private void SendInstruction()
        {
            _currentInstruction.Chef.AddInstruction(_currentInstruction);
        }
    }
}