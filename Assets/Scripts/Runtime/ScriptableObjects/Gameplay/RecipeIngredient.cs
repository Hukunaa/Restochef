using System;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using UnityEngine;

namespace Runtime.ScriptableObjects.Gameplay
{
    [Serializable]
    public struct RecipeIngredient
    {
        [SerializeField] 
        private Ingredient _ingredient;
        public Ingredient Ingredient => _ingredient;
    }
}