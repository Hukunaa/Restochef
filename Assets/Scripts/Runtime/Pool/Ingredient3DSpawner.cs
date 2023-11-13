using System;
using Runtime.Gameplay;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

namespace Runtime.Pool
{
    public class Ingredient3DSpawner : MonoBehaviour
    {
        [SerializeField] private AssetReferenceT<GameObject> _ingredientPrefabRef;
        private GameObject _ingredientPrefab;
        [SerializeField] private int _defaultCapacity = 10;
        
        private ObjectPool<Ingredient3D> _ingredientPool;

        private void Awake()
        {
            _ingredientPrefabRef.LoadAssetAsync<GameObject>().Completed += handle =>
            {
                _ingredientPrefab = handle.Result;
                _ingredientPool = new ObjectPool<Ingredient3D>(CreatePooledIngredient, OnTakeFromPool, OnReturnedToPool,
                    null, true, _defaultCapacity);
            };
        }

        public Ingredient3D RequestIngredient3D()
        {
            return _ingredientPool.Get();
        }

        private Ingredient3D CreatePooledIngredient()
        {
            var ingredient = Instantiate(_ingredientPrefab);
            var ingredient3D = ingredient.GetComponent<Ingredient3D>();
            ingredient3D.SetPool(_ingredientPool);
            return ingredient3D;
        }

        private void OnTakeFromPool(Ingredient3D _ingredient)
        {
            _ingredient.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(Ingredient3D _ingredient)
        {
            _ingredient.gameObject.SetActive(false);
        }
    }
}   