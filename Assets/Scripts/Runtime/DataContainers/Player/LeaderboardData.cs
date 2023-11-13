using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.DataContainers.Player
{
    [Serializable]
    public class LeaderboardData
    {
        [SerializeField]
        private List<LeaderboardEntry> _leaderboardEntries = new List<LeaderboardEntry>();

        public List<LeaderboardEntry> LeaderboardEntries
        {
            get => _leaderboardEntries;
            set => _leaderboardEntries = value;
        }
        
        public void SortLeaderboard()
        {
            _leaderboardEntries = _leaderboardEntries.OrderByDescending(x => x.playerScore).ToList();
        }

        public void UpdatePlayerEntryLeaderboard(string _playerName, int _newHighScore)
        {
            var playerEntry = _leaderboardEntries.FirstOrDefault(x => x.playerName == _playerName);
            if (playerEntry == null)
            {
                DebugHelper.PrintDebugMessage($"There is no entry with {_playerName} as a player name. Can't update the Leaderboard");
                return;
            }

            playerEntry.playerScore = _newHighScore;
            OnLeaderboardChanged?.Invoke();
            SortLeaderboard();
            SaveLeaderboard();
        }

        public void AddPlayerEntry(string _playerName)
        {
            if (_leaderboardEntries.Any(x => x.playerName == _playerName)) return;
            
            _leaderboardEntries.Add(new LeaderboardEntry(_playerName, 0));
            OnLeaderboardChanged?.Invoke();
            SortLeaderboard();
            SaveLeaderboard();
        }

        private void SaveLeaderboard()
        {
            DataLoader.SaveLeaderboard(_leaderboardEntries);
        }

        public UnityAction OnLeaderboardChanged;
    }
}