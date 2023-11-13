using System.Collections.Generic;
using DG.Tweening;
using Runtime.Enums;
using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using Runtime.UI.MainMenuUI.KitchenDataUI;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Runtime.UI.MainMenuUI.StoreUI
{
    public class StoreRecipeItemInfo : StoreItemInfo
    {
        [SerializeField] 
        private RectTransform _popup;
        
        [SerializeField] 
        private TMP_Text _recipeNameText;
       
        [SerializeField] 
        private Image _recipeImage;
      
        [SerializeField] 
        private Image _cardBackground;
      
        [SerializeField] 
        private TMP_Text _recipeScore;
        
        [Header("Asset Ref")]
        [SerializeField] 
        private AssetReferenceT<GameObject> _ingredientInfoAssetRef;
        
        [SerializeField] 
        private GameObject _recipeIngredientsContainer;
        
        [SerializeField] 
        private RarityColors _rarityColors;
        
        [Header("Listening to")]
        [SerializeField] 
        private StoreRecipeItemEventChannel _onShowRecipeInfoEventChannel;

        private StoreRecipeItem _currentRecipeItem;
        private List<GameObject> _ingredientInfoObjects;
        private float _popupYOffset;
        private AsyncOperationHandle<GameObject> _loadHandle;
        private GameObject _ingredientInfoPrefab;

        protected override void Awake()
        {
            base.Awake();
            _ingredientInfoObjects = new List<GameObject>();
            _onShowRecipeInfoEventChannel.onEventRaised += ShowRecipeInfo;
            _popupYOffset = _popup.anchoredPosition.y;

            _loadHandle = _ingredientInfoAssetRef.LoadAssetAsync<GameObject>();
            _loadHandle.Completed += LoadHandleOnCompleted;
        }

        private void LoadHandleOnCompleted(AsyncOperationHandle<GameObject> _obj)
        {
            _ingredientInfoPrefab = _obj.Result;
        }

        private void OnDestroy()
        {
            _onShowRecipeInfoEventChannel.onEventRaised -= ShowRecipeInfo;
            Addressables.Release(_loadHandle);
        }

        private void ShowRecipeInfo(StoreRecipeItem _recipe)
        {
            _currentRecipeItem = _recipe;
            InitialiseUI();
            ShowPopUp();
            _cardBackground.color = _rarityColors.Values[(int)_currentRecipeItem.Recipe.Rarity];
            _popup.DOAnchorPosY(-1000, 0.0f);
            _popup.DOAnchorPosY(_popupYOffset, 0.2f);
        }

        public override void HidePopUp()
        {
            _ingredientInfoObjects.ForEach(o => Destroy(o));
            _ingredientInfoObjects.Clear();
            _popup.DOAnchorPosY(-1000, 0.2f).OnComplete(() => base.HidePopUp());
        }

        private void InitialiseUI()
        {
            var recipe = _currentRecipeItem.Recipe;
            _recipeNameText.text = recipe.RecipeName;
            _recipeImage.sprite = recipe.RecipeIcon;
            _recipeScore.text = recipe.RecipePoints.ToString();
            
            string priceText = _currentRecipeItem.Price == 0 ? FreeItemText : _currentRecipeItem.Price.ToString();
            _priceText.text = priceText;
            _purchaseButton.interactable = CanPay();
            
            foreach(RecipeIngredients _ingredient in recipe.RecipeIngredients)
            {
                IngredientInfo _instance = Instantiate(_ingredientInfoPrefab, _recipeIngredientsContainer.transform).GetComponent<IngredientInfo>();
                ProcessedIngredient _processed = (ProcessedIngredient)_ingredient.Ingredient;
                _instance.SetIngredient(_processed.IngredientIcon, _processed.IngredientMix.StationAction.StationIcon);
                _ingredientInfoObjects.Add(_instance.gameObject);
            }
        }
        
        public override void OnBuyButtonClicked()
        {
            _currentRecipeItem.PurchaseItem();
            HidePopUp();
        }

        protected override bool CanPay()
        {
            return GameManager.Instance.PlayerDataContainer.Currencies.CanPay(ECurrencyType.SoftCurrency,
                _currentRecipeItem.Price);
        }
    }
}