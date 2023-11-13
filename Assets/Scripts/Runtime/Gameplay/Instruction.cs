using System;
using Runtime.ScriptableObjects.DataContainers.Stations;
using Runtime.ScriptableObjects.Gameplay.Ingredients;

namespace Runtime.Gameplay
{
    [Serializable]
    public class Instruction
    {
        public Chef Chef { get; private set; }

        public RawIngredient Ingredient { get; private set; }

        public BaseStationAction StationAction { get; private set; }

        public Instruction(){}

        public Instruction(Chef _chef, RawIngredient _ingredient, BaseStationAction _stationAction)
        {
            Chef = _chef;
            Ingredient = _ingredient;
            StationAction = _stationAction;
        }

        public void SelectChef(Chef _selectedChef)
        {
            Chef = _selectedChef;
        }

        public void SelectIngredient(RawIngredient _ingredientSelected)
        {
            Ingredient = _ingredientSelected;
        }

        public void SelectStationAction(BaseStationAction _selectedStationAction)
        {
            StationAction = _selectedStationAction;
        }

        public void DeselectIngredient()
        {
            Ingredient = null;
        }

        public string FormatInstruction()
        {
            return $"*** {Chef.ChefSettings.ChefName} process {Ingredient.IngredientName} with {StationAction.StationActionType.name}. ***";
        }

        public ProcessedIngredient IngredientOutput => StationAction.GetMixOutput(Ingredient);
    }
}
