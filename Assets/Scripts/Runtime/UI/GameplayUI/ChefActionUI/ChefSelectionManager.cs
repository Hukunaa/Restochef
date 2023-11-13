using System.Collections;
using System.Collections.Generic;
using Runtime.Managers.GameplayManager;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.UI.GameplayUI.ChefActionUI
{
    public class ChefSelectionManager : MonoBehaviour
    {
        [SerializeField]
        private ChefManager _chefManager;

        [SerializeField] 
        private AssetReferenceT<GameObject> _chefSelectionButtonAssetRef;

        [SerializeField] 
        private Transform _chefSelectionButtonContainer;

        [SerializeField] 
        private VoidEventChannel _onShiftStarted;

        [SerializeField] 
        private VoidEventChannel _onShiftEnded;

        [SerializeField] 
        private VoidEventChannel _onChefSelectedEventChannel;
        
        [SerializeField] 
        private UnityEvent<ChefSelectionButton> _onChefSelected;

        [SerializeField]
        private UnityEvent _onChefDeselected;

        [SerializeField] 
        private bool _useReferencedCards;
        
        [SerializeField]
        private List<ChefSelectionButton> _chefSelectionButtons = new List<ChefSelectionButton>();

        [SerializeField] 
        private bool _hideAfterInitialization;
        
        private AsyncOperationHandle<GameObject> _operationHandle;
        private GameObject _chefSelectionButtonPrefab;

        private ChefSelectionButton _activeChefSelectionButton;

        private void Awake()
        {
            _operationHandle = _chefSelectionButtonAssetRef.LoadAssetAsync<GameObject>();
            _operationHandle.Completed += OnChefSelectionButtonAssetLoaded;
            _onShiftStarted.onEventRaised += Display;
            _onShiftEnded.onEventRaised += OnShiftEnd;
        }

        private void OnShiftEnd()
        {
            DeselectCurrentChef();
            Hide();
        }

        private void OnDestroy()
        {
            Addressables.Release(_operationHandle);
            _onShiftStarted.onEventRaised -= Display;
            _onShiftEnded.onEventRaised -= OnShiftEnd;
        }

        private void OnChefSelectionButtonAssetLoaded(AsyncOperationHandle<GameObject> _obj)
        {
            _chefSelectionButtonPrefab = _obj.Result;
            StartCoroutine(InitializeChefPortraitCoroutine());
        }

        private IEnumerator InitializeChefPortraitCoroutine()
        {
            while (_chefManager.Initialized == false)
            {
                yield return null;
            }
            
            CreateChefSelectionButtons();

            if (!_hideAfterInitialization) yield break;
            
            Hide();
        }
        
        private void CreateChefSelectionButtons()
        {
            int count = 0;
            
            foreach (var chef in _chefManager.Chefs)
            {
                if (_useReferencedCards)
                {
                     var chefSelectionButton = _chefSelectionButtons[count];
                    {
                        chefSelectionButton.Initialize(chef);
                        chefSelectionButton.onChefButtonSelected += OnChefSelected;
                        chefSelectionButton.onChefButtonDeselected += OnChefDeselected;
                        count++;
                    }
                }

                else
                {
                    var instance = Instantiate(_chefSelectionButtonPrefab, _chefSelectionButtonContainer).GetComponent<ChefSelectionButton>();
                    instance.Initialize(chef);
                    instance.onChefButtonSelected += OnChefSelected;
                    instance.onChefButtonDeselected += OnChefDeselected;
                    _chefSelectionButtons.Add(instance);
                }
            }
        }

        private void OnChefSelected(ChefSelectionButton _chefSelectionButton)
        {
            if (_activeChefSelectionButton != null)
            {
                DeselectCurrentChef();
            }
            _activeChefSelectionButton = _chefSelectionButton;
            _onChefSelected?.Invoke(_chefSelectionButton);
            _onChefSelectedEventChannel.RaiseEvent();
        }

        private void OnChefDeselected(ChefSelectionButton _chefSelectionButton)
        {
            if (_activeChefSelectionButton != _chefSelectionButton) return;
            _activeChefSelectionButton = null;
            _onChefDeselected?.Invoke();
        }

        public void DeselectCurrentChef()
        {
            if (_activeChefSelectionButton == null) return;
            _activeChefSelectionButton.DeselectChef();
        }
        
        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Display()
        {
            gameObject.SetActive(true);
        }
    }
}