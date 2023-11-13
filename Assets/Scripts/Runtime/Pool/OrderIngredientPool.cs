using Runtime.Managers.GameplayManager.Orders;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.Pool
{
    public class OrderIngredientPool : MonoBehaviour
    {
        [SerializeField] 
        private AssetReferenceT<GameObject> _orderIngredientAssetRef;
        
        [SerializeField] 
        private int _defaultCapacity;
        
        private ObjectPool<OrderIngredient> _pool;
        private AsyncOperationHandle<GameObject> _operationHandle;
        private GameObject _orderIngredientPrefab;

        private void Awake()
        {
            _operationHandle = _orderIngredientAssetRef.LoadAssetAsync<GameObject>();
            _operationHandle.Completed += OperationHandleOnCompleted;
        }
        
        private void OperationHandleOnCompleted(AsyncOperationHandle<GameObject> _obj)
        {
            _orderIngredientPrefab = _obj.Result;
            _pool = new ObjectPool<OrderIngredient>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                null, true, _defaultCapacity);
        }

        public OrderIngredient RequestOrderIngredient()
        {
            return _pool.Get();
        }

        private void OnReturnedToPool(OrderIngredient _obj)
        {
            _obj.gameObject.SetActive(false);
            _obj.transform.SetParent(transform);
            _obj.ResetIngredient();
        }

        private void OnTakeFromPool(OrderIngredient _obj)
        {
            _obj.ResetIngredient();
            _obj.gameObject.SetActive(true);
        }

        private OrderIngredient CreatePooledItem()
        {
            var instance = Instantiate(_orderIngredientPrefab, transform).GetComponent<OrderIngredient>();
            instance.SetPool(_pool);
            return instance;
        }
    }
}