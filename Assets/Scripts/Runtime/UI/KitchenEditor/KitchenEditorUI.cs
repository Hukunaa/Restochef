using Runtime.DataContainers;
using Runtime.Gameplay;
using Runtime.Managers;
using Runtime.Managers.GameplayManager;
using Runtime.ScriptableObjects.EventChannels;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Runtime.Utility;
using Runtime.ScriptableObjects.DataContainers;
using DG.Tweening;
using System;
using TMPro;

namespace Runtime.UI.KitchenEditor
{
    public class KitchenEditorUI : MonoBehaviour
    {

        [SerializeField]
        private KitchenEditorManager _kitchenEditor;
        [SerializeField]
        private VoidEventChannel _onKitchenLoaded;
        [SerializeField]
        private VoidEventChannel _onStationEditSelect;
        [SerializeField]
        private GameObject _stationPopup;
        [SerializeField]
        private GameObject _upgradeConfirmPopup;
        [SerializeField]
        private KitchenCameraManager _kitchenCameraManager;
        [SerializeField]
        private GameObject _saveButton;
        [SerializeField]
        private SelectionPanel _selectionPanel;
        [SerializeField]
        private GameObject _warningText;
        [SerializeField]
        private GameObject _entryPointDebugPlane;
        [SerializeField]
        private float _easeFunctionDuration;
        [SerializeField]
        private TMP_Text _currencyText;

        private bool _needsResync;

        private RectTransform _canvasRect;
        private RectTransform _stationPopupTransform;
        [SerializeField]
        private KitchenTile _selectedTile;
        private KitchenTile _lastSelectedTile;
        private KitchenTile[] _upgradableTiles;

        private PlayerDataContainer _playerData;
        private string _lastLayerMask = "0";
        [SerializeField]
        private bool _isStationPopupOn;

        public event Action<KitchenTile> OnSelectedTileUpdated;

        void Start()
        {
            _selectedTile = null;
            _stationPopupTransform = _stationPopup.GetComponent<RectTransform>();
            _upgradeConfirmPopup.SetActive(false);
            _canvasRect = GetComponent<RectTransform>();
            _stationPopup.transform.DOScale(0, _easeFunctionDuration).SetEase(Ease.InExpo);
            _kitchenEditor.OnObjectClicked += ShowOptions;
            _kitchenEditor.OnObjectClicked += OutlineSelectedObject;
            _onKitchenLoaded.onEventRaised += ReloadUpgradableEntities;
            _kitchenEditor.Loader.OnKitchenUpdated += UpdateKitchenSaveState;

            CachePlayerData();
        }

        void Update()
        {
            if(_needsResync)
            {
                LoadUpgradableEntities();
                _needsResync = false;
            }

            if (_playerData == null) 
                return;

            _currencyText.text = _playerData.Currencies.SoftCurrencyBalance.ToString();
        }

        private void LateUpdate()
        {
            if (_selectedTile != null)
            {
                SetUIElementPosToObject(_stationPopupTransform, _selectedTile);
            }
        }
        private async Task CachePlayerData()
        {
            while (GameManager.Instance == null || GameManager.Instance.PlayerDataContainer == null ||
                !GameManager.Instance.DataLoaded)
            {
                await Task.Delay(5);
            }

            _playerData = GameManager.Instance.PlayerDataContainer;
            _playerData.Currencies.BalanceChanged += ReloadUpgradableEntities;
        }

        void ReloadUpgradableEntities()
        {
            _needsResync = true;
            UpdateKitchenSaveState();
        }

        void LoadUpgradableEntities()
        {
            _upgradableTiles = KitchenLayoutManager.Instance.GetAllTilesOfType(TileType.BENCH)
            .Where(p => p.UpgradableData != null && p.LinkedEntity != null && p.TileData != "locked_slot").ToArray();

            if (_upgradableTiles.Count() != 0)
            {
                for (int i = 0; i < _upgradableTiles.Length; ++i)
                {
                    Upgradable UpgradableItem = _upgradableTiles[i].UpgradableData;

                    if (_playerData.Currencies.SoftCurrencyBalance >= UpgradableItem.Data.CostToUpgrade && !UpgradableItem.IsMaxLevel)
                    {
                        foreach (Transform t in _upgradableTiles[i].LinkedEntity.GetComponentsInChildren<Transform>())
                            t.gameObject.layer = LayerMask.NameToLayer("Highlight");
                    }
                    else
                    {
                        foreach (Transform t in _upgradableTiles[i].LinkedEntity.GetComponentsInChildren<Transform>())
                            t.gameObject.layer = LayerMask.NameToLayer("HighlightStatic");
                    }
                }
            }
        }

