using System;
using Runtime.Managers.GameplayManager;
using Runtime.ScriptableObjects.EventChannels;
using TMPro;
using UnityEngine;

namespace Runtime.UI.GameplayUI
{
    [RequireComponent(typeof(ShiftManager))]
    public class ShiftTimerUI : MonoBehaviour
    {
        private ShiftManager _shiftManager;

        [SerializeField] 
        private TMP_Text _timerText;
        
        [SerializeField] 
        private TMP_Text _rushText;
        
        [SerializeField] 
        private GameObject _startShiftPopup;
        
        [SerializeField] 
        private Color _rushTimerColor = Color.red;
        
        [Header("Listening to")]
        [SerializeField] 
        private VoidEventChannel _onShiftStart;
        
        [SerializeField]
        private VoidEventChannel _onShiftEnd;

        [SerializeField] private FloatEventChannel _onRushStartEventChannel;
        
        private bool _updateTimer;
        
        private void Awake()
        {
            _shiftManager = GetComponent<ShiftManager>();

            _timerText.enabled = false;

            _onRushStartEventChannel.onEventRaised += OnRushStart;
            _onShiftStart.onEventRaised += StartShift;
            _onShiftEnd.onEventRaised += EndShift;
        }

        private void Start()
        {
            _rushText.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _onRushStartEventChannel.onEventRaised -= OnRushStart;
            _onShiftStart.onEventRaised -= StartShift;
            _onShiftEnd.onEventRaised -= EndShift;
        }

        private void Update()
        {
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            if (_updateTimer == false)
            {
                return;
            }

            _timerText.text = TimeSpan.FromSeconds(_shiftManager.CurrentTime).ToString(@"mm\:ss");
        }

        private void StartShift()
        {
            UpdateTimer();
            _startShiftPopup.SetActive(false);
            _timerText.enabled = true;
            _updateTimer = true;
        }

        private void EndShift()
        {
            _updateTimer = false;
            _timerText.enabled = false;
            _timerText.text = TimeSpan.FromSeconds(0).ToString(@"mm\:ss");
            _rushText.enabled = false;
        }

        private void OnRushStart(float _speedMultiplier)
        {
            _rushText.color = _rushTimerColor;
            _rushText.gameObject.SetActive(true);
            _timerText.color = _rushTimerColor;
        }
    }
}