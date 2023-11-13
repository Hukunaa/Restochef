using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.DataContainers.Stats;
using Runtime.Enums;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using Runtime.Utility;
using ScriptableObjects.DataContainers;
using UnityEngine;

namespace Runtime.DataContainers.Player
{
    [Serializable]
    public class KitchenData
    {
        private PlayerDataContainer _playerDataContainer;
        
        public string kitchenLayoutName;
        
        public KitchenTile[,] tiles;

        public int kitchenSizeX;
        public int kitchenSizeY;
        
        public int minRecipeSlots;
        public int maxRecipeSlots;
        public int minChefSlots;
        public int maxChefSlots;

        public int maxStationSlots;
        public int maxStationLevel;
        public int kitchenStars;
        public int kitchenRank;
        public int bonusPoints;

        public List<string> brigade;
        public List<string> menu;

        public List<ChefData> _brigadeChefs;
        public List<Recipe> _menuRecipes;
        
        private HashSet<RawIngredient> _requiredRawIngredients = new HashSet<RawIngredient>();
        private HashSet<IngredientMix> _requiredIngredientMix = new HashSet<IngredientMix>();

        public event Action OnXPValueChanged;
        public event Action OnMenuChanged;
        public event Action OnBrigadeChanged;

        public void SetPlayerDataContainer(PlayerDataContainer _playerDataContainer)
        {
            this._playerDataContainer = _playerDataContainer;
            OnXPValueChanged?.Invoke();
        }

        public void AddChefToBrigade(ChefData _chef)
        {
            brigade ??= new List<string>();

            if (brigade.Contains(_chef.ChefID)) return;
            
            brigade.Add(_chef.ChefID);
            _brigadeChefs.Add(_chef);
            SaveKitchenData();
            OnBrigadeChanged?.Invoke();
        }

        public void RemoveChefFromBrigade(ChefData _chef)
        {
            if(brigade == null || !brigade.Contains(_chef.ChefID)) return;

            brigade.Remove(_chef.ChefID);
            _brigadeChefs.Remove(_chef);
            SaveKitchenData();
            OnBrigadeChanged?.Invoke();
        }

        public void AddRecipeToMenu(Recipe _recipe)
        {
            menu ??= new List<string>();

            if (menu.Contains(_recipe.name)) return;
            
            menu.Add(_recipe.name);
            _menuRecipes.Add(_recipe);
            GetIngredientTree();
            SaveKitchenData();
            OnMenuChanged?.Invoke();
        }

        public void RemoveRecipeFromMenu(Recipe _recipe)
        {
            if(menu == null || !menu.Contains(_recipe.name)) return;

            menu.Remove(_recipe.name);
            _menuRecipes.Remove(_recipe);
            GetIngredientTree();
            SaveKitchenData();
            OnMenuChanged?.Invoke();
        }

        public void UpdateKitchenData()
        {
            kitchenRank = _playerDataContainer.GetKitchenRank() + 1;
            RewardItem _rankItem = _playerDataContainer.KitchenRanks[kitchenRank - 1];
            maxStationSlots = _rankItem.KitchenRankReward.MaxStations;
            maxChefSlots = _rankItem.KitchenRankReward.MaxChefs;
            maxStationLevel = _rankItem.KitchenRankReward.MaxStationsLVL;
            maxRecipeSlots = _rankItem.KitchenRankReward.MenuSize;
            bonusPoints = _rankItem.KitchenRankReward.BonusPoints;
            SaveKitchenData();
        }

        public void AddKitchenXP(int _amount)
        {
            if (_amount <= 0)
                return;

            kitchenStars += _amount;
            UpdateKitchenData();
            OnXPValueChanged?.Invoke();
        }

        private void SaveKitchenData()
        {
            DataLoader.SaveKitchenData(this);
        }

        public void RetrieveBrigadeChefs()
        {
            if (_brigadeChefs != null)
            {
                _brigadeChefs.Clear();
            }
            else
            {
                _brigadeChefs = new List<ChefData>();
            }
            
            foreach (var chefID in brigade)
            {
                var chef = _playerDataContainer.PlayerChefs.FirstOrDefault(x => x.ChefID == chefID);
                if (chef == null)
                {
                    DebugHelper.PrintDebugMessage($"No chef found with ChefID {chefID}");
                    continue;
                }
                if (_brigadeChefs.Contains(chef)) continue;
                _brigadeChefs.Add(chef);
            }
        }

        public void RetrieveRecipes()
        {
            if (_menuRecipes != null)
            {
                _menuRecipes.Clear();
            }
            else
            {
                _menuRecipes = new List<Recipe>();
            }
            
            foreach (var menuEntry in menu)
            {
                var recipe = _playerDataContainer.PlayerRecipes.First(x => x.name == menuEntry);
                if (_menuRecipes.Contains(recipe)) continue;
                _menuRecipes.Add(recipe);
            }
        }
        
        public void GetIngredientTree()
        {
            _requiredRawIngredients.Clear();
            _requiredIngredientMix.Clear();

            foreach (var recipe in _menuRecipes)
            {
                foreach (var recipeIngredient in recipe.RecipeIngredients)
                {
                    SetIngredientInfo(recipeIngredient.Ingredient);
                }
            }
        }

        private void SetIngredientInfo(Ingredient _ingredient)
        {
            switch (_ingredient.IngredientType)
            {
                case EIngredientType.RawIngredient:
                    var rawIngredient = (RawIngredient)_ingredient;
                    _requiredRawIngredients.Add(rawIngredient);
                    break;
                case EIngredientType.ProcessedIngredient:
                    var processedIngredient = (ProcessedIngredient)_ingredient;
                    _requiredIngredientMix.Add(processedIngredient.IngredientMix);
                    SetIngredientInfo(processedIngredient.IngredientMix.Input);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public HashSet<RawIngredient> RawIngredients => _requiredRawIngredients;
        public HashSet<IngredientMix> IngredientMixes => _requiredIngredientMix;
    }
}