        void OutlineSelectedObject(EntityType _type)
        {
            if (IsStationPopupOn)
                return;

            if (_lastSelectedTile != null && _lastLayerMask != "0")
            {
                HideDebugPlane();
                foreach (Transform t in _lastSelectedTile.LinkedEntity.GetComponentsInChildren<Transform>())
                    t.gameObject.layer = LayerMask.NameToLayer(_lastLayerMask);

            }

            if (_selectedTile != null)
            {
                _lastLayerMask = LayerMask.LayerToName(_selectedTile.LinkedEntity.layer);
                _lastSelectedTile = _selectedTile;
                ShowDebugPlane();
                foreach (Transform t in _selectedTile.LinkedEntity.GetComponentsInChildren<Transform>())
                    t.gameObject.layer = LayerMask.NameToLayer("OutlineYellow");
            }
        }

        void SetUIElementPosToObject(RectTransform _element, KitchenTile _object)
        {
            if (_object.LinkedEntity == null)
                return;

            _element.anchoredPosition = MathCalculation.GetWorldToScreenPos(Camera.main, _canvasRect, _object.LinkedEntity.transform, -Vector2.up * 50);
        }

        public void ShowOptions(EntityType _type)
        {
            if (IsStationPopupOn)
                return;

            HideOptions();
            if (_type == null)
                return;

            _selectedTile = KitchenLayoutManager.Instance.GetTileWithEntity(_type.gameObject);
            _selectionPanel.ShowInfoButton(_selectedTile.TileType == TileType.BENCH);
            _selectionPanel.ShowMoveButton(true);
            _selectionPanel.ShowText(true);
            _onStationEditSelect.RaiseEvent();
            //ShowDebugPlane();
            _selectionPanel.SetInfoText(_selectedTile.LinkedEntity.GetComponent<EntityType>().Name);
            _kitchenCameraManager.ChangeCameraFollowTarget(_selectedTile.LinkedEntity.transform);
            HideKitchenSaveState();
            OnSelectedTileUpdated?.Invoke(_selectedTile);
        }

        void ShowDebugPlane()
        {
            if(_selectedTile.Entrypoint != EntryPoint.NONE)
            {
                _entryPointDebugPlane.transform.parent = _selectedTile.LinkedEntity.transform;
                Vector3 entryDir = KitchenLayoutManager.Instance.GetDirFromEntryPoint(_selectedTile.Entrypoint);
                _entryPointDebugPlane.transform.position = _selectedTile.LinkedEntity.transform.position + entryDir;
            }
        }

        void HideDebugPlane()
        {
            _entryPointDebugPlane.transform.parent = null;
            _entryPointDebugPlane.transform.position = Vector3.right * 1000;
        }

        public void ShowOptions()
        {
            if (_selectedTile == null)
                return;

            _selectionPanel.ShowInfoButton(_selectedTile.TileType == TileType.BENCH);
            _selectionPanel.ShowMoveButton(true);
            _selectionPanel.ShowText(true);
            _kitchenCameraManager.ChangeCameraFollowTarget(_selectedTile.LinkedEntity.transform);
            HideKitchenSaveState();
        }

        public void HideOptions(bool _deselectTile = true)
        {
            _selectionPanel.ShowAll(false);

            if(_deselectTile)
            {
                _selectedTile = null;
                //HideDebugPlane();
                _kitchenCameraManager.ChangeCameraFollowTarget(null);
                UpdateKitchenSaveState();
                OnSelectedTileUpdated?.Invoke(null);
            }
        }

        public void ShowStationPopup()
        {
            if (_selectedTile == null)
                return;

            Upgradable upgradable = _selectedTile.UpgradableData;
            EntityType entity = _selectedTile.LinkedEntity.GetComponent<EntityType>();

            if (upgradable != null)
            {
                _isStationPopupOn = true;
                _stationPopup.transform.DOScale(1, _easeFunctionDuration).SetEase(Ease.OutExpo);
                StationUpgradePopup stationUpgradePopup = _stationPopup.GetComponent<StationUpgradePopup>();

                bool _isPayable = _playerData.Currencies.SoftCurrencyBalance >= upgradable.Data.CostToUpgrade;
                stationUpgradePopup.ChangeName(entity.Name);
                stationUpgradePopup.ChangeDescription(entity.Description);
                stationUpgradePopup.ChangeLevel(upgradable.Data.Level.ToString());
                stationUpgradePopup.ChangeAccidentChanceStat(upgradable.Data.AccidentChance.ToString());
                stationUpgradePopup.ChangeProcessingTimeStat(upgradable.Data.ProcessTimeBonus.ToString());

                string _cost = upgradable.IsMaxLevel ? "MAX LEVEL" : upgradable.Data.CostToUpgrade.ToString();
                stationUpgradePopup.ChangeCost(_cost);

                stationUpgradePopup.ToggleButton(_isPayable && !upgradable.IsMaxLevel);
            }
        }

