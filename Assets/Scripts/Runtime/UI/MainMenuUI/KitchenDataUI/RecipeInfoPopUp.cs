using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using DG.Tweening;
using Runtime.ScriptableObjects.DataContainers;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.UI.MainMenuUI.KitchenDataUI
{
    public class RecipeInfoPopUp : InfoPopUp
    {
        [SerializeField] private RectTransform _popup;
        [SerializeField] private TMP_Text _recipeNameText;
        [SerializeField] private Image _recipeImage;
        [SerializeField] private TMP_Text _recipeScore;
        
        [Header("Asset References")]
        [SerializeField] private AssetReferenceT<GameObject> _ingredientInfoAssetRef;
        [SerializeField] private GameObject _recipeIngredientsContainer;
        [SerializeField] private RarityColors _rarityColors;
        
        [Header("Listening to")]
        [SerializeField] private RecipeEventChannel _onShowRecipeInfoEventChannel;

        private Recipe _currentRecipe;
        private GameObject _ingredientInfoPrefab;
        private float _popupYOffset;
        private List<GameObject> _ingredientInfoObjects;
        private AsyncOperationHandle<GameObject> _loadHandle;

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

        private void ShowRecipeInfo(Recipe _recipe)
        {
            _currentRecipe = _recipe;
            InitialiseUI();
            ShowPopUp();
            _popup.GetComponent<Image>().color = _rarityColors.Values[(int)_currentRecipe.Rarity];
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
            _recipeNameText.text = _currentRecipe.RecipeName;
            _recipeImage.sprite = _currentRecipe.RecipeIcon;
            _recipeScore.text = _currentRecipe.RecipePoints.ToString();

            if (_ingredientInfoPrefab == null)
            {
                Debug.LogError("No Ingredient Info prefab selected.");
                return;
            }

            foreach(RecipeIngredients ingredient in _currentRecipe.RecipeIngredients)
            {
                for(int i = 0; i < ingredient.Quantity; ++i)
                {
                    IngredientInfo instance = Instantiate(_ingredientInfoPrefab, _recipeIngredientsContainer.transform).GetComponent<IngredientInfo>();
                    ProcessedIngredient processed = (ProcessedIngredient)ingredient.Ingredient;
                    instance.SetIngredient(processed.IngredientIcon, processed.IngredientMix.StationAction.StationIcon);
                    _ingredientInfoObjects.Add(instance.gameObject);
                }
            }
        }
    }
}