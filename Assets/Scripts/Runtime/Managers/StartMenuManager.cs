using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;

namespace Managers
{
    public class StartMenuManager : MonoBehaviour
    {
        [SerializeField] private VoidEventChannel _startGameEventChannel;
        [SerializeField] private VoidEventChannel _quitGameEventChannel;
        
        public void StartGame()
        {
            _startGameEventChannel.RaiseEvent();
        }

        public void QuitGame()
        {
            _quitGameEventChannel.RaiseEvent();
        }
    }
}
