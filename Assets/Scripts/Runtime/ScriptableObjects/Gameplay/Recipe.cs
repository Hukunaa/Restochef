using System;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.ScriptableObjects.Gameplay
{
    public enum RARITY
    {
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY
    }

    [CreateAssetMenu(fileName = "New Recipe", menuName = "ScriptableObjects/Gameplay/Recipe", order = 0)]
    public class Recipe : ScriptableObject
    {
        [SerializeField] 
        private string _recipeName;
        [SerializeField]
        private RARITY _recipeRarity;
        public string RecipeName => _recipeName;
        
        [SerializeField][Tooltip("Sprite that will be displayed in the order UI")]
        private Sprite _recipeIcon;
        public Sprite RecipeIcon => _recipeIcon;
        
        [SerializeField][Range(20, 120)][Tooltip("Time in seconds to complete the recipe")]
        private int _recipeTimer = 40;
        public int RecipeTimer => _recipeTimer;
        
        [Tooltip("List of ingredients required for the recipe")]
        [SerializeField] 
        private RecipeIngredients[] _recipeIngredients;
        public RecipeIngredients[] RecipeIngredients => _recipeIngredients;

        [SerializeField][Range(1,10)]
        private int _orderApparitionWeight = 5;
        public int OrderApparitionWeight => _orderApparitionWeight;

        [SerializeField]
        private uint _recipePoints;
        public uint RecipePoints => _recipePoints;

        public RARITY Rarity { get => _recipeRarity; set => _recipeRarity = value; }
    }
    
    [Serializable]
    public struct RecipeIngredients
    {
        [SerializeField]
        private ProcessedIngredient _ingredient;
        public ProcessedIngredient Ingredient => _ingredient;

        [SerializeField]
        private UInt16 _quantity;
        public UInt16 Quantity => _quantity;
    }
}