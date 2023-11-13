using Runtime.DataContainers;
using Runtime.Managers;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using Runtime.Utility;
using UnityEngine.Events;

namespace Runtime.Gameplay
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ChefNavigation : MonoBehaviour
    {
        [SerializeField] 
        private float _defaultMovementSpeed = 3.5f;

        [SerializeField]
        private float _lerpSpeed;

        [SerializeField] 
        private float _occupiedStationStopDistance = 2;
        
        [SerializeField] 
        private bool _logDebugMessage = false;

        public event Action OnChefMovementPaused;
        public event Action OnChefMovementResumed;
        
        private NavMeshAgent _agent;
        private KitchenLayoutManager _grid;
        private Chef _chef;
        private Coroutine _currentMoveCoroutine;
        private Vector3 _dir;
        private bool _chefPaused;
        private bool _isMoving;

        private void Awake()
        {
            _chef = GetComponent<Chef>();
        }

        private void Start()
        {
            _grid = KitchenLayoutManager.Instance;
            _isMoving = false;
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = _defaultMovementSpeed;
        }
        
        public void IncreaseChefSpeed(float _speedIncrease)
        {
            _agent.speed = _defaultMovementSpeed * _speedIncrease;
        }

        private void PauseChef()
        {
            DebugHelper.PrintDebugMessage("Paused", _logDebugMessage);
            _chefPaused = true;
            _agent.isStopped = true;
            OnChefMovementPaused?.Invoke();
        }

        private void ResumeChef()
        {
            _chefPaused = false;
            _agent.isStopped = false;
            OnChefMovementResumed?.Invoke();
        }

        public void StopCurrentMovement()
        {
            if (_currentMoveCoroutine == null) return;
            StopCoroutine(_currentMoveCoroutine);
            StopMoving();
            _agent.ResetPath();
        }
        
        private IEnumerator Move(KitchenTile _tile, Vector3 _dirToLook, float _stoppingDistance = 0)
        {
            if(!_isMoving)
            {
                StartMoving();
                _agent.ResetPath();
                _agent.SetDestination(new Vector3(_tile.Position.x, 0, _tile.Position.y));
                _agent.stoppingDistance = _stoppingDistance;
                
                while (_agent.pathPending)
                {
                    yield return null;
                }
                
                while (_agent.remainingDistance > _agent.stoppingDistance)
                {
                    yield return null;
                }

                if (_dirToLook != -Vector3.one)
                {
                    //We want to face the station, not look in the same direction as the station is
                    _dir = -_dirToLook;
                }
                StopMoving();
                
            }
            else
            {
                DebugHelper.PrintDebugMessage("Chef is already moving", _logDebugMessage);
            }
        }

        public void MoveToStorage(Storage _storage)
        {
            _currentMoveCoroutine = StartCoroutine(MoveToStorageCoroutine(_storage));
        }

        private IEnumerator MoveToStorageCoroutine(Storage _storage)
        {
            var tile = KitchenLayoutManager.Instance.GetTileData((int)_storage.Position.x, (int)_storage.Position.z);
            var entryPoint = GetKitchenTileEntryPoint(tile);
            
            if(!_isMoving)
            {
                StartMoving();
                _agent.ResetPath();
                _agent.SetDestination(new Vector3(entryPoint.Position.x, 0, entryPoint.Position.y));

                while (_agent.pathPending)
                {
                    yield return null;
                }
                
                while (_agent.remainingDistance > _agent.stoppingDistance)
                {
                    if (_agent.remainingDistance < _occupiedStationStopDistance)
                    {
                        if (_storage.CurrentInstruction.Chef != _chef)
                        {
                            if (!_chefPaused)
                            {
                                PauseChef();
                            }
                        }

                        else
                        {
                            if (_chefPaused)
                            {
                                ResumeChef();
                            }
                        }
                    }
                    
                    yield return null;
                }

                var dirToLook = _grid.GetDirFromEntryPoint(tile.Entrypoint);
                if (dirToLook != -Vector3.one)
                {
                    //We want to face the station, not look in the same direction as the station is
                    _dir = -dirToLook;
                }
                
                StopMoving();
            }
            else
            {
                DebugHelper.PrintDebugMessage("Chef is already moving", _logDebugMessage);
            }
            yield break;
        }

        public void MoveToStation(Station _station)
        {
            _currentMoveCoroutine = StartCoroutine(MoveToStationCoroutine(_station));
        }
        
        private IEnumerator MoveToStationCoroutine(Station _station)
        {
            var tile = KitchenLayoutManager.Instance.GetTileData((int)_station.Position.x, (int)_station.Position.z);
            var entryPoint = GetKitchenTileEntryPoint(tile);
            
            if(!_isMoving)
            {
                StartMoving();
                _agent.ResetPath();
                _agent.SetDestination(new Vector3(entryPoint.Position.x, 0, entryPoint.Position.y));

                while (_agent.pathPending)
                {
                    yield return null;
                }
                
                while (_agent.remainingDistance > _agent.stoppingDistance)
                {
                    if (_agent.remainingDistance < _occupiedStationStopDistance)
                    {
                        if (_station.CurrentInstruction.Chef != _chef)
                        {
                            if (!_chefPaused)
                            {
                                PauseChef();
                            }
                        }

                        else
                        {
                            if (_chefPaused)
                            {
                                ResumeChef();
                            }
                        }
                    }
                    
                    yield return null;
                }

                var dirToLook = _grid.GetDirFromEntryPoint(tile.Entrypoint);
                if (dirToLook != -Vector3.one)
                {
                    //We want to face the station, not look in the same direction as the station is
                    _dir = -dirToLook;
                }
                
                StopMoving();
            }
            else
            {
                DebugHelper.PrintDebugMessage("Chef is already moving", _logDebugMessage);
            }
        }
        
        private KitchenTile GetKitchenTileEntryPoint(KitchenTile _tile)
        {
            if (_tile.Entrypoint != EntryPoint.NONE)
            {
                return _grid.GetEntryPointTile(_tile.Position.x, _tile.Position.y);
            }
            else
            {
                DebugHelper.PrintDebugMessage("Can't move to tile. EntryPoint is NONE.", _logDebugMessage);
                return null;
            }
        }

        public void MoveToTile(KitchenTile _tile)
        {
            _currentMoveCoroutine = StartCoroutine(Move(_tile, Vector3.back));
        }

        public void LookAtDirection(Vector3 _dir)
        {
            this._dir = _dir;
        }

        private void Update()
        {
            if(!_isMoving)
            {
                if(_dir != -Vector3.one && _dir != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_dir), _lerpSpeed * Time.deltaTime);
                }
            }
            if(_agent.hasPath)
            {
                DebugHelper.PrintDebugMessage("is moving", _logDebugMessage);
            }
        }

        private void StartMoving()
        {
            _isMoving = true;
            _agent.avoidancePriority = 99;
        }

        private void StopMoving()
        {
            _currentMoveCoroutine = null;
            _isMoving = false;
            _agent.avoidancePriority = 0;
        }

        public Coroutine CurrentMoveCoroutine => _currentMoveCoroutine;
        public NavMeshAgent Agent => _agent;
        public bool ChefPaused => _chefPaused;
        public bool IsMoving => _isMoving;
    }
}
