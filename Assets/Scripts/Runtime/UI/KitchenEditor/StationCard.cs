using Runtime.DataContainers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.UI.KitchenEditor
{
    public class StationCard : MonoBehaviour
    {
        [SerializeField]
        private string _entityType;

        public void OnCardClicked()
        {
            KitchenEditorUI _stationUI = GetComponentInParent<KitchenEditorUI>();
            if(_stationUI == null)
            {
                Debug.Log("No object found with Station Info UI as parent! ");
                return;
            }

            _stationUI.SetSelectedEntityTo(_entityType);
        }
    }
}
