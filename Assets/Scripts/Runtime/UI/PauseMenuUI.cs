using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private Canvas _pauseMenuCanvas;
        [SerializeField] private GameObject _pauseMenu;

        [Header("Broadcasting on")]
        [SerializeField] private BoolEventChannel _changeTimePausedEventChannel;
        [SerializeField] private VoidEventChannel _onQuitShiftEventChannel;

        [Header("Listening to")]
        [SerializeField] private VoidEventChannel[] _displayUIEventChannels;
        [SerializeField] private VoidEventChannel[] _hideUIEventChannel;
        
        private void Awake()
        {
            if (_displayUIEventChannels.Length > 0)
            {
                foreach (var eventChannel in _displayUIEventChannels)
                {
                    eventChannel.onEventRaised += DisplayUI;
                }
            }

            if (_hideUIEventChannel.Length > 0)
            {
                foreach (var eventChannel in _hideUIEventChannel)
                {
                    eventChannel.onEventRaised += HideUI;
                }
            }
        }

        private void OnDestroy()
        {
            if (_displayUIEventChannels.Length > 0)
            {
                foreach (var eventChannel in _displayUIEventChannels)
                {
                    eventChannel.onEventRaised -= DisplayUI;
                }
            }

            if (_hideUIEventChannel.Length > 0)
            {
                foreach (var eventChannel in _hideUIEventChannel)
                {
                    eventChannel.onEventRaised -= HideUI;
                }
            }
        }

        private void Start()
        {
            HidePauseMenu();
            HideUI();
        }

        private void DisplayUI()
        {
            _pauseMenuCanvas.enabled = true;
        }

        private void HideUI()
        {
            _pauseMenuCanvas.enabled = false;
        }
        
        private void ShowPauseMenu()
        {
            DisplayButtons();
            _changeTimePausedEventChannel.RaiseEvent(true);
        }

        private void HidePauseMenu()
        {
            HideButtons();
            _changeTimePausedEventChannel.RaiseEvent(false);
        }
        
        private void DisplayButtons()
        {
            _pauseMenu.SetActive(true);
        }

        private void HideButtons()
        {
            
            _pauseMenu.SetActive(false);
        }
        
        public void OnPauseMenuButtonClicked()
        {
            ShowPauseMenu();
        }
        
        public void OnResumeButtonClicked()
        {
            HidePauseMenu();
        }
        
        public void OnQuitButtonClicked()
        {
            HidePauseMenu();
            _onQuitShiftEventChannel.RaiseEvent();
        }
    }
}