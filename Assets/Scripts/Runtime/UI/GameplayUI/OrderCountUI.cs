using System;
using Runtime.ScriptableObjects.EventChannels;
using TMPro;
using UnityEngine;

namespace Runtime.UI.GameplayUI
{
    public class OrderCountUI : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text _orderSuccessCountText;
        
        [SerializeField] 
        private TMP_Text _orderFailureCountText;

        [Header("Listening on")]
        [SerializeField] 
        private VoidEventChannel _onShiftStart;
        
        [SerializeField] 
        private VoidEventChannel _onShiftEnd;
        
        [SerializeField] 
        private VoidEventChannel _onOrderSuccess;
        
        [SerializeField] 
        private VoidEventChannel _onOrderFailure;
        
        private int _orderSuccessCount;
        private int _orderFailureCount;

        private void Awake()
        {
            _onShiftStart.onEventRaised += ShowOrderCount;
            _onShiftEnd.onEventRaised += HideOrderCount;
            _onOrderSuccess.onEventRaised += OnOrderSuccess;
            _onOrderFailure.onEventRaised += OnOrderFailure;
        }
        
        private void OnDestroy()
        {
            _onShiftStart.onEventRaised -= ShowOrderCount;
            _onShiftEnd.onEventRaised -= HideOrderCount;
            _onOrderSuccess.onEventRaised -= OnOrderSuccess;
            _onOrderFailure.onEventRaised -= OnOrderFailure;
        }

        private void Start()
        {
            _orderSuccessCount = 0;
            _orderFailureCount = 0;
            UpdateOrderSuccessCount();
            UpdateOrderFailureCount();
            HideOrderCount();
        }

        private void ShowOrderCount()
        {
            gameObject.SetActive(true);
        }
        
        private void HideOrderCount()
        {
            gameObject.SetActive(false);
        }
        
        private void OnOrderSuccess()
        {
            _orderSuccessCount += 1;
            UpdateOrderSuccessCount();
        }

        private void OnOrderFailure()
        {
            _orderFailureCount += 1;
            UpdateOrderFailureCount();
        }

        private void UpdateOrderSuccessCount()
        {
            _orderSuccessCountText.text = _orderSuccessCount.ToString();
        }
        
        private void UpdateOrderFailureCount()
        {
            _orderFailureCountText.text = _orderFailureCount.ToString();
        }

        public int OrderSuccessCount => _orderSuccessCount;
        public int OrderFailureCount => _orderFailureCount;
    }
}