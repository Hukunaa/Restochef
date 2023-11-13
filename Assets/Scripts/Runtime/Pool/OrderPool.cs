using Runtime.Managers.GameplayManager.Orders.CustomClass;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.Pool
{
    public class OrderPool : MonoBehaviour
    {
        [SerializeField] 
        private AssetReferenceT<GameObject> _orderAssetRef;

        [SerializeField] 
        private int _defaultCapacity;
        
        private GameObject _orderPrefab;
        
        private ObjectPool<Order> _pool;
        private AsyncOperationHandle<GameObject> _operationHandle;

        private void Awake()
        {
            _operationHandle = _orderAssetRef.LoadAssetAsync<GameObject>();
            _operationHandle.Completed += OperationHandleOnCompleted;
        }
        
        private void OperationHandleOnCompleted(AsyncOperationHandle<GameObject> _obj)
        {
            _orderPrefab = _obj.Result;
            _pool = new ObjectPool<Order>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                null, true, _defaultCapacity);
        }

        public Order _requestOrderItem()
        {
            return _pool.Get();
        }
        
        private void OnReturnedToPool(Order _obj)
        {
            _obj.gameObject.SetActive(false);
            _obj.transform.localScale = Vector3.one;
            _obj.CleanIngredients();
            _obj.transform.SetParent(transform);
        }

        private void OnTakeFromPool(Order _obj)
        {
            _obj.CleanIngredients();
            _obj.SetDefaultParameters();
            _obj.gameObject.SetActive(true);
        }

        private Order CreatePooledItem()
        {
            var instance = Instantiate(_orderPrefab, transform).GetComponent<Order>();
            instance.SetPool(_pool);
            return instance;
        }
    }
}