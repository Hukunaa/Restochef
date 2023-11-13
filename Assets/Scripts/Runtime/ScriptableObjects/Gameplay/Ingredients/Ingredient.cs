using System;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.ScriptableObjects.Gameplay.Ingredients
{
    public abstract class Ingredient : ScriptableObject
    {
        [SerializeField]
        protected string _ingredientName;
        public string IngredientName => _ingredientName;
        
        [SerializeField] 
        protected Sprite _ingredientIcon;
        public Sprite IngredientIcon => _ingredientIcon;

        [SerializeField] 
        private Ingredient3DSettings _ingredient3DSettings;
        public Ingredient3DSettings Ingredient3DSettings => _ingredient3DSettings;

        [SerializeField] 
        protected EIngredientType _ingredientType;
        public EIngredientType IngredientType => _ingredientType;
    }

    [Serializable]
    public class Ingredient3DSettings
    {
        [SerializeField] 
        private Mesh _ingredientMesh;
        public Mesh IngredientMesh => _ingredientMesh;

        [SerializeField] private EIngredientSocket _ingredientSocket;
        public EIngredientSocket IngredientSocket => _ingredientSocket;
    }
    
}