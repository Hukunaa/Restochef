using Runtime.Managers.GameplayManager.Orders;
using Runtime.Managers.GameplayManager.Orders.CustomClass;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.UI;
using Runtime.UI.GameplayUI;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.Managers.GameplayManager
{
    public class ShiftScoreManager : MonoBehaviour
    {
        private int _currentScore;
        
        [SerializeField]
        private ScoreUI _scoreUI;
        
        [SerializeField] private ScoreSettings _scoreSettings;

        [SerializeField] private OrderManager _orderManager;

        [Header("Listening to")]
        [SerializeField] private VoidEventChannel _retrieveShiftGains;
        
        [SerializeField] private bool _showDebugMessage = false;

        private void Awake()
        {
            _scoreSettings.Initialize();

            if (_scoreUI == null)
            {
                _scoreUI = FindObjectOfType<ScoreUI>();
            }

            if (_orderManager == null)
            {
                _orderManager = FindObjectOfType<OrderManager>();
            }

            _orderManager.OrderClosed += AddToScore;

            _retrieveShiftGains.onEventRaised += RetrieveScore;

            UpdateScore(0);
        }

        private void OnDestroy()
        {
            if (_orderManager == null) return;
            _orderManager.OrderClosed -= AddToScore;
            _retrieveShiftGains.onEventRaised -= RetrieveScore;
        }

        private void AddToScore(Order _order, EPerformanceFeedback _performanceFeedback)
        {
            var pointsEarned = Mathf.RoundToInt(_order.Recipe.RecipePoints * _scoreSettings.ScoreMultipliers[_performanceFeedback]);
            DebugHelper.PrintDebugMessage($"Earned {pointsEarned} points by preparing a {_performanceFeedback.ToString()} {_order.Recipe.name}", _showDebugMessage);
            var newScore = _currentScore + pointsEarned;
            UpdateScore(newScore);
        }

        private void UpdateScore(int _newScore)
        {
            _currentScore = _newScore;
            _scoreUI.UpdateScore(_currentScore.ToString());
        }

        //Called on ReturnHome button clicked
        private void RetrieveScore()
        {
            GameManager.Instance.PlayerDataContainer.ComputePlayerScore(_currentScore);
        }

        public int CurrentScore => _currentScore;

        public bool IsHighScore => CurrentScore > GameManager.Instance.PlayerDataContainer.PlayerScore.Score;
    }
}