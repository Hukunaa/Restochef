using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Runtime.Factory;
using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace Runtime.UI.MainMenuUI.CookMenuUI
{
    public class RecipeSelectionMenuUI : UIToolkitMonoBehavior
    {
        private Button _backButton;
        private Button _selectRecipesButton;
        private VisualElement _recipeContainer;

        private VisualElement _mandatorySlotsContainer;
        private VisualElement _optionalSlotsContainer;

        [SerializeField] private UnityEvent _onBackButtonClicked;
        [SerializeField] private UnityEvent _onSelectRecipesButtonClicked;
        
        private PlayerDataContainer _playerDataContainer;

        [SerializeField] private VisualTreeAsset _recipeTemplate;
        [SerializeField] private StyleSheet _slotStyleSheet;
        [SerializeField] private string _mandatorySlotClassName;
        [SerializeField] private string _optionalSlotClassName;
        
        private List<Recipe> _selectedRecipe = new List<Recipe>();
        private List<VisualElement> _slots = new List<VisualElement>();
        
        private bool _dataContainersLoaded;

        protected override void Awake()
        {
            base.Awake();
            StartCoroutine(LoadDataContainers());
        }

        private IEnumerator LoadDataContainers()
        {
            while (GameManager.Instance == null || !GameManager.Instance.DataLoaded)
            {
                yield return null;
            }

            _playerDataContainer = GameManager.Instance.PlayerDataContainer; 
            
            _dataContainersLoaded = true;
        }
        
        protected override void FindVisualElements()
        {
            _backButton = Root.Q<Button>("BackButton");
            _selectRecipesButton = Root.Q<Button>("SelectRecipesButton");
            _recipeContainer = Root.Q<VisualElement>("unity-content-container");
            _mandatorySlotsContainer = Root.Q<VisualElement>("MandatorySlotsContainer");
            _optionalSlotsContainer = Root.Q<VisualElement>("OptionalSlotsContainer");
        }

        protected override void BindButtons()
        {
            _backButton.clicked += _onBackButtonClicked.Invoke;
            _selectRecipesButton.clicked += _onSelectRecipesButtonClicked.Invoke;
        }

        public async Task Initialize()
        {
            _selectRecipesButton.SetEnabled(false);
            _selectedRecipe.Clear();
            
            while (_dataContainersLoaded == false)
            {
                await Task.Delay(50);
            }
            
            InitializeRecipeButtons();
            GenerateSlots();
        }

        private void GenerateSlots()
        {
            int mandatorySlotAmount = _playerDataContainer.SelectedKitchenData.minRecipeSlots;
            int optionalSlotAmount = _playerDataContainer.SelectedKitchenData.maxRecipeSlots -
                                     _playerDataContainer.SelectedKitchenData.minRecipeSlots;
            SlotFactory.InitializeSlots(ref _slots, _mandatorySlotsContainer, mandatorySlotAmount, _slotStyleSheet,
                _mandatorySlotClassName);
            SlotFactory.InitializeSlots(ref _slots, _optionalSlotsContainer, optionalSlotAmount, _slotStyleSheet,
                _optionalSlotClassName);
        }

        private void InitializeRecipeButtons()
        {
            _recipeContainer.Clear();
            
            foreach (var recipe in _playerDataContainer.PlayerRecipes)
            {
                var recipeButton = CreateRecipeButton(recipe);
                _recipeContainer.Add(recipeButton);
            }
        }

        private void OnRecipeButtonClicked(Recipe _recipe)
        {
            if (_selectedRecipe.Contains(_recipe))
            {
                _selectedRecipe.Remove(_recipe);
            }

            else
            {
                _selectedRecipe.Add(_recipe);
            }

            UpdateSlots();
            CheckSelectRecipesCondition();
            
            print($"{_recipe.RecipeName} button clicked");
        }

        private void CheckSelectRecipesCondition()
        {
            _selectRecipesButton.SetEnabled(_selectedRecipe.Count >= _playerDataContainer.SelectedKitchenData.minRecipeSlots);
        }

        private void UpdateSlots()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_selectedRecipe.Count > i)
                {
                    if (_slots[i].childCount == 0)
                    {
                        var slotContent = new VisualElement();
                        slotContent.name = "Content";
                        slotContent.AddToClassList("content");
                        slotContent.style.backgroundImage = _selectedRecipe[i].RecipeIcon.texture;
                    
                        _slots[i].Add(slotContent);
                    }

                    else
                    {
                        var visualElement = _slots[i].Query<VisualElement>("Content").First();
                        if (visualElement != null)
                        {
                            visualElement.style.backgroundImage = _selectedRecipe[i].RecipeIcon.texture;
                        }
                    }
                }

                else
                {
                    _slots[i].Clear();
                }
            }
        }

        private TemplateContainer CreateRecipeButton(Recipe _recipe)
        {
            var recipeVE = _recipeTemplate.Instantiate();

            var recipeButton = recipeVE.Q<Button>("RecipeButton");
            
            var recipeSprite = recipeVE.Q<VisualElement>("RecipeInfo");
            var recipePoints = recipeVE.Q<Label>("PointsAmount");
            var recipeName = recipeVE.Q<Label>("RecipeName");

            recipeSprite.style.backgroundImage = _recipe.RecipeIcon.texture;
            recipePoints.text = _recipe.RecipePoints.ToString();
            recipeName.text = _recipe.RecipeName;
            
            recipeButton.clicked += () =>
            {
                if (_selectedRecipe.Contains(_recipe))
                {
                    recipeSprite.RemoveFromClassList("selected");
                }

                else
                {
                    recipeSprite.AddToClassList("selected");
                }
                
                OnRecipeButtonClicked(_recipe);
            };

            return recipeVE;
        }

        /*
        public void SaveSelectedRecipes()
        {
            if (_shiftDataContainer == null)
            {
                Debug.LogError("Shift Data Container was not loaded correctly.");
            }
            
            _shiftDataContainer.SetShiftRecipes(_selectedRecipe.ToArray());
        }
        */

        /*private void OnDestroy()
        {
            Addressables.Release(_playerDataContainerLoadHandle);
            Addressables.Release(_shiftDataContainer);
        }*/
    }
}