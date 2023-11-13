using Runtime.ScriptableObjects.DataContainers.Stations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.UI.GameplayUI.ChefActionUI
{
    public class StationSelectionButton : MonoBehaviour
    {
        [SerializeField] 
        private Image _actionIcon;

        public UnityAction<StationSelectionButton> onStationActionClicked;
        
        private BaseStationAction _stationAction;

        public void Initialize(BaseStationAction _stationAction)
        {
            this._stationAction = _stationAction;
            _actionIcon.sprite = _stationAction.StationIcon;
        }

        public void OnButtonClicked()
        {
            onStationActionClicked?.Invoke(this);
        }

        public BaseStationAction StationAction => _stationAction;
    }
}