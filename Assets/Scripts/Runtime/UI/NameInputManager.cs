using System;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.Utility;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class NameInputManager : MonoBehaviour
    {
        private PlayerDataContainer _playerDataContainer;

        [SerializeField] 
        private GameObject _nameInputGo;

        [SerializeField] 
        private TMP_InputField _inputField;
        
        [Header("Listening to")]
        [SerializeField]
        private VoidEventChannel _onGDRPConsentReady;

        [Header("Broadcasting on")]
        [SerializeField] 
        private VoidEventChannel _onNameInputReady;
        
        private void Awake()
        {
            _onGDRPConsentReady.onEventRaised += CheckName;
            HideNameInputUI();
        }

        private void OnDestroy()
        {
            _onGDRPConsentReady.onEventRaised -= CheckName;
        }
        
        private void CheckName()
        {
            DataLoader.CopyFromResourcesToPersistent();
            var playerName = DataLoader.LoadPlayerName();
            if (playerName == String.Empty)
            {
                ShowNameInputUI();
            }
            else
            {
                _onNameInputReady.RaiseEvent();
            }
        }

        private void HideNameInputUI()
        {
            _nameInputGo.SetActive(false);
        }

        private void ShowNameInputUI()
        {
            _nameInputGo.SetActive(true);
        }

        public void OnConfirmButtonClicked()
        {
            DataLoader.SavePlayerName(_inputField.text);
            HideNameInputUI();
            _onNameInputReady.RaiseEvent();
        }
    }
}