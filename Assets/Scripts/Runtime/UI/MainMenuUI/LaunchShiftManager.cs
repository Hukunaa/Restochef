using System;
using System.Collections;
using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.UI.MainMenuUI
{
    public class LaunchShiftManager : MonoBehaviour
    {
        [SerializeField] 
        private UnityEvent _launchShift;
        
        [SerializeField] 
        private TMP_Text _shiftNotReadyText;

        [SerializeField] 
        private TMP_Text _menuInfoText;
        
        [SerializeField] 
        private TMP_Text _brigadeInfoText;

        [SerializeField] 
        private Color _conditionMetColor = Color.green;
        
        [SerializeField] 
        private Color _conditionNotMetColor = Color.red;
        
        [SerializeField] 
        private string _brigadeNotReadyMessage;
        
        [SerializeField] 
        private string _menuNotReadyMessage;
        
        [SerializeField] 
        private string _brigadeAndMenuNotReadyMessage;

        [SerializeField] 
        private float _messageDuration = 2;

        private Coroutine _shiftNotReadyMessageCoroutine;

        private PlayerDataContainer _playerDataContainer;

        private IEnumerator Initialize()
        {
            while (GameManager.Instance == null || !GameManager.Instance.DataLoaded)
            {
                yield return null;
            }

            _playerDataContainer = GameManager.Instance.PlayerDataContainer;
            
            UpdateMenuInfo();
            UpdateBrigadeInfo();

            _playerDataContainer.SelectedKitchenData.OnMenuChanged += UpdateMenuInfo;
            _playerDataContainer.SelectedKitchenData.OnBrigadeChanged += UpdateBrigadeInfo;
        }

        private void UpdateMenuInfo()
        {
            var recipeCount = _playerDataContainer.SelectedKitchenData._menuRecipes.Count;
            var minSlots = _playerDataContainer.SelectedKitchenData.minRecipeSlots;
            var maxSlots = _playerDataContainer.SelectedKitchenData.maxRecipeSlots;
            _menuInfoText.text =
                $"{recipeCount}/{maxSlots}";
            _menuInfoText.color = recipeCount >= minSlots ? _conditionMetColor : _conditionNotMetColor;
        }

        private void UpdateBrigadeInfo()
        {
            var brigadeCount = _playerDataContainer.SelectedKitchenData._brigadeChefs.Count;
            var minSlots = _playerDataContainer.SelectedKitchenData.minChefSlots;
            var maxSlots = _playerDataContainer.SelectedKitchenData.maxChefSlots;
            _brigadeInfoText.text =
                $"{brigadeCount}/{maxSlots}";
            _brigadeInfoText.color = brigadeCount >= minSlots ? _conditionMetColor : _conditionNotMetColor;
        }

        private void Awake()
        {
            StartCoroutine(Initialize());
        }

        private void OnDestroy()
        {
            _playerDataContainer.SelectedKitchenData.OnMenuChanged -= UpdateMenuInfo;
            _playerDataContainer.SelectedKitchenData.OnBrigadeChanged -= UpdateBrigadeInfo;
        }

        private void Start()
        {
            HideMessage();
        }

        private void OnDisable()
        {
            if (_shiftNotReadyMessageCoroutine != null)
            {
                StopCoroutine(_shiftNotReadyMessageCoroutine);
                _shiftNotReadyMessageCoroutine = null;
            }
            
            HideMessage();
        }

        public void OnLaunchShiftButtonClicked()
        {
            var brigadeReady = IsBrigadeReady();
            var menuReady = IsMenuReady();
            
            if (brigadeReady && menuReady)
            {
                _launchShift?.Invoke();
            }

            else
            {
                if (!brigadeReady && !menuReady)
                {
                    ShowShiftNotReadyMessage(_brigadeAndMenuNotReadyMessage);
                    return;
                }

                if (!brigadeReady)
                {
                    ShowShiftNotReadyMessage(_brigadeNotReadyMessage);
                    return;
                }
                
                ShowShiftNotReadyMessage(_menuNotReadyMessage);
            }
        }

        private void ShowShiftNotReadyMessage(string _message)
        {
            if (_shiftNotReadyMessageCoroutine != null)
            {
                StopCoroutine(_shiftNotReadyMessageCoroutine);
            }
            
            _shiftNotReadyMessageCoroutine = StartCoroutine(ShowShiftNotReadyMessageCoroutine(_message));
        }

        private IEnumerator ShowShiftNotReadyMessageCoroutine(string _message)
        {
            _shiftNotReadyText.text = _message;
            ShowMessage();

            yield return new WaitForSeconds(_messageDuration);
            
            HideMessage();
            _shiftNotReadyMessageCoroutine = null;
        }

        private void ShowMessage()
        {
            _shiftNotReadyText.enabled = true;
        }

        private void HideMessage()
        {
            _shiftNotReadyText.enabled = false;
        }

        private bool IsBrigadeReady()
        {
            var kitchenData = GameManager.Instance.PlayerDataContainer.SelectedKitchenData;
            return kitchenData._brigadeChefs.Count >= kitchenData.minChefSlots;
           
        }

        private bool IsMenuReady()
        {
            var kitchenData = GameManager.Instance.PlayerDataContainer.SelectedKitchenData;
            return kitchenData.menu.Count >= kitchenData.minRecipeSlots;
        }
    }
}