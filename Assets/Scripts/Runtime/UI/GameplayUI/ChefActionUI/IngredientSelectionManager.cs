using System.Collections;
using System.Collections.Generic;
using Runtime.Gameplay;
using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.UI.GameplayUI.ChefActionUI
{
    public class IngredientSelectionManager : MonoBehaviour
    {
        [SerializeField] 
        private AssetReferenceT<GameObject> _ingredientSelectionButtonAssetRef;

        [SerializeField] 
        private VoidEventChannel _onIngredientSelectedEventChannel;
        
        [SerializeField] 
        private UnityEvent<IngredientSelectionButton> _onIngredientSelected;
        
        [SerializeField] 
        private UnityEvent _onIngredientDeselected;

        [SerializeField] 
        private Transform _ingredientContainer;

        [SerializeField] 
        private bool _usePreInstantiatedIngredientSelectionButtons;
        
        [SerializeField]
        private List<IngredientSelectionButton> _ingredientSelectionButtons = new List<IngredientSelectionButton>();
        private IngredientSelectionButton _selectedIngredientButton;
        private AsyncOperationHandle<GameObject> _operationHandle;
        private GameObject _ingredientSelectionButtonPrefab;
        private PlayerDataContainer _playerDataContainer;
        private Station[] _stations;

        private void Awake()
        {
            _operationHandle = _ingredientSelectionButtonAssetRef.LoadAssetAsync();
            _operationHandle.Completed += OnAssetRefLoaded;
        }

        private void OnDestroy()
        {
            Addressables.Release(_operationHandle);
        }

        private void OnAssetRefLoaded(AsyncOperationHandle<GameObject> _obj)
        {
            _ingredientSelectionButtonPrefab = _obj.Result;
            StartCoroutine(CreateIngredientSelectionButtonsCoroutine());
        }

        private IEnumerator CreateIngredientSelectionButtonsCoroutine()
        {
            while (GameManager.Instance == null || !GameManager.Instance.DataLoaded)
            {
                yield return null;
            }

            _playerDataContainer = GameManager.Instance.PlayerDataContainer;
            
            CreateIngredientSelectionButtons();
            
            Hide();
        }

        private void CreateIngredientSelectionButtons()
        {
            int count = 0;
            foreach (var ingredient in _playerDataContainer.SelectedKitchenData.RawIngredients)
            {
                IngredientSelectionButton ingredientSelectionButton;
                if (_usePreInstantiatedIngredientSelectionButtons)
                {
                    ingredientSelectionButton = _ingredientSelectionButtons[count];
                }

                else
                {
                    ingredientSelectionButton = Instantiate(_ingredientSelectionButtonPrefab, _ingredientContainer).GetComponent<IngredientSelectionButton>();
                    _ingredientSelectionButtons.Add(ingredientSelectionButton);
                }
                
                ingredientSelectionButton.SetIngredient(ingredient);
                ingredientSelectionButton._onIngredientSelected += OnIngredientButtonClicked;

                count++;
            }
        }

        private void OnIngredientButtonClicked(IngredientSelectionButton _ingredientButton)
        {
            if (_selectedIngredientButton == _ingredientButton)
            {
                DeselectIngredient();
                return;
            }
            SelectIngredient(_ingredientButton);
        }

        private void SelectIngredient(IngredientSelectionButton _ingredientButton)
        {
            _selectedIngredientButton = _ingredientButton;
            _onIngredientSelected?.Invoke(_ingredientButton);
            _onIngredientSelectedEventChannel.RaiseEvent();
            DisableUnselectedIngredients();
        }
        
        public void DeselectIngredient()
        {
            _selectedIngredientButton = null;
            EnableAllIngredientButtons();
            _onIngredientDeselected?.Invoke();
        }

        private void DisableUnselectedIngredients()
        {
            foreach (var ingredientButton in _ingredientSelectionButtons)
            {
                if(ingredientButton == _selectedIngredientButton) continue;
                ingredientButton.SetIngredientButtonInactive();
            }
        }

        private void EnableAllIngredientButtons()
        {
            foreach (var ingredientButton in _ingredientSelectionButtons)
            {
                ingredientButton.SetIngredientButtonActive();
            }
        }
        
        public void Hide()
        {
            _selectedIngredientButton = null;
            gameObject.SetActive(false);
        }

        public void Display()
        {
            EnableAllIngredientButtons();
            gameObject.SetActive(true);
        }

    }
}