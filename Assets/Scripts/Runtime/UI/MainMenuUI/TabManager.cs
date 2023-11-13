using Runtime.ScriptableObjects.EventChannels;
using Runtime.UI.UIUtility;
using UnityEngine;

namespace Runtime.UI.MainMenuUI
{
    public class TabManager : MonoBehaviour
    {
        [SerializeField] private MainMenuTab _marketTab;
        [SerializeField] private MainMenuTab _menuTab;
        [SerializeField] private MainMenuTab _cookTab;
        [SerializeField] private MainMenuTab _brigadeTab;
        [SerializeField] private MainMenuTab _kitchenTab;

        [SerializeField] private SwipeMenuLayout _swipeMenu;

        [Header("Listening to")]
        [SerializeField] private VoidEventChannel[] _tabInitializedEventChannel;

        [Header("Broadcast on")]
        [SerializeField] private VoidEventChannel _onTabsInitialized;

        private MainMenuTab _currentTab;
        private int _initializedTabCount;

        private void Awake()
        {
            foreach (var eventChannel in _tabInitializedEventChannel)
            {
                eventChannel.onEventRaised += OnTabInitialized;
            }
        }

        private void OnDestroy()
        {
            foreach (var eventChannel in _tabInitializedEventChannel)
            {
                eventChannel.onEventRaised -= OnTabInitialized;
            }
        }

        private void OnTabInitialized()
        {
            _initializedTabCount += 1;
            if (_initializedTabCount == _tabInitializedEventChannel.Length)
            {
                TabsInitialized();
            }
        }

        private void TabsInitialized()
        {
            _onTabsInitialized.RaiseEvent();
            ShowCookTab();
            _swipeMenu.ShowLayout();
        }

        public void ShowMarketTab()
        {
            SwitchTab(_marketTab);
        }

        public void ShowMenuTab()
        {
            SwitchTab(_menuTab);
        }

        public void ShowCookTab()
        {
            SwitchTab(_cookTab);
        }

        public void ShowBrigadeTab()
        {
            SwitchTab(_brigadeTab);
        }

        public void ShowKitchenTab()
        {
            SwitchTab(_kitchenTab);
        }

        public void SwitchTab(MainMenuTab _newTab)
        {
            _currentTab = _newTab;
            _swipeMenu.ShowSelectedTab(_currentTab.GetComponent<RectTransform>());
        }

        public void ShowCurrentTab()
        {
            if (_currentTab == null) 
                return;

            _swipeMenu.ShowSelectedTab(_currentTab.GetComponent<RectTransform>());
        }

        public MainMenuTab MarketTab { get => _marketTab; }
        public MainMenuTab MenuTab { get => _menuTab; }
        public MainMenuTab CookTab { get => _cookTab; }
        public MainMenuTab BrigadeTab { get => _brigadeTab; }
        public MainMenuTab KitchenTab { get => _kitchenTab; }

    }
}