using Runtime.ScriptableObjects.Gameplay;
using System.Collections;
using System.Collections.Generic;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using UnityEngine;

namespace Runtime.DataContainers
{
    public class IngredientResult
    {
        private Ingredient _result;
        private IngredientMix _resultMix;

        public IngredientResult(Ingredient _ingredient, IngredientMix _mix)
        {
            _result = _ingredient;
            _resultMix = _mix;
        }

        public Ingredient Result { get => _result; }
        public IngredientMix ResultMix { get => _resultMix; }
    }
}
