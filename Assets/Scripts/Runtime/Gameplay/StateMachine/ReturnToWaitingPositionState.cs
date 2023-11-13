using System;
using System.Collections;
using System.Threading;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.Gameplay.StateMachine
{
    [Serializable]
    public class ReturnToWaitingPositionState : ChefState
    {
        private static readonly int Walk = Animator.StringToHash("Walk");
        
        public override void Enter()
        {
            _chef.onInstructionsChanged += OnInstructionReceived;
        }

        private void OnInstructionReceived()
        {
            if (_chef.CurrentInstruction != null)
            {
                StopAllCoroutines();
                _chef.Nav.StopCurrentMovement();
                _chef.StateMachine.TransitionTo(_chef.StateMachine.takeStorageIngredientState);
            }
        }
        
        public override IEnumerator ExecuteStateBehavior()
        {
            PlayAnimation(Walk);
            _chef.Nav.MoveToTile(_chef.WaitingTile);
            yield return _chef.Nav.CurrentMoveCoroutine;
            _chef.StateMachine.TransitionTo(_chef.StateMachine.waitForInstructionState);
        }

        public override void PlayAnimation(int _animationHash)
        {
            AnimatorHelper.ResetAllAnimatorTriggers(_chef.ChefAnimator);
            _chef.ChefAnimator.SetTrigger(_animationHash);
        }

        public override void Exit()
        {
            _chef.onInstructionsChanged -= OnInstructionReceived;
        }
    }
}