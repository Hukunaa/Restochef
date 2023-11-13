using System.Collections.Generic;
using Runtime.ScriptableObjects.Gameplay;
using UnityEngine;
using System.Linq;

namespace Runtime.UI.MainMenuUI.KitchenDataUI
{
    public class MenuManager : InventoryItemUIManager
    {
        private RecipeItemUI _recipeItemUIPrefab;

        private List<RecipeItemUI> _recipeItems = new List<RecipeItemUI>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _playerDataContainer.onRecipeAddedToCollection -= OnRecipeAddedToCollection;
        }

        protected override void OnPrefabLoaded(GameObject _prefab)
        {
            _recipeItemUIPrefab = _prefab.GetComponent<RecipeItemUI>();
        }

        protected override void CreateItems()
        {
            foreach (var recipe in _playerDataContainer.PlayerRecipes)
            {
                CreateRecipeItem(recipe);
            }
            
            _playerDataContainer.onRecipeAddedToCollection += OnRecipeAddedToCollection;
            SortItems();
        }

        private void CreateRecipeItem(Recipe _recipe)
        {
            var recipeItemUI = Instantiate(_recipeItemUIPrefab, _unusedItemContainer, false);
            recipeItemUI.SetInventoryItemManager(this);
            recipeItemUI.Initialize(_recipe);
            _recipeItems.Add(recipeItemUI);
        }

        private void OnRecipeAddedToCollection(Recipe _recipe)
        {
            CreateRecipeItem(_recipe);
        }

        protected override void SetSavedConfig()
        {
            foreach (var recipeItem in _recipeItems)
            {
                if (_playerDataContainer.SelectedKitchenData.menu.Contains(recipeItem.Recipe.name))
                {
                    recipeItem.UseItem();
                }
            }
        }

        public override void OnItemAdded(InventoryItemUI _item)
        {
            var recipeItem = (RecipeItemUI)_item;
            _playerDataContainer.SelectedKitchenData.AddRecipeToMenu(recipeItem.Recipe);
            SortItems();
        }

        public override void OnItemRemoved(InventoryItemUI _item)
        {
            var recipeItem = (RecipeItemUI)_item;
            _playerDataContainer.SelectedKitchenData.RemoveRecipeFromMenu(recipeItem.Recipe);
            SortItems();
        }

        public void SortItems()
        {
            //Little hack to force copy the list and not update the current one as it would break the enumeration
            List<RecipeItemUI> _array = _recipeItems.ToArray().ToList();

            _array.Sort(Utility.SortingHelper.CompareByRarity);
            for (int i = 0; i < _recipeItems.Count; ++i)
            {
                _array[i].transform.SetSiblingIndex(i);
            }
        }
    }
}