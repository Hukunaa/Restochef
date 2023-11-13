using Runtime.Managers.GameplayManager;
using Runtime.ScriptableObjects.EventChannels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Runtime.DataContainers;
using UnityEngine.EventSystems;

namespace Runtime.Managers
{
    public class KitchenEditorManager : MonoBehaviour
    {
        [SerializeField]
        private VoidEventChannel _onKitchenEditorStart;
        [SerializeField]
        private VoidEventChannel _onKitchenLoaded;
        [SerializeField]
        private KitchenLoader _kitchenLoader;
        
        private bool _restrictClick;
        private string _clickableObjectName;

        public event Action<EntityType> OnObjectClicked;
        private bool _isBlocked;

        void Start()
        {
            _isBlocked = false;
            _onKitchenLoaded.onEventRaised += InitEditor;
        }

        void InitEditor()
        {
            _onKitchenLoaded.onEventRaised -= InitEditor;
            //DO STUFF
            _onKitchenEditorStart.RaiseEvent();
        }

        public void RestrictClickToSingleObject(string _objectName)
        {
            _restrictClick = true;
            _clickableObjectName = _objectName;
        }

        public void StopClickRestriction()
        {
            _restrictClick = false;
        }
        
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !_isBlocked)
            {
                Ray _cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit _hit;
                if (Physics.Raycast(_cameraRay, out _hit))
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                        return;

                    EntityType _type = _hit.collider.gameObject.GetComponentInParent<EntityType>();

                    if (_restrictClick)
                    {
                        if (_type == null || _type.gameObject.name != _clickableObjectName) return;
                        OnObjectClicked?.Invoke(_type);
                    }

                    else
                    {
                        OnObjectClicked?.Invoke(_type);
                    }
                }
            }
        }

        public void BlockEvents(bool _value)
        {
            _isBlocked = _value;
        }

        public KitchenLoader Loader { get => _kitchenLoader; }
    }
}
