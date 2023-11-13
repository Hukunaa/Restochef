using Runtime.DataContainers.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class LeaderboardEntryUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _rankText;
        [SerializeField] private TMP_Text _playerNameText;
        [SerializeField] private TMP_Text _playerScoreText;
        [SerializeField] private Image _backgroundImage;
        
        [SerializeField] private Color _leaderBackgroundColor = Color.yellow;
        [SerializeField] private Color _playerBackgroundColor = Color.red;
        [SerializeField] private Color _defaultBackgroundColor = Color.cyan;
        
        private LeaderboardEntry _entry;
        
        public void UpdateContent(int _rank, LeaderboardEntry _entry, bool _isPlayer)
        {
            this._entry = _entry;
            _rankText.text = _rank.ToString();
            _playerNameText.text = _entry.playerName;
            _playerScoreText.text = _entry.playerScore.ToString();

            if (_rank == 1)
            {
                _backgroundImage.color = _leaderBackgroundColor;
            }

            else if(_isPlayer)
            {
                _backgroundImage.color = _playerBackgroundColor;
            }

            else
            {
                _backgroundImage.color = _defaultBackgroundColor;
            }
        }

        public LeaderboardEntry Entry => _entry;
    }
}