using System;
using System.Collections.Generic;
using Runtime.DataContainers;
using Runtime.Gameplay;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;

namespace Runtime.Managers.GameplayManager
{
    public class ChefManager : MonoBehaviour
    {
        private Chef[] _chefs;
        private Chef _selectedChef;

        [SerializeField] private TransformEventChannel _onCameraFollowTargetChanged;

        [Header("Listening to")]
        [SerializeField] private VoidEventChannel _onChefSpawned;
        [SerializeField] private ChefSelectionEventChannel _onSelectChef;
        [SerializeField] private VoidEventChannel _onDeselectChef;
        
        [Header("Broadcasting on")]
        [SerializeField] private VoidEventChannel _onChefManagerInitialized;
        [SerializeField] private ChefSelectionEventChannel _onChefSelected;
        [SerializeField] private VoidEventChannel _onChefDeselected;
        
        private void Awake()
        {
            _onChefSpawned.onEventRaised += GetChefs;
            _onSelectChef.onEventRaised += SelectChef;
            _onDeselectChef.onEventRaised += DeselectChef;
        }

        private void OnDestroy()
        {
            _onChefSpawned.onEventRaised -= GetChefs;
            _onSelectChef.onEventRaised -= SelectChef;
            _onDeselectChef.onEventRaised -= DeselectChef;
        }
        
        private void GetChefs()
        {
            _chefs = FindObjectsOfType<Chef>();
            _onChefManagerInitialized.RaiseEvent();
            Initialized = true;
        }
        
        private void SelectChef(Chef _chef)
        {
            if (_selectedChef == _chef)
            {
                DeselectChef();
                return;
            }
            
            _selectedChef = _chef;
            _onCameraFollowTargetChanged.RaiseEvent(_chef.transform);
            _onChefSelected.RaiseEvent(_chef);
        }

        private void DeselectChef()
        {
            _selectedChef = null;
            _onCameraFollowTargetChanged.RaiseEvent(null);
            _onChefDeselected.RaiseEvent();
        }

        public Chef SelectedChef { get => _selectedChef; }
        public Chef[] Chefs { get => _chefs; }

        public bool Initialized { get; private set; }
    }
}