using System.Linq;
using Cinemachine;
using Runtime.Gameplay;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;

namespace Runtime.Managers.GameplayManager
{
    public class KitchenCameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private KitchenLoader _kitchenLoader;
        
        [SerializeField] private TransformEventChannel _changeFollowTargetEventChannel;
        [SerializeField] private VoidEventChannel _onKitchenEditorStart;
        
        [SerializeField] private CinemachineTargetGroup _targetGroup;
        
        private void Awake()
        {
            _changeFollowTargetEventChannel.onEventRaised += ChangeCameraFollowTarget;
            _onKitchenEditorStart.onEventRaised += InitializeEditorCamera;
        }

        private void InitializeEditorCamera()
        {
            InitializeEditorTargetGroup();
            _virtualCamera.m_Follow = _targetGroup.Transform;
            _onKitchenEditorStart.onEventRaised -= InitializeEditorCamera;
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