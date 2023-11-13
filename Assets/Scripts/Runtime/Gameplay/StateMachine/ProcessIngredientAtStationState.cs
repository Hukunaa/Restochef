using System;
using System.Collections;
using Runtime.Managers.GameplayManager;
using Runtime.Managers.GameplayManager.Orders;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.Gameplay.StateMachine
{
    [Serializable]
    public class ProcessIngredientAtStationState : ChefState
    {
        private static readonly int WalkHolding = Animator.StringToHash("WalkHolding");
        private static readonly int IdleHolding = Animator.StringToHash("IdleHolding");
        private static readonly int Cutting = Animator.StringToHash("Cutting");
        private static readonly int Frying = Animator.StringToHash("Frying");
        private static readonly int Boiling = Animator.StringToHash("Boiling");

        private OrderManager _orderManager;
        private StationManager _stationManager;

        private Instruction _chefInstruction;
        private Station _station;
        
        private void Awake()
        {
            _orderManager = FindObjectOfType<OrderManager>();
            _stationManager = FindObjectOfType<StationManager>();
        }
        
        public override void Enter()
        {
            _chefInstruction = _chef.CurrentInstruction;
            _chef.onInstructionCancelled += OnInstructionCancelled;
            _station = _stationManager.GetClosestAvailableStation(_chefInstruction.StationAction.StationActionType,
                _chef.CurrentInstruction.Chef.Position);
        }

        private void OnInstructionCancelled(Instruction _cancelledInstruction)
        {
            if (_chefInstruction != _cancelledInstruction)
            {
                return;
            }
            
            _station.InstructionCancelled(_cancelledInstruction);
            StopAllCoroutines();
            
            _chef.Nav.OnChefMovementPaused -= OnChefMovementPaused;
            _chef.Nav.OnChefMovementResumed -= OnChefMovementResumed;
            _station.OnIngredientProcessed -= OnIngredientProcessed;
            _chef.Nav.StopCurrentMovement();
            
            _chef.ReleaseIngredient();
            ReturnInstrument();
            _chef.StateMachine.TransitionTo(_chef.StateMachine.returnToWaitingPositionState);
        }

        public override IEnumerator ExecuteStateBehavior()
        {
            _station.QueueInstruction(_chef.CurrentInstruction);
            yield return StartCoroutine(MoveToStation());
            ProcessIngredient();
        }
        
        private void SendIngredient(Ingredient _processedIngredient)
        {
            _orderManager.AssignIngredientToOrder(_processedIngredient);
            DebugHelper.PrintDebugMessage($"{_chef.ChefSettings.ChefName} sent {_processedIngredient.IngredientName}.", _showDebugMessage);
        }
        
        private IEnumerator MoveToStation()
        {
            _chef.Nav.MoveToStation(_station);
            
            _chef.Nav.OnChefMovementPaused += OnChefMovementPaused;
            _chef.Nav.OnChefMovementResumed += OnChefMovementResumed;
            
            PlayAnimation(WalkHolding);
            
            while (_chef.Nav.IsMoving)
            {
                yield return null;
            }
            
            _chef.Nav.OnChefMovementPaused -= OnChefMovementPaused;
            _chef.Nav.OnChefMovementResumed -= OnChefMovementResumed;
        }

        private void OnChefMovementResumed()
        {
            PlayAnimation(WalkHolding);
        }

        private void OnChefMovementPaused()
        {
            PlayAnimation(IdleHolding);
        }

        private void ProcessIngredient()
        {
            _chef.TransformIngredient(_station);
            TakeInstrument();
            PlayStationInteractionAnimation();
            _station.ProcessIngredient(_chefInstruction.Chef, _chefInstruction.Ingredient);
            _station.OnIngredientProcessed += OnIngredientProcessed;
        }

        private void OnIngredientProcessed(Ingredient _output)
        {
            _station.OnIngredientProcessed -= OnIngredientProcessed;
            ReturnInstrument();
            _chef.ReleaseIngredient();
            SendIngredient(_output);
            ChangeState();
        }

        private void TakeInstrument()
        {
            switch (_station.Action.StationActionType.name)
            {
                case "t1_prep":
                    _station.StationInstrument.transform.SetParent(_chef.ChefSocketsManager.KnifeSocket.transform, false); 
                    _station.StationInstrument.transform.localPosition = Vector3.zero;
                    break;
                case "t2_stove":
                    _station.StationInstrument.transform.SetParent(_chef.ChefSocketsManager.PanSocket.transform, false); 
                    _station.StationInstrument.transform.localPosition = Vector3.zero;
                    break;
                case "t3_oven":
                    break;
            }
        }

        private void ReturnInstrument()
        {
            _station.StationInstrument.transform.SetParent(_station.InstrumentSocket.transform, false);
            _station.StationInstrument.transform.localPosition = Vector3.zero;
        }

        private void PlayStationInteractionAnimation()
        {
            switch (_station.Action.StationActionType.name)
            {
                case "t1_prep":
                    _chef.ChefAnimator.SetTrigger(Cutting);
                    break;
                case "t2_stove":
                    _chef.ChefAnimator.SetTrigger(Frying);
                    break;
                case "t3_oven":
                    _chef.ChefAnimator.SetTrigger(Boiling);
                    break;
            }
        }
        
        public override void PlayAnimation(int _animationHash)
        {
            AnimatorHelper.ResetAllAnimatorTriggers(_chef.ChefAnimator);
            _chef.ChefAnimator.SetTrigger(_animationHash);
        }
        
        private void ChangeState()
        {
            _chef.OnInstructionProcessed(_chefInstruction);

            if (_chef.CurrentInstruction != null)
            {
                _chef.StateMachine.TransitionTo(_chef.StateMachine.takeStorageIngredientState);
            }
            else
            {
                _chef.StateMachine.TransitionTo(_chef.StateMachine.returnToWaitingPositionState);
            }
        }

        public override void Exit()
        {
            _chef.onInstructionCancelled -= OnInstructionCancelled;
        }
    }
}