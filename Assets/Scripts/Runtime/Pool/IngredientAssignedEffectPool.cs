using Runtime.Managers.GameplayManager.Orders.CustomClass;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.Pool
{
    public class IngredientAssignedEffectPool : MonoBehaviour
    {
        [SerializeField] 
        private AssetReferenceT<GameObject> _ingredientEffectAssetRef;
        
        [SerializeField] 
        private int _defaultCapacity;
        
        [SerializeField] 
        private Transform _inactiveContainer;
        
        [SerializeField] 
        private Transform _activeContainer;
        
        private ObjectPool<IngredientAssignedEffect> _pool;
        private AsyncOperationHandle<GameObject> _operationHandle;
        private GameObject _ingredientEffectPrefab;

        private void Awake()
        {
             _operationHandle = _ingredientEffectAssetRef.LoadAssetAsync<GameObject>();
             _operationHandle.Completed += handle =>
            {
                _ingredientEffectPrefab = handle.Result;
                _pool = new ObjectPool<IngredientAssignedEffect>(CreatePooledIngredient, OnTakeFromPool, OnReturnedToPool,
                    null, true, _defaultCapacity);
            };
        }
        
        public IngredientAssignedEffect RequestIngredientAssignedEffect()
        {
            return _pool.Get();
        }

        private IngredientAssignedEffect CreatePooledIngredient()
        {
            var instance = Instantiate(_ingredientEffectPrefab);
            var ingredientAssignedEffect = instance.GetComponent<IngredientAssignedEffect>();
            ingredientAssignedEffect.SetPool(_pool);
            var effectTransform = ingredientAssignedEffect.transform;
            effectTransform.parent = _inactiveContainer;
            effectTransform.localScale = new Vector3(100, 100, 100);
            return ingredientAssignedEffect;
        }

        private void OnTakeFromPool(IngredientAssignedEffect _ingredientEffect)
        {
            _ingredientEffect.transform.parent = _activeContainer;
            _ingredientEffect.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(IngredientAssignedEffect _ingredientEffect)
        {
            _ingredientEffect.gameObject.SetActive(false);
            _ingredientEffect.transform.parent = _inactiveContainer;
        }
    }
}