using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.DataContainers;
using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers.Stations;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using Runtime.Utility;
using UnityEngine;
using DG.Tweening;

namespace Runtime.Gameplay
{
    [Serializable]
    [RequireComponent(typeof(EntityType))]
    public class Station : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeReference]
        private BaseStationAction _action;
       
        [SerializeField]
        private int _numberOfClicksForRepair;
        
        [Header("References")]
        [SerializeField]
        private GameObject _instrumentSocket;
        
        [SerializeField]
        private GameObject _stationInstrument;
        
        [SerializeField]
        private Transform _ingredientSocket;
        
        [SerializeField] 
        private FloatEventChannel _onShiftStartRushEventChannel;

        [SerializeField] 
        private VoidEventChannel _onStationAccidentStart;
        
        [SerializeField] 
        private VoidEventChannel _onStationAccidentEnd;

        [SerializeField]
        private AudioSource _onActionStartAudio;

        [SerializeField]
        private AudioSource _onAccidentClickAudio;
        

        [Header("Tutorial")]
        [SerializeField] 
        private VoidEventChannel _stationAccidentStart;
        
        [SerializeField] 
        private VoidEventChannel _stationAccidentEnd;
        
        [Header("Debug")]
        [SerializeField]
        private bool _forceAccident;
        
        [SerializeField]
        private bool _logDebugMessage;
        
        private LinkedList<Instruction> _instructionQueue;
        
        public event Action<float> OnStationProcessStarted;
        public event Action OnStationProcessCancelled;
        public event Action OnStationProcessPause;
        public event Action OnStationProcessResume;
        public event Action OnStationClicked;
        public event Action<Ingredient> OnIngredientProcessed;
        private KitchenTile _entityTile;

        private float _speedMultiplier = 1;


        private int _currentNumberOfClicks;
        
        private bool _isMalfunctioning;

        private Coroutine _currentProcessIngredientCoroutine;

        private void Awake()
        {
            _isMalfunctioning = false;
            _onShiftStartRushEventChannel.onEventRaised += OnShiftRushStart;
        }

        private void OnDestroy()
        {
            _onShiftStartRushEventChannel.onEventRaised -= OnShiftRushStart;
        }

        private void OnShiftRushStart(float _speedIncrease)
        {
            _speedMultiplier = _speedIncrease;
        }

        private void Start()
        {
            _instructionQueue = new LinkedList<Instruction>();
            _speedMultiplier = 1;
        }
        
        public void QueueInstruction(Instruction _instruction)
        {
            _instructionQueue.AddLast(_instruction);
        }
        
        public void InstructionComplete()
        {
            _instructionQueue.RemoveFirst();
        }
        
        public void InstructionCancelled(Instruction _cancelledInstruction)
        {
            if (CurrentInstruction == null) return;
            
            if (CurrentInstruction != _cancelledInstruction)
            {
                _instructionQueue.Remove(_cancelledInstruction);
                _onActionStartAudio.Stop();
                return;
            }
            
            _instructionQueue.Remove(_cancelledInstruction);
            OnStationProcessCancelled?.Invoke();
            if (_currentProcessIngredientCoroutine != null)
            {
                StopCoroutine(_currentProcessIngredientCoroutine);
            }
            
            DebugHelper.PrintDebugMessage($"{name}: Instruction {_cancelledInstruction.FormatInstruction()} cancelled. {_instructionQueue.Count} in queue");
        }

        public void ProcessIngredient(Chef _chef, RawIngredient _ingredient)
        {
            _currentProcessIngredientCoroutine = StartCoroutine(ProcessIngredientCoroutine(_chef, _ingredient));
        }

        private IEnumerator ProcessIngredientCoroutine(Chef _chef, RawIngredient _ingredient)
        {
            if (_entityTile == null)
            {
                _entityTile = KitchenLayoutManager.Instance.GetTileWithEntity(this.gameObject);
            }

            int _chefBonus = GetChefBonus(_chef);
            IngredientResult output = _action.ProcessIngredient(_ingredient);
            float timeBonus = ((float)(_entityTile.UpgradableData.Data.ProcessTimeBonus + _chefBonus) / 100.0f) * output.ResultMix.ProcessingDuration;
            var processTime = (output.ResultMix.ProcessingDuration - timeBonus) / _speedMultiplier;
            OnStationProcessStarted?.Invoke(processTime);

            _onActionStartAudio.Play();

            int accidentChance = _forceAccident ? 200 : _entityTile.UpgradableData.Data.AccidentChance;
            bool doesFail = UnityEngine.Random.Range(0, 100) <= accidentChance;
            var failDelay = processTime / UnityEngine.Random.Range(2, 10);
            var successDelay = processTime - failDelay;
            if (doesFail)
            {
                yield return new WaitForSeconds(failDelay);
                _stationAccidentStart.RaiseEvent();
                _isMalfunctioning = true;
                OnStationProcessPause?.Invoke();
                _onStationAccidentStart.RaiseEvent();
                transform.DOScale(1.1f, 0.3f).SetLoops(-1, LoopType.Yoyo);
                while (_isMalfunctioning)
                {
                    yield return null;
                }
                OnStationProcessResume?.Invoke();
                _stationAccidentEnd.RaiseEvent();
                _onStationAccidentEnd.RaiseEvent();
                yield return new WaitForSeconds(successDelay);
            }
            else
            {
                yield return new WaitForSeconds(processTime);
            }

            OnIngredientProcessed?.Invoke(output.Result);
            _onActionStartAudio.Stop();
            DebugHelper.PrintDebugMessage($"Prepared: {output.Result.IngredientName}", _logDebugMessage);
            
            _currentProcessIngredientCoroutine = null;
            InstructionComplete();
        }
        
        public void CheckFire()
        {
            OnStationClicked?.Invoke();
            if (_isMalfunctioning)
            {
                _onAccidentClickAudio.Play();

                if (_currentNumberOfClicks < _numberOfClicksForRepair)
                {
                    _currentNumberOfClicks++;
                    transform.DOKill();
                    transform.DOScale(0.8f, 0.1f).OnComplete(() => transform.DOScale(1.0f, 0.1f));
                }
                
                if(_currentNumberOfClicks >= _numberOfClicksForRepair)
                {
                    _currentNumberOfClicks = 0;
                    _isMalfunctioning = false;
                    OnStationProcessResume?.Invoke();
                }
            }
        }

        public int GetChefBonus(Chef _chef)
        {
            switch(_entityTile.TileData)
            {
                case "cutting_station":
                    return _chef.Data.Skills[0].Level;
                case "stove_station":
                    return _chef.Data.Skills[1].Level;
                case "boiling_station":
                    return _chef.Data.Skills[2].Level;
            }
            return 0;
        }

        public void BeginAccidentOverride()
        {
            _forceAccident = true;
        }

        public void StopAccidentOverride()
        {
            _forceAccident = false;
        }

        public Coroutine CurrentProcessIngredientCoroutine => _currentProcessIngredientCoroutine;

        public Instruction CurrentInstruction => _instructionQueue.First.Value;
        public bool CurrentlyUsed => _instructionQueue.Count > 0;

        public Vector3 Position => transform.position;
        public BaseStationAction Action => _action;

        public GameObject InstrumentSocket => _instrumentSocket;
        public GameObject StationInstrument => _stationInstrument;
        public Transform IngredientSocket => _ingredientSocket;

        public bool IsMalfunctioning { get => _isMalfunctioning; }
    }
}
