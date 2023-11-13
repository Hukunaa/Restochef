using Runtime.DataContainers;
using Runtime.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Runtime.UI.KitchenEditor
{
    public class KitchenEditorMovementHandler : MonoBehaviour
    {
        private KitchenEditorManager _kitchenEditor;
        private KitchenLayoutManager _kitchenLayout;
        private KitchenEditorUI _kitchenUI;
        private KitchenTile _selectedTile;
        private KitchenTile _selectedEntrypoint;
        private Drag _dragEntity;
        private AudioSource _snapAudio;

        [SerializeField]
        private SelectionPanel _kitchenPanel;
        [SerializeField]
        private bool _isMovingEntity;
        [SerializeField]
        private float _snapScale;
        [SerializeField]
        private AudioClip _snapAudioClip;
        [SerializeField]
        private Vector3 _cursorPosSnapped;
        private Vector3 _lastCursorPosSnapped;
        [SerializeField]
        private Vector3 _entryPointDir;
        private bool[,] _validMap;
        private Plane _plane;

        private string _lastLayerMask;
        void Start()
        {
            _plane = new Plane(Vector3.up, Vector3.zero);
            _isMovingEntity = false;
            _kitchenUI = GetComponent<KitchenEditorUI>();
            _kitchenEditor = _kitchenUI.KitchenEditor;
            _kitchenLayout = KitchenLayoutManager.Instance;
            _snapAudio = GetComponent<AudioSource>();
        }

        public void EnterMoveMode()
        {
            if (_kitchenUI.SelectedTile == null)
                return;

            _selectedTile = _kitchenUI.SelectedTile;
            _kitchenUI.KitchenCamera.ChangeCameraFollowTarget(null);
            _selectedEntrypoint = _kitchenLayout.GetEntryPointTile(_selectedTile.Position.x, _selectedTile.Position.y);

            if(_selectedEntrypoint != null)
                _entryPointDir = _kitchenLayout.GetDirFromEntryPoint(_selectedTile.Entrypoint);

            InitDefaultTile(_kitchenUI.SelectedTile.Position.x, _kitchenUI.SelectedTile.Position.y);
            BuildMap();

            _lastLayerMask = LayerMask.LayerToName(_selectedTile.LinkedEntity.gameObject.layer);
            SetObjectLayerMask("OutlineGreen");
            _lastCursorPosSnapped = _selectedTile.LinkedEntity.transform.position;
            _dragEntity = _selectedTile.LinkedEntity.gameObject.AddComponent<Drag>();
            _dragEntity.SetHandler(this);
            _isMovingEntity = true;
            _kitchenEditor.BlockEvents(true);

            Vector3 _transitionPos = _selectedTile.LinkedEntity.transform.position;
            _transitionPos.y = 0;
            _selectedTile.LinkedEntity.transform.DOKill();
            _selectedTile.LinkedEntity.transform.DOMove(_transitionPos + Vector3.up * 0.2f, 0.25f).SetEase(Ease.OutCirc);
        }

        void InitDefaultTile(int x, int y)
        {
            if(_selectedTile.Entrypoint != EntryPoint.NONE)
            {
                KitchenTile defaultEntrypointTile = new KitchenTile();
                KitchenTile _tile = _kitchenLayout.GetEntryPointTile(_selectedTile.Position.x, _selectedTile.Position.y);
                defaultEntrypointTile.SetPosition(_tile.Position.x, _tile.Position.y);
                _kitchenLayout.CopyTileTo(defaultEntrypointTile, _tile.Position.x, _tile.Position.y);
            }

            KitchenTile defaultTile = new KitchenTile();
            defaultTile.SetPosition(x,y);
            _kitchenLayout.CopyTileTo(defaultTile, x, y);
        }

        public void ExitMoveMode()
        {
            Destroy(_selectedTile.LinkedEntity.gameObject.GetComponent<Drag>());
            SetObjectLayerMask(_lastLayerMask);

            _selectedTile.LinkedEntity.transform.DOKill();
            Vector3 _transitionPos = _selectedTile.LinkedEntity.transform.position;
            _transitionPos.y = 0;
            _selectedTile.LinkedEntity.transform.DOMove(_transitionPos, 0.25f).SetEase(Ease.OutCirc);

            _selectedTile = null;
            _selectedEntrypoint = null;
            _dragEntity = null;
            _isMovingEntity = false;
            _kitchenEditor.BlockEvents(false);
        }

        void BuildMap()
        {
            _validMap = new bool[_kitchenLayout.Settings.kitchenSizeX, _kitchenLayout.Settings.kitchenSizeY];

            //Remove all non walkable areas from being a valid position
            for (int x = 0; x < _kitchenLayout.Settings.kitchenSizeX; ++x)
                for (int y = 0; y < _kitchenLayout.Settings.kitchenSizeY; ++y)
                {
                    _validMap[x, y] = true;
                    if(_kitchenLayout.Tiles[x,y].TileType != TileType.WALKABLE)
                        _validMap[x, y] = false;
                }
        }

        void SetObjectLayerMask(string _layer)
        {
            if (_selectedTile.LinkedEntity.gameObject.layer != LayerMask.NameToLayer(_layer))
                foreach (Transform t in _selectedTile.LinkedEntity.GetComponentsInChildren<Transform>())
                    t.gameObject.layer = LayerMask.NameToLayer(_layer);
        }

        private void Update()
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            _plane.Raycast(r, out float _distance);
            Vector3 point = r.origin + r.direction * _distance;
            _cursorPosSnapped = Snapping.Snap(point, Vector3.one * _snapScale);
            _cursorPosSnapped.x = Mathf.Clamp(_cursorPosSnapped.x, 0, _kitchenLayout.Settings.kitchenSizeX - 1);
            _cursorPosSnapped.z = Mathf.Clamp(_cursorPosSnapped.z, 0, _kitchenLayout.Settings.kitchenSizeY - 1);
            if(_dragEntity != null)
            {
                if(_dragEntity.IsDragged)
                {
                    if(_lastCursorPosSnapped != _cursorPosSnapped)
                    {
                        _lastCursorPosSnapped = _cursorPosSnapped;
                        _snapAudio.PlayOneShot(_snapAudioClip);
                    }
                }
                if (!VerifyValidPosition())
                {
                    _kitchenPanel.ShowValidateButton(false);
                    SetObjectLayerMask("OutlineRed");
                }
                else
                {
                    _kitchenPanel.ShowValidateButton(true);
                    SetObjectLayerMask("OutlineGreen");
                }
            }

        }

        public void ValidatePlacement()
        {
            _kitchenLayout.CopyTileTo(_selectedTile, (int)_lastCursorPosSnapped.x, (int)_lastCursorPosSnapped.z);
            if(_selectedEntrypoint != null)
                _kitchenLayout.CopyTileTo(_selectedEntrypoint, (int)_lastCursorPosSnapped.x + (int)_entryPointDir.x, (int)_lastCursorPosSnapped.z + (int)_entryPointDir.z);

            _kitchenUI.SetSelectedTileTo(_selectedTile);
            //_kitchenLayout.SaveTiles();
        }

        public void RotateEntity()
        {
            if (_selectedTile == null)
                return;

            if(_selectedEntrypoint != null)
            {
                _entryPointDir = Quaternion.AngleAxis(90, Vector3.up) * _entryPointDir;
                _entryPointDir.x = Mathf.RoundToInt(_entryPointDir.x);
                _entryPointDir.y = Mathf.RoundToInt(_entryPointDir.y);
                _entryPointDir.z = Mathf.RoundToInt(_entryPointDir.z);
                _selectedTile.SetEntryPoint(_kitchenLayout.GetEntryPointFromDir(_entryPointDir));
            }
            _selectedTile.LinkedEntity.transform.DOKill(true);
            _selectedTile.LinkedEntity.transform.DORotate(new Vector3(0, 90, 0), 0.2f, RotateMode.WorldAxisAdd).SetEase(Ease.OutCirc);
        }

        public bool VerifyValidPosition()
        {
            bool _entryPointValid = true;
            if (_selectedEntrypoint != null)
            {
                _entryPointDir = _kitchenLayout.GetDirFromEntryPoint(_selectedTile.Entrypoint);
                int x = (int)_lastCursorPosSnapped.x + (int)_entryPointDir.x;
                int y = (int)_lastCursorPosSnapped.z + (int)_entryPointDir.z;

                if (x >= _kitchenLayout.Settings.kitchenSizeX || x < 0 || y < 0 || y >= _kitchenLayout.Settings.kitchenSizeY)
                    _entryPointValid = false;
                else
                    _entryPointValid = _validMap[x, y];
            }

            return _validMap[(int)_lastCursorPosSnapped.x, (int)_lastCursorPosSnapped.z] && _entryPointValid;
        }

        public bool IsMovingEntity { get => _isMovingEntity; }
        public KitchenTile SelectedTile { get => _selectedTile; }
        public Vector3 CursorPosSnapped { get => _cursorPosSnapped; }
    }
}
