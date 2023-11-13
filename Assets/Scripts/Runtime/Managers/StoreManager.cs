using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Runtime.DataContainers.Stats;
using Runtime.Enums;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.UI.MainMenuUI.StoreUI;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.Managers
{
    public class StoreManager : MonoBehaviour
    {
        [SerializeField]
        private List<Recipe> _recipes;

        [SerializeField]
        private ChefData[] _storeChefs;
        
        [SerializeField] 
        private AssetReferenceT<GameObject> _chefItemAssetRef;
        
        [SerializeField] 
        private AssetReferenceT<GameObject> _recipeItemAssetRef;

        [SerializeField] 
        private Transform _chefItemsContainer;
        
        [SerializeField] 
        private Transform _recipeItemsContainer;

        [Header("Broadcasting on")]
        [SerializeField] 
        private VoidEventChannel _onStoreTabInitialized;

        [SerializeField] 
        private bool _logDebugMessage;
        
        private AsyncOperationHandle<GameObject> _chefItemOperationHandle;
        private AsyncOperationHandle<GameObject> _recipeItemOperationHandle;

        private GameObject _chefItemPrefab;
        private GameObject _recipeItemPrefab;
        
        private AsyncOperationHandle<IList<Recipe>> _loadHandle;
        private const string RecipesAddressPath = "Assets/ScriptableObjects/DataContainers/Recipes/";

        private StoreItem _selectedStoreItem;

        private Dictionary<RARITY, int> _chefPriceTable;
        private Dictionary<RARITY, int> _recipePriceTable;

        private List<StoreChefItem> _chefsInStore = new List<StoreChefItem>();
        private List<StoreRecipeItem> _recipesInStore = new List<StoreRecipeItem>();

        private void Awake()
        {
            Initialize();
        }

        private async void Initialize()
        {
            while (GameManager.Instance == null || !GameManager.Instance.DataLoaded)
            {
                await Task.Delay(50);
            }
            
            var loadRecipes = LoadRecipes();
            var loadChefs = GetChefsData();
            var loadAssetRef = LoadAssetReferences();
            var loadChefPriceTable = DataLoader.LoadChefPricesTable();
            var loadRecipesPriceTable = DataLoader.LoadRecipesPricesTable();
            List<Task> loadingTasks = new List<Task>();
            loadingTasks.Add(loadRecipes);
            loadingTasks.Add(loadChefs);
            loadingTasks.Add(loadAssetRef);
            loadingTasks.Add(loadChefPriceTable);
            loadingTasks.Add(loadRecipesPriceTable);

            while (loadingTasks.Count > 0)
            {
                Task loadTaskComplete = await Task.WhenAny(loadingTasks);
                if (loadTaskComplete == loadRecipes)
                {
                    DebugHelper.PrintDebugMessage("Store Recipes loaded", _logDebugMessage);
                }
                else if (loadTaskComplete == loadChefs)
                {
                    DebugHelper.PrintDebugMessage("Store Recipes loaded", _logDebugMessage);
                }
                else if(loadTaskComplete == loadAssetRef)
                {
                    DebugHelper.PrintDebugMessage("Asset references loaded", _logDebugMessage);
                }
                else if (loadTaskComplete == loadChefPriceTable)
                {
                    DebugHelper.PrintDebugMessage("Chef Price Table loaded", _logDebugMessage);
                    _chefPriceTable = loadChefPriceTable.Result;
                }
                else if (loadTaskComplete == loadRecipesPriceTable)
                {
                    DebugHelper.PrintDebugMessage("Recipes Price Table loaded", _logDebugMessage);
                    _recipePriceTable = loadRecipesPriceTable.Result;
                }

                loadingTasks.Remove(loadTaskComplete);
            }
            
            GenerateStoreItems();
            _onStoreTabInitialized.RaiseEvent();
        }

        private void GenerateStoreItems()
        {
            foreach (var chef in _storeChefs)
            {
                var instance = Instantiate(_chefItemPrefab, _chefItemsContainer).GetComponent<StoreChefItem>();
                instance.Initialize(chef, _chefPriceTable[chef.Rarity]);
                instance.OnStoreItemClicked += OnStoreItemClicked;
                instance.OnPurchase += PurchaseChefItem;
                _chefsInStore.Add(instance);
            }

            foreach (var recipe in _recipes)
            {
                var instance = Instantiate(_recipeItemPrefab, _recipeItemsContainer).GetComponent<StoreRecipeItem>();
                instance.Initialize(recipe, _recipePriceTable[recipe.Rarity]);
                instance.OnStoreItemClicked += OnStoreItemClicked;
                instance.OnPurchase += PurchaseRecipeItem;
                _recipesInStore.Add(instance);
            }
            SortItems();
        }
        
        private async Task LoadRecipes()
        {
            var paths= DataLoader.LoadStoreRecipe().Select(x => $"{RecipesAddressPath}{x}.asset").ToArray();
            IEnumerable keys = paths;
            _loadHandle = Addressables.LoadAssetsAsync<Recipe>(
                keys,
                _recipe =>
                {
                    _recipes.Add(_recipe);
                }, Addressables.MergeMode.Union 
                );

            while (!_loadHandle.IsDone)
            {
                await Task.Delay(50);
            }
        }
        
        private async Task GetChefsData()
        {
            _storeChefs = await DataLoader.LoadAllStoreChefs();

            List<Task> loadChefSettings = new List<Task>();

            foreach (var chefData in _storeChefs)
            {
                var loadOperation= chefData.LoadAssetRef();
                loadChefSettings.Add(loadOperation);
            }
            
            while (loadChefSettings.Count > 0)
            {
                Task loadTaskComplete = await Task.WhenAny(loadChefSettings);
                loadChefSettings.Remove(loadTaskComplete);
            }
        }

        private async Task LoadAssetReferences()
        {
            _chefItemOperationHandle = _chefItemAssetRef.LoadAssetAsync<GameObject>();
            _chefItemOperationHandle.Completed += _handle => _chefItemPrefab = _handle.Result;
            
            _recipeItemOperationHandle = _recipeItemAssetRef.LoadAssetAsync<GameObject>();
            _recipeItemOperationHandle.Completed += _handle => _recipeItemPrefab = _handle.Result;

            while (!_chefItemOperationHandle.IsDone || !_recipeItemOperationHandle.IsDone)
            {
                await Task.Delay(50);
            }
        }

        private void OnStoreItemClicked(StoreItem _storeItem)
        {
            if (_selectedStoreItem == null)
            {
                _selectedStoreItem = _storeItem;
                _selectedStoreItem.SelectItem();
                return;
            }

            if (_selectedStoreItem == _storeItem)
            {
                _selectedStoreItem.DeselectItem();
                _selectedStoreItem = null;
            }
            else
            {
                _selectedStoreItem.DeselectItem();
                _selectedStoreItem = _storeItem;
                _selectedStoreItem.SelectItem();
            }
        }

        private void PurchaseChefItem(StoreChefItem _chefItem)
        {
            if (GameManager.Instance.PlayerDataContainer.Currencies.Pay(ECurrencyType.SoftCurrency, _chefItem.Price) ==
                false) return;
            DataLoader.RemoveStoreChefItem(_chefItem.ChefData);
            GameManager.Instance.PlayerDataContainer.AddChefToPlayerInventory(_chefItem.ChefData);
            _chefsInStore.Remove(_chefItem);
            Destroy(_chefItem.gameObject);
            SortItems();
        }

        private void PurchaseRecipeItem(StoreRecipeItem _recipeItem)
        {
            if (GameManager.Instance.PlayerDataContainer.Currencies.Pay(ECurrencyType.SoftCurrency, _recipeItem.Price) ==
                false) return;
            
            _recipes.Remove(_recipeItem.Recipe);
            var recipeList = _recipes.Select(x => x.name).ToList();
            DataLoader.SaveStoreRecipes(recipeList);
            GameManager.Instance.PlayerDataContainer.AddRecipeToPlayerInventory(_recipeItem.Recipe);
            _recipesInStore.Remove(_recipeItem);
            Destroy(_recipeItem.gameObject);
            SortItems();
        }
        
        private void OnDestroy()
        {
            Addressables.Release(_loadHandle);
            Addressables.Release(_chefItemOperationHandle);
            Addressables.Release(_recipeItemOperationHandle);
        }
        public void SortItems()
        {
            //Little hack to force copy the list and not update the current one as it would break the enumeration
            List<StoreChefItem> _arrayChefs = _chefsInStore.ToArray().ToList();
            List<StoreRecipeItem> _arrayRecipes = _recipesInStore.ToArray().ToList();

            _arrayChefs.Sort(SortingHelper.CompareByRarity);
            _arrayRecipes.Sort(SortingHelper.CompareByRarity);

            for (int i = 0; i < _chefsInStore.Count; ++i)
                _arrayChefs[i].transform.SetSiblingIndex(i);

            for (int i = 0; i < _recipesInStore.Count; ++i)
                _arrayRecipes[i].transform.SetSiblingIndex(i);
        }
    }
}