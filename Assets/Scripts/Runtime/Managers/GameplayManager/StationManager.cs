using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Gameplay;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.DataContainers.Stations;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.Managers.GameplayManager
{
    public class StationManager : MonoBehaviour
    {
        [Header("Listening to")]
        [SerializeField] 
        private VoidEventChannel _onKitchenLoaded;

        [Header("Broadcasting on")]
        [SerializeField] 
        private VoidEventChannel _onStationInitializedEventChannel;
        
        
        [SerializeField] 
        private bool _logDebugMessage;

        private Dictionary<ActionType, List<Station>> _stations = new Dictionary<ActionType, List<Station>>();
        private PlayerDataContainer _playerDataContainer;
        
        private void Awake()
        {
            _onKitchenLoaded.onEventRaised += InitializeStations;
        }

        private void OnDestroy()
        {
            _onKitchenLoaded.onEventRaised -= InitializeStations;
        }

        private void InitializeStations()
        {
            StartCoroutine(InitializeStationsCoroutine());
        }

        private IEnumerator InitializeStationsCoroutine()
        {
            while (GameManager.Instance == null || !GameManager.Instance.DataLoaded)
            {
                yield return null;
            }

            _playerDataContainer = GameManager.Instance.PlayerDataContainer;
            
            GetSceneStations();
            DispatchMixes();
            _onStationInitializedEventChannel.RaiseEvent();
            Debug.Log("Station Manager Initialized");
            StationsInitialized = true;
        }
        

        private void DispatchMixes()
        {
            HashSet<BaseStationAction> _stationActions = new HashSet<BaseStationAction>();

            foreach (var stationType in _stations)
            {
                foreach (var station in stationType.Value)
                {
                    _stationActions.Add(station.Action);
                }
            }

            foreach (var stationAction in _stationActions)
            {
                var mixes = _playerDataContainer.SelectedKitchenData.IngredientMixes.Where(x => x.StationAction == stationAction).ToArray();
                stationAction.AssignMixes(mixes);
            }
        }

        private void GetSceneStations()
        {
            var stations = FindObjectsOfType<Station>();

            foreach (var station in stations)
            {
                if (!_stations.ContainsKey(station.Action.StationActionType))
                {
                    _stations[station.Action.StationActionType] = new List<Station>();
                }
                
                _stations[station.Action.StationActionType].Add(station);
            }
        }

        private Station[] GetStationsOfType(ActionType _actionType)
        {
            if (!_stations.ContainsKey(_actionType) || _stations[_actionType].Count == 0)
            {
                DebugHelper.PrintDebugMessage($"Station Manager has no entry for station of type {_actionType.name}", _logDebugMessage);
                return null;
            }
            
            return _stations[_actionType].ToArray();
        }

        public Station GetClosestAvailableStation(ActionType _actionType, Vector3 pos)
        {
            var station = GetStationsOfType(_actionType);

            if (station == null) return null;
            
            if (station.Length == 1)
            {
                return station[0];
            }

            var orderedStations = station.OrderBy(s => (s.transform.position - pos).sqrMagnitude).ToArray();
            var unusedStation = orderedStations.First(_s => _s.CurrentlyUsed == false);
            return unusedStation != null ? unusedStation : orderedStations[0];
        }

        public void OverrideAccident()
        {
            foreach (var stationType in _stations)
            {
                foreach (var station in stationType.Value)
                {
                    station.BeginAccidentOverride();
                }
            }
        }

        public void StopOverridingAccident()
        {
            foreach (var stationType in _stations)
            {
                foreach (var station in stationType.Value)
                {
                    station.StopAccidentOverride();
                }
            }
        }

        public Dictionary<ActionType, List<Station>> KitchenStations => _stations;

        public bool StationsInitialized { get; private set; }
    }
}