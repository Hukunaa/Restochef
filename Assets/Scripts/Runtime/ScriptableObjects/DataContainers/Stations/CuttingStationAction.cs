using System.Linq;
using Runtime.DataContainers;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.ScriptableObjects.DataContainers.Stations
{
    [CreateAssetMenu(fileName = "CuttingStationAction", menuName = "ScriptableObjects/Gameplay/StationAction/CuttingStationAction", order = 0)]
    public class CuttingStationAction : BaseStationAction
    {
        public override IngredientResult ProcessIngredient(RawIngredient _ingredient)
        {
            IngredientMix output;
            
            var mixe = Mixes.FirstOrDefault(x => x.Input == _ingredient);
            if (mixe == null)
            {
                DebugHelper.PrintDebugMessage("No ingredient Match!", true);
                return null;
            }
            
            output = mixe;
            
            DebugHelper.PrintDebugMessage($"Ingredient Match for {output.Output.IngredientName}", false);
            return new IngredientResult(output.Output, output);
        }
    }
}