        public void ShowUpgradeConfirm()
        {
            _upgradeConfirmPopup.SetActive(true);
            _upgradeConfirmPopup.transform.DOScale(1, _easeFunctionDuration).SetEase(Ease.OutExpo);
            Upgradable upgradable = _selectedTile.UpgradableData;
            EntityType entity = _selectedTile.LinkedEntity.GetComponent<EntityType>();
            StationSlotStatEntry table = DataLoader.LoadStationStat(upgradable.Data.Level + 1);
            var stationUpgradeConfirmPopup = _upgradeConfirmPopup.GetComponent<StationUpgradeConfirmPopup>();
            stationUpgradeConfirmPopup.ChangeName(entity.Name);
            stationUpgradeConfirmPopup.ChangeLevel(upgradable.Data.Level, upgradable.Data.Level + 1);
            stationUpgradeConfirmPopup.ChangeAccidentChanceStat(upgradable.Data.AccidentChance, table.accident_bonus);
            stationUpgradeConfirmPopup.ChangeProcessingTimeStat(upgradable.Data.ProcessTimeBonus, table.processing_time_bonus);
            stationUpgradeConfirmPopup.ChangeCost(table.cost);
        }

        public void SetSelectedEntityTo(string _entityType)
        {

            Vector2Int _pos = new Vector2Int(_selectedTile.Position.x, _selectedTile.Position.y);
            HideDebugPlane();
            _kitchenEditor.Loader.SetEntityTo(_pos, _entityType);
            _selectedTile = KitchenLayoutManager.Instance.GetTileData(_pos.x, _pos.y);
            LoadUpgradableEntities();
            OnSelectedTileUpdated?.Invoke(_selectedTile);
        }

        public void UpgradeStation()
        {
            Upgradable upgradable = _selectedTile.UpgradableData;
            if (_playerData.Currencies.CanPay(Enums.ECurrencyType.SoftCurrency, upgradable.Data.CostToUpgrade))
            {
                if(upgradable.CanUpgrade(_playerData.Currencies.SoftCurrencyBalance))
                {
                    _playerData.Currencies.Pay(Enums.ECurrencyType.SoftCurrency, upgradable.Data.CostToUpgrade);
                    upgradable.Upgrade();
                    KitchenLayoutManager.Instance.SaveTiles();
                    HideConfirmPopup();
                    ShowStationPopup();
                }
            }
        }

        public void HideConfirmPopup()
        {
            _upgradeConfirmPopup.transform.DOScale(0, _easeFunctionDuration).SetEase(Ease.InExpo).OnComplete(() => { _upgradeConfirmPopup.SetActive(false); });
        }

        public void HideStationPopup()
        {
            _isStationPopupOn = false;
            _stationPopup.transform.DOScale(0, _easeFunctionDuration).SetEase(Ease.InExpo);
            ShowOptions();
        }

        void HideKitchenSaveState()
        {
            _saveButton.SetActive(false);
            _warningText.SetActive(false);
        }

        void UpdateKitchenSaveState()
        {
            bool isValid = CheckForValidSave();
            _saveButton.SetActive(isValid);
            _warningText.SetActive(!isValid);
        }

        bool CheckForValidSave()
        {
            bool hasStoveStation = false;
            bool hasBoilingStation = false;
            bool hasCuttingStation = false;

            foreach (EntityType t in _kitchenEditor.Loader.InstantiatedEntityTypes)
            {
                if (t.Type == "boiling_station")
                    hasBoilingStation = true;

                if (t.Type == "stove_station")
                    hasStoveStation = true;

                if (t.Type == "cutting_station")
                    hasCuttingStation = true;
            }

            if (hasStoveStation && hasCuttingStation && hasBoilingStation)
                return true;
            else
                return false;
        }

        public void SetSelectedTileTo(KitchenTile _tile)
        {
            _selectedTile = _tile;
        }

        private void OnDestroy()
        {
            _onKitchenLoaded.onEventRaised -= ReloadUpgradableEntities;
            _kitchenEditor.OnObjectClicked -= ShowOptions;
            _kitchenEditor.OnObjectClicked -= OutlineSelectedObject;
            _playerData.Currencies.BalanceChanged -= ReloadUpgradableEntities;
            _kitchenEditor.Loader.OnKitchenUpdated -= UpdateKitchenSaveState;
        }

        public bool IsStationPopupOn { get => _isStationPopupOn; }
        public KitchenEditorManager KitchenEditor { get => _kitchenEditor; }
        public KitchenTile SelectedTile { get => _selectedTile; }
        public KitchenCameraManager KitchenCamera { get => _kitchenCameraManager; }
    }
}
