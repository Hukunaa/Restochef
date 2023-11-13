using System;
using System.Collections.Generic;
using Runtime.Managers.GameplayManager;
using Unity.Collections;
using UnityEngine;

namespace Runtime.ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "ScoreSettings", menuName = "ScriptableObjects/Gameplay/ScoreSettings", order = 0)]
    public class ScoreSettings : ScriptableObject
    {
        [SerializeField]
        private List<ScoreKeyValuePair> scoreMultipliers = new List<ScoreKeyValuePair>()
        {
            new ScoreKeyValuePair(EPerformanceFeedback.Failed),
            new ScoreKeyValuePair(EPerformanceFeedback.Completed),
            new ScoreKeyValuePair(EPerformanceFeedback.Good),
            new ScoreKeyValuePair(EPerformanceFeedback.Excellent)
        };

        [SerializeField] private Dictionary<EPerformanceFeedback, float> _scoreMultipliers;

        public Dictionary<EPerformanceFeedback, float> ScoreMultipliers
        {
            get => _scoreMultipliers;
            private set => _scoreMultipliers = value;
        }

        public void Initialize()
        {
            _scoreMultipliers = new Dictionary<EPerformanceFeedback, float>();
            foreach (var score in scoreMultipliers)
            {
                _scoreMultipliers[score.key] = score.val;
            }
        }
    }

    [Serializable]
    public class ScoreKeyValuePair
    {
        [ReadOnly]
        public EPerformanceFeedback key;
        
        public float val;

        public ScoreKeyValuePair(EPerformanceFeedback _key)
        {
            key = _key;
        }
    }
}