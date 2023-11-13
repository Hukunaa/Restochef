using System;
using System.IO;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.DataContainers.Player
{
    [Serializable]
    public class PlayerScore
    {
        [SerializeField]
        private int _score;

        public UnityAction ScoreChanged;
        
        public void LoadScore()
        {
            _score = DataLoader.LoadScore();
        }
        
        public void SaveScore()
        {
            Debug.Log($"Saving Score: {_score.ToString()}");
            DataLoader.SaveScore(_score);
        }

        public void UpdateScore(int _newScore)
        {
            //Need server side authorization
            if (_newScore <= _score) return;
            _score = _newScore;
            ScoreChanged?.Invoke();
            SaveScore();
        }
        
        public int Score { get => _score; }
    }
}

