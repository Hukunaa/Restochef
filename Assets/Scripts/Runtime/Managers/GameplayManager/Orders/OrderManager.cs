using System.Collections;
using System.Collections.Generic;
using Runtime.Managers.GameplayManager.Orders.CustomClass;
using Runtime.Pool;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Runtime.Managers.GameplayManager.Orders
{
    public class OrderManager : MonoBehaviour
    {
        [SerializeField] 
        private OrderPool _orderPool;

        [SerializeField] 
        private OrderManagerSettings _settings;

        [SerializeField] 
        private Transform _orderContainer;
        
        [SerializeField] 
        private IngredientProcessEventChannel _onIngredientProcess;
        
        [Header("Listening to")]
        
        [SerializeField] 
        private VoidEventChannel _onShiftStartedEventChannel;

        [SerializeField] 
        private VoidEventChannel _onShiftEndedEventChannel;

        [Header("Broadcasting on")]
        
        [SerializeField] 
        private VoidEventChannel _onOrderSuccess;
        
        [SerializeField] 
        private VoidEventChannel _onOrderFailure;
        
        [Header("Tutorial")]
        [SerializeField] 
        private bool _overrideFirstOrder;

        [SerializeField] 
        private Recipe _firstOrderRecipe;

        [SerializeField] 
        private Order _firstOrderRef;
        
        private List<Order> _orders = new List<Order>();
        
        public UnityAction<Order, EPerformanceFeedback> OrderClosed;

        private Coroutine _generateOrdersCoroutine;

        private bool _shiftStarted;

        private void Awake()
        {
            _onIngredientProcess.onEventRaised += AssignIngredientToOrder;
            _onShiftStartedEventChannel.onEventRaised += OnShiftStarted;
            _onShiftEndedEventChannel.onEventRaised += OnShiftEnded;
        }
        
        private void OnShiftStarted()
        {
            _shiftStarted = true;
            
            if (_overrideFirstOrder)
            {
                _firstOrderRef.SetOrder(_firstOrderRecipe);
                _firstOrderRef._onOrderClosed += DestroyFirstOrder;
                _orders.Add(_firstOrderRef);
                return;
            }
            
            _generateOrdersCoroutine = StartCoroutine(GenerateOrdersWithDelay());
        }

        private void DestroyFirstOrder(Order _order, EPerformanceFeedback _performanceFeedback)
        {
            OrderClosed?.Invoke(_order, _performanceFeedback);
            if (_performanceFeedback == EPerformanceFeedback.Failed)
            {
                _onOrderFailure.RaiseEvent();
            }
            else
            {
                _onOrderSuccess.RaiseEvent();
            }
            
            _order._onOrderClosed -= DestroyFirstOrder;
            _orders.Remove(_order);
        }

        private void OnShiftEnded()
        {
            _shiftStarted = false;
            ClearOrders();
            StopCoroutine(_generateOrdersCoroutine);
        }

        private void Update()
        {
            if (!_shiftStarted || _orders.Count > 0) return;

           StartOrderGeneration();
        }

        public void StartOrderGeneration()
        {
            if (_generateOrdersCoroutine != null)
            {
                StopCoroutine(_generateOrdersCoroutine);
            }
            
            _generateOrdersCoroutine = StartCoroutine(GenerateOrdersWithDelay());
        }

        private IEnumerator GenerateOrdersWithDelay()
        {
            while (true)
            {
                var randomRecipe = PickRandomWeightedRecipe();
                CreateOrder(randomRecipe);

                float delay = GetRandomDelay();

                if (randomRecipe.RecipeIngredients.Length <= 2)
                    yield return new WaitForSeconds(delay - (delay * (_settings.AppearanceSpeedPercentageForSmallOrders / 100)));
                else
                    yield return new WaitForSeconds(delay);


            }
        }

        private int GetRandomDelay()
        {

            float value = Random.Range(_settings.OrderApparitionSpeed.x, _settings.OrderApparitionSpeed.y);
            float reducer = (_settings.OrderSpeedMultiplierPerRank / 100) * value;
            return (int)(value - reducer);
        }

        private Recipe PickRandomWeightedRecipe()
        {
            int[] recipeWeights = GetRecipesWeights();
            int recipeIndex =  RandomHelper.GetRandomWeightedIndex(recipeWeights);
            return GameManager.Instance.PlayerDataContainer.SelectedKitchenData._menuRecipes[recipeIndex];
        }
        
        private int[] GetRecipesWeights()
        {
            int[] recipeWeights = new int[GameManager.Instance.PlayerDataContainer.SelectedKitchenData._menuRecipes.Count];
            for (int i = 0; i < GameManager.Instance.PlayerDataContainer.SelectedKitchenData._menuRecipes.Count; i++)
            {
                recipeWeights[i] = GameManager.Instance.PlayerDataContainer.SelectedKitchenData._menuRecipes[i].OrderApparitionWeight;
            }

            return recipeWeights;
        }
        
        private void CreateOrder(Recipe _recipe)
        {
            Order newOrder = _orderPool._requestOrderItem();
            newOrder.transform.SetParent(_orderContainer);
            newOrder.SetOrder(_recipe);
            newOrder._onOrderCancelled += CancelOrder;
            newOrder._onOrderClosed += CloseOrder;
            _orders.Add(newOrder);
        }

        private void CloseOrder(Order _order, EPerformanceFeedback _performanceFeedback)
        {
            OrderClosed?.Invoke(_order, _performanceFeedback);
            if (_performanceFeedback == EPerformanceFeedback.Failed)
            {
                _onOrderFailure.RaiseEvent();
            }
            else
            {
                _onOrderSuccess.RaiseEvent();
            }
            
            _order._onOrderCancelled -= CancelOrder;
            _order._onOrderClosed -= CloseOrder;
            _orders.Remove(_order);
        }

        private void CancelOrder(Order _order)
        {
            _orders.Remove(_order);
            _order._onOrderCancelled -= CancelOrder;
            _order._onOrderClosed -= CloseOrder;
            _onOrderFailure.RaiseEvent();
        }

        private void ClearOrders()
        {
            for (int i = _orders.Count - 1; i >= 0 ; i--)
            {
               _orders[i].Release();
               _orders.RemoveAt(i);
            }
        }
        
        public void AssignIngredientToOrder(Ingredient _ingredient)
        {
            foreach (var order in _orders)
            {
                if (order.TryAssignIngredient(_ingredient))
                {
                    return;
                }
            }
        }
        
        private void OnDestroy()
        {
            _onIngredientProcess.onEventRaised -= AssignIngredientToOrder;
            _onShiftStartedEventChannel.onEventRaised -= OnShiftStarted;
            _onShiftEndedEventChannel.onEventRaised -= OnShiftEnded;
        }

        public List<Order> Orders => _orders;
    }
}