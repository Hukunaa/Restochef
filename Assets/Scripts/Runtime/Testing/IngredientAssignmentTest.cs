using Runtime.Managers.GameplayManager;
using Runtime.Managers.GameplayManager.Orders;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Testing
{
    public class IngredientAssignmentTest : MonoBehaviour
    {
        private OrderManager _orderManager;
        [SerializeField] private Ingredient[] _ingredients;

        private void Awake()
        {
            _orderManager = FindObjectOfType<OrderManager>();
        }

        public void AssignIngredient()
        {
            _orderManager.AssignIngredientToOrder(GetRandomIngredient());
        }

        private Ingredient GetRandomIngredient()
        {
            var ingredient = _ingredients[Random.Range(0, _ingredients.Length)];
            //print($"Assigned {ingredient.IngredientName} to orders");
            return ingredient;
        }
        
    }
}