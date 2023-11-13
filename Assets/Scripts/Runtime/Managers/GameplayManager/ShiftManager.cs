using System;
using GeneralScriptableObjects;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.UI.GameplayUI;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Managers.GameplayManager
{
    [RequireComponent(typeof(ShiftTimerUI))]
    public class ShiftManager : MonoBehaviour
    {
        [SerializeField]
        private ShiftSettings _shiftSettings;

        [SerializeField] 
        private bool _startShiftOnSceneInitialized;
        
        [Header("Listening to")] [SerializeField]
        private VoidEventChannel _sceneInitializedEventChannel;
        
        [Header("Broadcast on")]
        [SerializeField]
        private VoidEventChannel _onShiftStartedEventChannel;
        
        [SerializeField]
        private VoidEventChannel _onShiftEndedEventChannel;

        [SerializeField]
        private FloatEventChannel _onShiftRushStart;

        [Header("Unity Events")]
        [SerializeField]
        private UnityEvent _onShiftStart;
        
        [SerializeField] 
        private UnityEvent _onRushStart;
        
        [SerializeField]
        private UnityEvent _onShiftEnded;

        private bool _rushModeActive;
        public float CurrentTime { get; private set; }

        private bool _shiftStarted;
        
        private void Awake()
        {
            if (_startShiftOnSceneInitialized)
            {
                _sceneInitializedEventChannel.onEventRaised += StartShift;
            }
        }

        private void OnDestroy()
        {
            if (_startShiftOnSceneInitialized)
            {
                _sceneInitializedEventChannel.onEventRaised -= StartShift;
            }

        }

        public void StartShift()
        {
            _shiftStarted = true;
            _onShiftStartedEventChannel.RaiseEvent();
            _onShiftStart?.Invoke();
        }

        private void Start()
        {
            CurrentTime = _shiftSettings.ShiftDuration;
        }

        private void Update()
        {
            if (_shiftStarted == false) return;
            
            CurrentTime -= Time.deltaTime;

            if (_shiftSettings.ShiftRushStart > 0 && !_rushModeActive && CurrentTime <= _shiftSettings.ShiftRushStart)
            {
                ActivateRushMode();
            }
            
            if (CurrentTime <= 0)
            {
               ShiftEnded();
            }
        }

        private void ShiftEnded()
        {
            _shiftStarted = false;
            CurrentTime = 0;
            _onShiftEndedEventChannel.RaiseEvent();
            _onShiftEnded?.Invoke();
        }

        private void ActivateRushMode()
        {
            _rushModeActive = true;
            _onShiftRushStart.RaiseEvent(_shiftSettings.RushSpeedIncrease);
            _onRushStart?.Invoke();
        }
    }
}