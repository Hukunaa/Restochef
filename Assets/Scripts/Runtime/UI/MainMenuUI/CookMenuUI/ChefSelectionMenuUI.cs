/*using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Runtime.DataContainers.Stats;
using Runtime.Factory;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace Runtime.UI.MainMenuUI.CookMenuUI
{
    public class ChefSelectionMenuUI : UIToolkitMonoBehavior
    {
        private Button _backButton;
        private Button _selectChefsButton;
        private VisualElement _chefButtonsContainer;
        private VisualElement _mandatorySlotsContainer;
        private VisualElement _optionalSlotsContainer;
        
        [SerializeField] private AssetReferenceT<PlayerDataContainer> _playerDataContainerAssetRef;
        private PlayerDataContainer _playerDataContainer;
        
        [SerializeField] private AssetReferenceT<ShiftDataContainer> _shiftDataContainerAssetRef;
        private ShiftDataContainer _shiftDataContainer;
        
        private AsyncOperationHandle<PlayerDataContainer> _playerDataContainerLoadHandle;
        private AsyncOperationHandle<ShiftDataContainer> _shiftDataContainerLoadHandle;
        private bool _dataContainersLoaded;

        [SerializeField] private VisualTreeAsset _chefButtonAsset;
        [SerializeField] private VisualTreeAsset _statAsset;
        
        [SerializeField] private StyleSheet _slotStyleSheet;
        [SerializeField] private string _mandatorySlotClassName;
        [SerializeField] private string _optionalSlotClassName;
        
        private List<VisualElement> _slots = new List<VisualElement>();
        private List<ChefData> _selectedChefs = new List<ChefData>();

        [SerializeField] private UnityEvent _onBackButtonClicked;
        [SerializeField] private UnityEvent _onSelectChefsButtonClicked;
        
        [SerializeField] private bool _logDebugMessage = true;

        protected override void FindVisualElements()
        {
            _backButton = Root.Q<Button>("BackButton");
            _selectChefsButton = Root.Q<Button>("SelectChefsButton");
            _chefButtonsContainer = Root.Q<VisualElement>("unity-content-container");
            _mandatorySlotsContainer = Root.Q<VisualElement>("MandatorySlotsContainer");
            _optionalSlotsContainer = Root.Q<VisualElement>("OptionalSlotsContainer");
        }

        protected override void BindButtons()
        {
            _backButton.clicked += _onBackButtonClicked.Invoke;
            _selectChefsButton.clicked += _onSelectChefsButtonClicked.Invoke;
        }

        protected override void Awake()
        {
            base.Awake();
            StartCoroutine(LoadDataContainers());
        }

        private IEnumerator LoadDataContainers()
        {
            _playerDataContainerLoadHandle = _playerDataContainerAssetRef.LoadAssetAsync<PlayerDataContainer>();
            _playerDataContainerLoadHandle.Completed += _handle => _playerDataContainer = _handle.Result;
            yield return _playerDataContainerLoadHandle;

            _shiftDataContainerLoadHandle = _shiftDataContainerAssetRef.LoadAssetAsync<ShiftDataContainer>();
            _shiftDataContainerLoadHandle.Completed += _handle => _shiftDataContainer = _handle.Result;
            yield return _shiftDataContainerLoadHandle;
            
            _dataContainersLoaded = true;
        }

        public async Task Initialize()
        {
            _selectChefsButton.SetEnabled(false);
            
            while (_dataContainersLoaded == false)
            {
                await Task.Delay(50);
            }
            
            InitializeChefButtons();

            GenerateSlots();
        }

        private void GenerateSlots()
        {
            int _mandatorySlotAmount = _playerDataContainer.PlayerKitchenData.minChefSlots;
            int _optionamSlotAmount =
                _playerDataContainer.PlayerKitchenData.maxChefSlots - _playerDataContainer.PlayerKitchenData.minChefSlots;
            SlotFactory.InitializeSlots(ref _slots, _mandatorySlotsContainer, _mandatorySlotAmount, _slotStyleSheet,
                _mandatorySlotClassName);
            SlotFactory.InitializeSlots(ref _slots, _optionalSlotsContainer, _optionamSlotAmount, _slotStyleSheet,
                _optionalSlotClassName);
        }

        private void InitializeChefButtons()
        {
            _chefButtonsContainer.Clear();
            
            foreach (var chefData in _playerDataContainer.ChefsData)
            {
                var chefButton = CreateChefButton(chefData);
                _chefButtonsContainer.Add(chefButton);
            }
        }

        private void OnChefButtonClicked(ChefData _chefData)
        {
            DebugHelper.PrintDebugMessage($"{_chefData.ChefSettings.ChefName} clicked!", _logDebugMessage);
            if (_selectedChefs.Contains(_chefData))
            {
                _selectedChefs.Remove(_chefData);
            }

            else
            {
                _selectedChefs.Add(_chefData);
            }

            UpdateSlots();
            CheckSelectRecipesCondition();
        }
        
        private void CheckSelectRecipesCondition()
        {
            _selectChefsButton.SetEnabled(_selectedChefs.Count >= _playerDataContainer.PlayerKitchenData.minChefSlots);
        }

        private TemplateContainer CreateChefButton(ChefData _chefData)
        {
            var templateContainer = _chefButtonAsset.Instantiate();

            var chefButton = templateContainer.Q<Button>("ChefButton");
            var chefNameLabel = templateContainer.Q<Label>("ChefName");
            var chefLevel = templateContainer.Q<Label>("ChefLevel");
            var chefPortrait = templateContainer.Q<VisualElement>("ChefPortrait");
            var statContainer = templateContainer.Q<VisualElement>("StatsContainer");

            chefNameLabel.text = _chefData.ChefSettings.ChefName;
            chefLevel.text = _chefData.LevelData.Level.ToString();
            chefPortrait.style.backgroundImage = _chefData.ChefSettings.ChefHeadPortrait.texture;

            statContainer.Clear();
            foreach (var skill in _chefData.Skills)
            {
                var stat = _statAsset.Instantiate();
                var statName = stat.Q<Label>("StatName");
                var statValue = stat.Q<Label>("StatValue");

                statName.text = skill.Name;
                statValue.text = skill.Level.ToString();
                
                statContainer.Add(stat);
            }

            chefButton.clicked += () =>
            {
                if (_selectedChefs.Contains(_chefData))
                {
                    chefButton.RemoveFromClassList("selected");
                }

                else
                {
                    chefButton.AddToClassList("selected");
                }
                
                OnChefButtonClicked(_chefData);
            };

            return templateContainer;
        }
        
        private void UpdateSlots()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_selectedChefs.Count > i)
                {
                    if (_slots[i].childCount == 0)
                    {
                        var slotContent = new VisualElement();
                        slotContent.name = "Content";
                        slotContent.AddToClassList("content");
                        slotContent.style.backgroundImage = _selectedChefs[i].ChefSettings.ChefHeadPortrait.texture;
                    
                        _slots[i].Add(slotContent);
                    }

                    else
                    {
                        var visualElement = _slots[i].Query<VisualElement>("Content").First();
                        if (visualElement != null)
                        {
                            visualElement.style.backgroundImage = _selectedChefs[i].ChefSettings.ChefHeadPortrait.texture;
                        }
                    }
                }

                else
                {
                    _slots[i].Clear();
                }
            }
        }
        
        public void SaveSelectedChefs()
        {
            if (_shiftDataContainer == null)
            {
                Debug.LogError("Shift Data Container was not loaded correctly.");
            }
            
            _shiftDataContainer.SetShitChefs(_selectedChefs.ToArray());
        }
        
        private void OnDestroy()
        {
            Addressables.Release(_playerDataContainerLoadHandle);
            Addressables.Release(_shiftDataContainer);
        }
    }
}*/