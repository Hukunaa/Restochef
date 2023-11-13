using System;
using Cinemachine;
using Newtonsoft.Json;
using Runtime.Gameplay;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;
using System.Linq;

namespace Runtime.Managers.GameplayManager
{
    public class ShiftCameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private ChefManager _chefManager;
        [SerializeField] private KitchenLoader _kitchenLoader;
        

        [SerializeField] private TransformEventChannel _changeFollowTargetEventChannel;
        [SerializeField] private VoidEventChannel _onChefManagerInitialized;
        [SerializeField] private VoidEventChannel _onKitchenEditorStart;
        
        [SerializeField] private CinemachineTargetGroup _targetGroup;

        public CinemachineVirtualCamera VirtualCamera { get => _virtualCamera; }
        public CinemachineTargetGroup TargetGroup { get => _targetGroup; }

        private void Awake()
        {
            _changeFollowTargetEventChannel.onEventRaised += ChangeCameraFollowTarget;
            _onChefManagerInitialized.onEventRaised += InitializeCamera;
            _onKitchenEditorStart.onEventRaised += InitializeEditorCamera;
        }

        private void InitializeCamera()
        {
            InitializeTargetGroup();
            _virtualCamera.m_Follow = _targetGroup.Transform;
            _onChefManagerInitialized.onEventRaised -= InitializeCamera;
        }

        private void InitializeEditorCamera()
        {
            InitializeEditorTargetGroup();
            _virtualCamera.m_Follow = _targetGroup.Transform;
            _onKitchenEditorStart.onEventRaised -= InitializeEditorCamera;
        }
        private void InitializeTargetGroup()
        {
            _targetGroup.m_Targets = new CinemachineTargetGroup.Target[_chefManager.Chefs.Length];
            for (int i = 0; i < _chefManager.Chefs.Length; i++)
            {
                _targetGroup.m_Targets[i].target = _chefManager.Chefs[i].transform;
                _targetGroup.m_Targets[i].weight = 1;
            }
        }
        private void InitializeEditorTargetGroup()
        {
            _targetGroup.m_Targets = new CinemachineTargetGroup.Target[_kitchenLoader.InstantiatedObjects.Count];
            for (int i = 0; i < _kitchenLoader.InstantiatedObjects.Count; i++)
            {
                _targetGroup.m_Targets[i].target = _kitchenLoader.InstantiatedObjects[i].transform;
                _targetGroup.m_Targets[i].weight = 1;
            }
        }

        private void OnDestroy()
        {
            _changeFollowTargetEventChannel.onEventRaised -= ChangeCameraFollowTarget;
            _onChefManagerInitialized.onEventRaised -= InitializeCamera;
            _onKitchenEditorStart.onEventRaised -= InitializeEditorCamera;
        }

        public void ChangeCameraFollowTarget(Transform newTarget)
        {
            if (newTarget == null)
            {
                _virtualCamera.m_Follow = _targetGroup.Transform;
                return;
            }
            
            _virtualCamera.m_Follow = newTarget;
        }
    }
}