using Runtime.ScriptableObjects.EventChannels;
using TMPro;
using UnityEngine;

namespace Runtime.UI.GameplayUI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private Canvas _scoreCanvas;
        [SerializeField] private TMP_Text _scoreText;
        
        [SerializeField] private VoidEventChannel _onShiftStarted;
        [SerializeField] private VoidEventChannel _onShiftEnded;

        [SerializeField] private bool _hideOnStart = true;
        
        
        private void Awake()
        {
            _scoreCanvas = GetComponent<Canvas>();

            _onShiftStarted.onEventRaised += DisplayScore;
            _onShiftEnded.onEventRaised += HideScore;
        }

        private void Start()
        {
            if (_hideOnStart)
            {
                HideScore();
            }
        }

        private void OnDestroy()
        {
            _onShiftStarted.onEventRaised -= DisplayScore;
            _onShiftEnded.onEventRaised -= HideScore;
        }

        private void DisplayScore()
        {
            _scoreCanvas.enabled = true;
        }

        private void HideScore()
        {
            _scoreCanvas.enabled = false;
        }

        public void UpdateScore(string _newScore)
        {
            _scoreText.text = _newScore;
        }
    }
}