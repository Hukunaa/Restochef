using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Managers.GameplayManager.Orders;
using Runtime.Managers.GameplayManager.Orders.CustomClass;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Gameplay
{
    public class ChefAI : MonoBehaviour
    {
        [SerializeField] 
        private VoidEventChannel _startAIBehaviorEventChannel;
        
        [SerializeField] 
        private OrderManager _orderManager;

        [SerializeField] 
        private AISettings _aiSettings;
        
        private float _currentTime = 0;

        private List<Chef> _controlledChefs = new List<Chef>();
        private List<Station> _stations;
        private readonly List<Station> _malfunctioningStations = new List<Station>();

        private bool _initialized;

        private void Awake()
        {
            _startAIBehaviorEventChannel.onEventRaised += InitializeAI;
        }
        
        private void OnDestroy()
        {
            StopAllCoroutines();
            _startAIBehaviorEventChannel.onEventRaised -= InitializeAI;
        }

        private void InitializeAI()
        {
            _controlledChefs = FindObjectsOfType<Chef>().ToList();
            _stations = FindObjectsOfType<Station>().ToList();

            _initialized = true;
        }
        
        private void Update()
        {
            if (_initialized == false) return;
            
            if (_currentTime <= 0)
            {
                TryAssignInstruction();
                CheckForStationAccident();
                _currentTime = _aiSettings.UpdateRate;
            }

            else
            {
                _currentTime -= Time.deltaTime;
            }
        }

        private void CheckForStationAccident()
        {
            foreach (var station in _stations)
            {
                if (station.IsMalfunctioning && !_malfunctioningStations.Contains(station))
                {
                    StartCoroutine(RepairStation(station));
                }
            }
        }

        private void TryAssignInstruction()
        {
            if (_orderManager.Orders.Count == 0 || _controlledChefs.Count == 0 || _controlledChefs.All(x => x.InstructionCount > 0))
            {
                return;
            }

            foreach (var chef in _controlledChefs)
            {
                if(chef.InstructionCount > 0) continue;
                
                foreach (var order in _orderManager.Orders)
                {
                    if (TryAssigningOrderIngredient(chef, order))
                    {
                        break;
                    }
                }
            }
        }

        private bool TryAssigningOrderIngredient(Chef _chef, Order _order)
        {
            var orderIngredient = _order.OrderIngredients.FirstOrDefault(x => x.Assigned == false);
            if (orderIngredient == null) return false;
            
            orderIngredient.Assign();
            var instruction = new Instruction(_chef, orderIngredient.Ingredient.IngredientMix.Input, orderIngredient.Ingredient.IngredientMix.StationAction);
            StartCoroutine(SendInstruction(_chef, instruction));
            return true;
        }

        private IEnumerator SendInstruction(Chef _chef, Instruction _instruction)
        {
            var sendInstructionDelay = Random.Range(_aiSettings.InstructionReactionTime.x, _aiSettings.InstructionReactionTime.y);
            yield return new WaitForSeconds(sendInstructionDelay);
            _chef.AddInstruction(_instruction);
        }

        private IEnumerator RepairStation(Station _station)
        {
            if (!_malfunctioningStations.Contains(_station))
            {
                _malfunctioningStations.Add(_station);
            }
            
            var reactionDelay = Random.Range(_aiSettings.AccidentReactionTime.x, _aiSettings.AccidentReactionTime.y);
            yield return new WaitForSeconds(reactionDelay);
            
            while (_station.IsMalfunctioning)
            {
                var clickDelay = Random.Range(_aiSettings.RepairSpeedRange.x, _aiSettings.RepairSpeedRange.y);
                yield return new WaitForSeconds(clickDelay);
                _station.CheckFire();
            }

            _malfunctioningStations.Remove(_station);
        }
    }
}