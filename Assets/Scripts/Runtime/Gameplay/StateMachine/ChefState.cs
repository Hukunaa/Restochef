using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.StateMachine
{
    [Serializable]
    public abstract class ChefState : MonoBehaviour
    {
        [SerializeField] 
        protected Chef _chef;

        [SerializeField] 
        private string _stateName;
        
        [SerializeField] 
        protected bool _showDebugMessage;
        
        public abstract void Enter();

        public void StartStateBehavior()
        {
            StartCoroutine(ExecuteStateBehavior());
        }

        public abstract IEnumerator ExecuteStateBehavior();

        public abstract void PlayAnimation(int _animationHash);
        
        public abstract void Exit();

        public string StateName => _stateName;
    }
}