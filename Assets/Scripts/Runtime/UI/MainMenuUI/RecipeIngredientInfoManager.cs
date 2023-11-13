using System.Collections.Generic;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using Runtime.UI.MainMenuUI.KitchenDataUI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.UI.MainMenuUI
{
    public class RecipeIngredientInfoManager : MonoBehaviour
    {
        [SerializeField] private AssetReferenceT<GameObject> _ingredientInfoAssetRef;
        private AsyncOperationHandle<GameObject> _loadHandle;
        private GameObject _ingredientInfoPrefab;
        private List<GameObject> _ingredientInfoObjects = new List<GameObject>();
        private Recipe _recipe;
        
        private void OnDestroy()
        {
            if (!_loadHandle.IsValid()) return;
            
            Addressables.Release(_loadHandle);
        }

        private void LoadAsset()
        {
            _loadHandle = _ingredientInfoAssetRef.LoadAssetAsync<GameObject>();
            _loadHandle.Completed += LoadHandleOnCompleted;
        }

        private void LoadHandleOnCompleted(AsyncOperationHandle<GameObject> _obj)
        {
            _ingredientInfoPrefab = _obj.Result;
            InitializeIngredients(_recipe);
        }

        public void InitializeIngredients(Recipe _recipe)
        {
            this._recipe = _recipe;
            if (_ingredientInfoPrefab == null)
            {
               LoadAsset();
               return;
            }
            
            if (_ingredientInfoObjects.Count > 0)
            {
                ClearIngredientsInfo();
            }
            
            foreach(RecipeIngredients ingredient in _recipe.RecipeIngredients)
            {
                for(int i = 0; i < ingredient.Quantity; ++i)
                {
                    IngredientInfo instance = Instantiate(_ingredientInfoPrefab, transform).GetComponent<IngredientInfo>();
                    ProcessedIngredient processed = (ProcessedIngredient)ingredient.Ingredient;
                    instance.SetIngredient(processed.IngredientIcon, processed.IngredientMix.StationAction.StationIcon);
                    _ingredientInfoObjects.Add(instance.gameObject);
                }
            }
        }
        
        private void ClearIngredientsInfo()
        {
            for (int i = _ingredientInfoObjects.Count - 1; i >= 0; i--)
            {
                Destroy(_ingredientInfoObjects[i]);
                _ingredientInfoObjects.RemoveAt(i);
            }
        }
    }
}