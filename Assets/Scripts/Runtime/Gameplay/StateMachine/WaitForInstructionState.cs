using System;
using System.Collections;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.Gameplay.StateMachine
{
    [Serializable]
    public class WaitForInstructionState : ChefState
    {
        private static readonly int Idle = Animator.StringToHash("Idle");
        
        public override void Enter()
        {
            PlayAnimation(Idle);
            _chef.Nav.LookAtDirection(Vector3.back);
            _chef.onInstructionsChanged += OnInstructionsChanged;
        }

        private void OnInstructionsChanged()
        {
            if (_chef.CurrentInstruction == null) return;
            
            _chef.StateMachine.TransitionTo(_chef.StateMachine.takeStorageIngredientState);
        }

        public override IEnumerator ExecuteStateBehavior()
        {
            yield break;
        }

        public override void PlayAnimation(int _animationHash)
        {
            AnimatorHelper.ResetAllAnimatorTriggers(_chef.ChefAnimator);
            _chef.ChefAnimator.SetTrigger(_animationHash);
        }
        

        public override void Exit()
        {
            _chef.onInstructionsChanged -= OnInstructionsChanged;
        }
    }
}