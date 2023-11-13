using System;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.Gameplay.StateMachine
{
    [Serializable]
    public class ChefStateMachine : MonoBehaviour
    {
        [SerializeField] 
        private Chef _chef;
        
        [SerializeField] 
        private bool _showStateChangesDebugMessage;
        
        public WaitForInstructionState waitForInstructionState;
        public TakeStorageIngredientState takeStorageIngredientState;
        public ProcessIngredientAtStationState processIngredientState;
        public ReturnToWaitingPositionState returnToWaitingPositionState;

        public ChefState currentState;

        public void Initialize(ChefState _startingState)
        {
            CurrentState = _startingState;
            _startingState.Enter();
            DebugHelper.PrintDebugMessage($"{_chef.ChefSettings.ChefName} enter {CurrentState.StateName}!", _showStateChangesDebugMessage);
            _startingState.StartStateBehavior();
        }
        public void TransitionTo(ChefState _nextState)
        {
            CurrentState.Exit();
            DebugHelper.PrintDebugMessage($"{_chef.ChefSettings.ChefName} exit {CurrentState.StateName}!", _showStateChangesDebugMessage);
            CurrentState = _nextState;
            _nextState.Enter();
            DebugHelper.PrintDebugMessage($"{_chef.ChefSettings.ChefName} enter {CurrentState.StateName}!", _showStateChangesDebugMessage);
            _nextState.StartStateBehavior();
            currentState = _nextState;
        }
        
        public ChefState CurrentState { get; private set; }
    }
}