using System;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.ScriptableObjects.Gameplay.Ingredients
{
    [CreateAssetMenu(fileName = "NewProcessedIngredient", menuName = "Ingredients/ProcessedIngredient", order = 0)]
    public class ProcessedIngredient : Ingredient
    {
        [SerializeField]
        private IngredientMix _ingredientMix;
        public IngredientMix IngredientMix => _ingredientMix;

        private void Reset()
        {
            _ingredientType = EIngredientType.ProcessedIngredient;
        }
    }
}