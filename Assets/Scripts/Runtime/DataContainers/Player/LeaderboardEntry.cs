using System;

namespace Runtime.DataContainers.Player
{
    [Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public int playerScore;

        public LeaderboardEntry(string _playerName, int _playerScore)
        {
            playerName = _playerName;
            playerScore = _playerScore;
        }
    }
}