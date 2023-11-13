/*using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Runtime.UI.MainMenuUI.CookMenuUI
{
    public class ShiftPreparationMenu : MonoBehaviour
    {
        [SerializeField] private RecipeSelectionMenuUI _recipeSelectionMenu;
        [SerializeField] private ChefSelectionMenuUI _chefSelectionMenu;

        [SerializeField] private UnityEvent _onShiftPreparationComplete;
        
        public void InitializeMenu()
        {
            _recipeSelectionMenu.Initialize();
            _chefSelectionMenu.Initialize();
        }

        public void DisplayMenu()
        {
            DisplayRecipeSelectionTab();
        }

        public void HideMenu()
        {
            _chefSelectionMenu.HideUI();
            _recipeSelectionMenu.HideUI();
        }

        public void DisplayRecipeSelectionTab()
        {
            _chefSelectionMenu.HideUI();
            _recipeSelectionMenu.DisplayUI();
        }

        public void DisplayChefSelectionTab()
        {
            _recipeSelectionMenu.HideUI();
            _chefSelectionMenu.DisplayUI();
        }

        public void FinishShiftPreparation()
        {
            _recipeSelectionMenu.SaveSelectedRecipes();
            _chefSelectionMenu.SaveSelectedChefs();

            _onShiftPreparationComplete?.Invoke();
        }
    }
}*/