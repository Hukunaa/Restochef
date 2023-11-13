using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Pool;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Runtime.Managers.GameplayManager.Orders.CustomClass
{
    public class Order : MonoBehaviour
    {
        [SerializeField] 
        private SlicedFilledImage _orderTimer;

        [SerializeField] 
        private Image _recipeImage;

        [SerializeField] 
        private Transform _ingredientContainer;

        [SerializeField]
        private Image _cardBackground;

        [SerializeField] 
        private CanvasGroup _orderCanvasGroup;

        [SerializeField] 
        private Button _cancelButton;
        
        [SerializeField]
        private RarityColors _rarityColors;

        [SerializeField] private UnityEvent<bool> _onOrderComplete;
        
        private OrderIngredientPool _orderIngredientPool;
        private Recipe _recipe;

        private float _maxTime;
        private float CurrentTime { get; set; }

        public Action<Order, EPerformanceFeedback> _onOrderClosed;
        public Action<Order> _onOrderCancelled;
        private AsyncOperationHandle<GameObject> _operationHandle;
        
        private List<OrderIngredient> _orderIngredients = new List<OrderIngredient>();

        private bool _orderClosed;
        
        private IObjectPool<Order> _pool;

        private bool _initialized;

        private void Awake()
        {
            _orderIngredientPool = GameObject.FindGameObjectWithTag("OrderIngredientPool").GetComponent<OrderIngredientPool>();
        }

        private void Update()
        {
            if (!_initialized || _orderClosed) return;
            
            CurrentTime -= Time.deltaTime;
            UpdateProgressBar();
            
            if (CurrentTime <= 0)
            {
                CloseOrder();
            }
        }

        private void UpdateProgressBar()
        {
            _orderTimer.fillAmount = Mathf.Clamp01(CurrentTime / _maxTime);
        }

        public void SetOrder(Recipe _recipe)
        {
            this._recipe = _recipe;
            _maxTime = _recipe.RecipeTimer;
            CurrentTime = _maxTime;
            UpdateProgressBar();
            _orderCanvasGroup.interactable = true;
            _recipeImage.sprite = _recipe.RecipeIcon;
            _cardBackground.color = _rarityColors.Values[(int)_recipe.Rarity];
            _initialized = true;
            InitializeIngredients();
        }

        public void SetDefaultParameters()
        {
            _cancelButton.gameObject.SetActive(true);
            _orderClosed = false;
        }

        public void CleanIngredients()
        {
            if(_orderIngredients.Count == 0) return;

            for (int i = _orderIngredients.Count - 1; i >= 0; i--)
            {
                _orderIngredients[i].Release();
                _orderIngredients.RemoveAt(i);
            }
        }
        
        private void InitializeIngredients()
        {
            CleanIngredients();
            
            foreach (var recipeIngredient in _recipe.RecipeIngredients)
            {
                for (int i = 0; i < recipeIngredient.Quantity; i++)
                {
                    var instance = _orderIngredientPool.RequestOrderIngredient();
                    instance.transform.SetParent(_ingredientContainer);
                    instance.Initialize(recipeIngredient.Ingredient);
                    _orderIngredients.Add(instance);
                }
            }
        }

        public void CancelOrder()
        {
            if (_orderClosed) return;
            _onOrderCancelled?.Invoke(this);
            OrderComplete(false);
        }
        
        private void CloseOrder()
        {
            EPerformanceFeedback closeStatus = GetCloseStatus(); 
            _onOrderClosed?.Invoke(this, closeStatus);
            OrderComplete(closeStatus != EPerformanceFeedback.Failed);
        }

        private void OrderComplete(bool _orderSuccessful)
        {
            _initialized = false;
            _orderClosed = true;
            _orderCanvasGroup.interactable = false;
            _cancelButton.gameObject.SetActive(false);
            _onOrderComplete?.Invoke(_orderSuccessful);
        }
        
        private EPerformanceFeedback GetCloseStatus()
        {
            var ratio = Mathf.Clamp01(CurrentTime / _maxTime);

            return ratio switch
            {
                > 0.5f => EPerformanceFeedback.Excellent,
                > 0.25f => EPerformanceFeedback.Good,
                _ => ratio > 0 ? EPerformanceFeedback.Completed : EPerformanceFeedback.Failed
            };
        }

        public bool TryAssignIngredient(Ingredient _ingredient)
        {
            var orderIngredient = _orderIngredients.FirstOrDefault(i => i.Ingredient == _ingredient && i.Completed == false);
            
            if (orderIngredient == null)
            {
                return false;
            }
            
            orderIngredient.AssignIngredient();

            if (IsOrderComplete())
            {
                CloseOrder();
            }
            
            return true;
        }

        private bool IsOrderComplete()
        {
            return _orderIngredients.All(_i => _i.Completed == true);
        }
        
        public void SetPool(IObjectPool<Order> _pool) => this._pool = _pool;

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void Release()
        {
            _pool.Release(this);
        }
        
        public Recipe Recipe => _recipe;

        public List<OrderIngredient> OrderIngredients => _orderIngredients;
    }
}