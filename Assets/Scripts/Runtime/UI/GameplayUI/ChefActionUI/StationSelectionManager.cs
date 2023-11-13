using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Managers.GameplayManager;
using Runtime.ScriptableObjects.DataContainers.Stations;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.UI.GameplayUI.ChefActionUI
{
    public class StationSelectionManager : MonoBehaviour
    {
        [SerializeField] 
        private RadialLayout _radialLayout;

        [SerializeField] 
        private Transform _popup;
        
        [SerializeField] 
        private StationManager _stationManager;

        [SerializeField] 
        private AssetReferenceT<GameObject> _stationSelectionButtonAssetRef;

        [SerializeField] 
        private Transform _inactiveButtonContainer;
        
        [SerializeField] 
        private bool _usePreInstantiatedStationSelectionButtons;

        [SerializeField]
        private List<StationSelectionButton> _stationSelectionButtons = new List<StationSelectionButton>();
        
        [Header("Broadcasting on")]
        [SerializeField] 
        private VoidEventChannel _onStationSelectionManagerInitialized;
        
        [SerializeField] 
        private VoidEventChannel _onStationSelectedEventChannel;
        
        [SerializeField] 
        private UnityEvent<StationSelectionButton> _onStationSelected;
        
        private readonly List<StationSelectionButton> _wheelCurrentButtons = new List<StationSelectionButton>();
        private AsyncOperationHandle<GameObject> _operationHandle;
        private GameObject _stationSelectionButtonPrefab;
        private RectTransform _rectTransform;
        private BaseStationAction[] _shiftStationActions;
        
        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
            _operationHandle = _stationSelectionButtonAssetRef.LoadAssetAsync<GameObject>();
            _operationHandle.Completed += OperationHandleOnCompleted;
        }

        private void OnDestroy()
        {
            Addressables.Release(_operationHandle);
        }

        private void OperationHandleOnCompleted(AsyncOperationHandle<GameObject> _obj)
        {
            _stationSelectionButtonPrefab = _obj.Result;
            StartCoroutine(InitializeCoroutine());
        }

        private IEnumerator InitializeCoroutine()
        {
            while (!_stationManager.StationsInitialized)
            {
                yield return null;
            }

            _shiftStationActions = GetAllStationActions();

            int count = 0;
            foreach (var stationManagerKitchenStation in _stationManager.KitchenStations)
            {
                StationSelectionButton stationSelectionButton;
                if (_usePreInstantiatedStationSelectionButtons)
                {
                    stationSelectionButton = _stationSelectionButtons[count];
                }

                else
                {
                    stationSelectionButton = Instantiate(_stationSelectionButtonPrefab, _inactiveButtonContainer).GetComponent<StationSelectionButton>();
                    _stationSelectionButtons.Add(stationSelectionButton);
                }
                
                stationSelectionButton.Initialize(stationManagerKitchenStation.Value[0].Action);
                stationSelectionButton.onStationActionClicked += OnStationSelected;
                HideButton(stationSelectionButton);
                count++;
            }
            HideStationSelectionWheel();
            Debug.Log("Station Selection Manager Initialized");
            _onStationSelectionManagerInitialized.RaiseEvent();
        }
        
        public void ShowStationSelectionWheel(IngredientSelectionButton _currentIngredientSelectionButton)
        {
            _popup.position = _currentIngredientSelectionButton.transform.position;
            ClearCurrentWheel();
            var availableStationActions = GetAvailableStations(_currentIngredientSelectionButton.Ingredient);
            var matchingButtons = GetMatchingStationSelectionButtons(availableStationActions);
            foreach (var selectionButton in matchingButtons)
            {
                DisplayButton(selectionButton);
            }
            AdaptRadialLayoutSetting(matchingButtons.Length);
            gameObject.SetActive(true);
        }

        private void AdaptRadialLayoutSetting(int _childCount)
        {
            switch (_childCount)
            {
                
                case 1:
                    _radialLayout.StartAngle = 90;
                    break;
                case 2:
                    _radialLayout.StartAngle = 30;
                    _radialLayout.MaxAngle = 120;
                    break;
                case 3:
                    _radialLayout.StartAngle = 30;
                    _radialLayout.MaxAngle = 120;
                    break;
            }
        }

        public void HideStationSelectionWheel()
        {
            gameObject.SetActive(false);
        }

        private void OnStationSelected(StationSelectionButton _stationSelectionButton)
        {
            _onStationSelected?.Invoke(_stationSelectionButton);
            _onStationSelectedEventChannel.RaiseEvent();
        }

        private void ClearCurrentWheel()
        {
            if (_wheelCurrentButtons.Count == 0) return;

            for (int i = _wheelCurrentButtons.Count - 1; i >= 0; i--)
            {
                HideButton(_wheelCurrentButtons[i]);
            }
        }

        private StationSelectionButton[] GetMatchingStationSelectionButtons(BaseStationAction[] _actions)
        {
            List<StationSelectionButton> matchingButtons = new List<StationSelectionButton>();
            foreach (var stationSelectionButton in _stationSelectionButtons)
            {
                if (_actions.Contains(stationSelectionButton.StationAction))
                {
                    matchingButtons.Add(stationSelectionButton);
                }
            }

            return matchingButtons.ToArray();
        }

        private void DisplayButton(StationSelectionButton _stationSelectionButton)
        {
            _stationSelectionButton.transform.SetParent(_radialLayout.transform, false);
            _stationSelectionButton.gameObject.SetActive(true);
            _wheelCurrentButtons.Add(_stationSelectionButton);
        }

        private void HideButton(StationSelectionButton _stationSelectionButton)
        {
            _stationSelectionButton.transform.SetParent(_inactiveButtonContainer, false);
            _stationSelectionButton.gameObject.SetActive(false);
            _wheelCurrentButtons.Remove(_stationSelectionButton);
        }
        
        private BaseStationAction[] GetAvailableStations(RawIngredient _ingredient)
        {
            if (_ingredient == null)
            {
                Debug.LogWarning("GetAvailableStation argument is null");
                return null;
            }
            List<BaseStationAction> stationActions = new List<BaseStationAction>();

            foreach (var stationAction in _shiftStationActions)
            {
                if (stationAction.CanProcessIngredient(_ingredient))
                {
                    stationActions.Add(stationAction);
                }
            }

            return stationActions.ToArray();
        }

        private BaseStationAction[] GetAllStationActions()
        {
            List<BaseStationAction> availableStationActions = new List<BaseStationAction>();

            foreach (var station in _stationManager.KitchenStations)
            {
                availableStationActions.Add(station.Value[0].Action);
            }

            return availableStationActions.ToArray();
        }
    }
}