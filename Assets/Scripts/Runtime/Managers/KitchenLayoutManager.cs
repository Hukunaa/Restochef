using System.Collections.Generic;
using UnityEngine;
using Runtime.DataContainers;
using Runtime.Utility;
using System;
using System.Linq;
using Runtime.DataContainers.Player;

namespace Runtime.Managers
{
    [ExecuteInEditMode]
    public class KitchenLayoutManager : MonoBehaviour
    {
        private static KitchenLayoutManager instance = null;
        public static KitchenLayoutManager Instance => instance;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                instance = this;
            }
        }
        
        private KitchenData _settings;
        private KitchenTile[,] _tiles;

        private Dictionary<TileType, List<KitchenTile>> _sortedTiles;
        
        [SerializeField]
        private bool _logDebugMessage = false;

        public void InitTiles()
        {
            _tiles = new KitchenTile[_settings.kitchenSizeX, _settings.kitchenSizeY];
            for (int i = 0; i < _settings.kitchenSizeX; ++i)
            {
                for (int j = 0; j < _settings.kitchenSizeY; ++j)
                {
                    _tiles[i, j] = new KitchenTile();
                    _tiles[i, j].SetPosition(i, j);
                }
            }
            SaveTiles();
        }

        public void LoadTiles()
        {
            _settings = GameManager.Instance.PlayerDataContainer.SelectedKitchenData;
            _tiles = GameManager.Instance.PlayerDataContainer.SelectedKitchenData.tiles;
            if (_tiles == null)
            {
                InitTiles();
            }
            else
            {
                SaveGridPositions();
                FillSortedTiles();
            }
        }

        void SaveGridPositions()
        {
            for (int i = 0; i < _settings.kitchenSizeX; ++i)
            {
                for (int j = 0; j < _settings.kitchenSizeY; ++j)
                {
                    _tiles[i, j].SetPosition(i, j);
                }
            }
        }

        void FillSortedTiles()
        {
            if (_sortedTiles != null)
            {
                _sortedTiles.Clear();
            }
            else
            {
                _sortedTiles = new Dictionary<TileType, List<KitchenTile>>();
            }

            foreach(KitchenTile t in _tiles)
            {
                if(_sortedTiles.ContainsKey(t.TileType))
                {
                    _sortedTiles[t.TileType].Add(t);
                }
                else
                {
                    _sortedTiles.Add(t.TileType, new List<KitchenTile>());
                    _sortedTiles[t.TileType].Add(t);
                }
            }
        }

        public KitchenTile GetTileWithEntity(GameObject _object)
        {
            if (_tiles == null)
                return null;

            foreach(KitchenTile t in _tiles)
            {
                if (t.LinkedEntity == _object)
                    return t;
            }
            return null;

        }
        public KitchenTile[] GetAllTilesOfType(TileType _type)
        {
            if (_sortedTiles.ContainsKey(_type))
            {
                return _sortedTiles[_type].ToArray();
            }
            else
            {
                return null;
            }
        }
        public void SaveTiles()
        {
            DataLoader.SaveGrid(_settings.kitchenLayoutName, _tiles, _settings.kitchenSizeX, _settings.kitchenSizeY);
        }

        public void SwitchTile(KitchenTile _from, KitchenTile _to)
        {
            KitchenTile _tmp = _from;
            Vector2Int _tmpPos = _to.Position;
            Vector2Int _tmpPos2 = _from.Position;

            _tiles[_from.Position.x, _from.Position.y] = _to;
            _tiles[_from.Position.x, _from.Position.y].SetPosition(_tmpPos2.x, _tmpPos2.y);

            _tiles[_to.Position.x, _to.Position.y] = _tmp;
            _tiles[_to.Position.x, _to.Position.y].SetPosition(_tmpPos.x, _tmpPos.y);
        }

        public void CopyTileTo(KitchenTile _tile, int x, int y)
        {
            if (x < _settings.kitchenSizeX && y < _settings.kitchenSizeY && x >= 0 && y >= 0)
            {
                _tiles[x, y] = _tile;
                _tiles[x, y].SetPosition(x, y);
            }
        }
        public KitchenTile GetTileData(int x, int y)
        {
            if (x < _settings.kitchenSizeX && y < _settings.kitchenSizeY && x >= 0 && y >= 0)
            {
                return _tiles[x, y];
            }
            else
            {
                return null;
            }
        }

        public Vector3 GetDirFromEntryPoint(EntryPoint _entry)
        {
            switch(_entry)
            {
                case EntryPoint.RIGHT:
                    return Vector3.right;
                case EntryPoint.LEFT:
                    return -Vector3.right;
                case EntryPoint.UP:
                    return Vector3.forward;
                case EntryPoint.DOWN:
                    return -Vector3.forward;
                case EntryPoint.NONE:
                    return -Vector3.one;
            }
            return -Vector3.one;
        }
        public EntryPoint GetEntryPointFromDir(Vector3 _dir)
        {
            if (_dir == Vector3.right)
                return EntryPoint.RIGHT;

            if (_dir == -Vector3.right)
                return EntryPoint.LEFT;

            if (_dir == Vector3.forward)
                return EntryPoint.UP;

            if (_dir == -Vector3.forward)
                return EntryPoint.DOWN;

            if (_dir == -Vector3.one)
                return EntryPoint.NONE;

            return EntryPoint.NONE;
        }

        public KitchenTile GetEntryPointTile(int x, int y)
        {
            if (x < _settings.kitchenSizeX && y < _settings.kitchenSizeY && x >= 0 && y >= 0)
            {
                if (_tiles[x, y].TileType != TileType.BENCH && _tiles[x, y].TileType != TileType.STORAGE)
                {
                    return null;
                }
                else
                {
                    switch(_tiles[x,y].Entrypoint)
                    {
                        case EntryPoint.RIGHT:
                            return _tiles[x + 1, y];
                        case EntryPoint.LEFT:
                            return _tiles[x - 1, y];
                        case EntryPoint.UP:
                            return _tiles[x, y + 1];
                        case EntryPoint.DOWN:
                            return _tiles[x, y - 1];
                    }

                    return null;
                }
            }
            else
            {
                LogDebugMessage("No valid tile found");
                return null;
            }
        }
        
        public Vector2 GetInsideDirectionFromTile(KitchenTile _tile)
        {
            if (_tile == null)
            {
                LogDebugMessage("Invalid Tile input for inside direction calculation");
                return Vector2.zero;
            }

            Vector2 _dir = Vector2.right;
            Vector2 _finalDir = Vector2.zero;
            for(int i = 0; i < 4; ++i)
            {
                if (GetTileData(_tile.Position.x + (int)_dir.x, _tile.Position.y + (int)_dir.y) != null)
                    _finalDir += _dir;

                _dir = MathCalculation.RotateVector(_dir, 90);
            }
            return _finalDir.normalized;
        }
        private void LogDebugMessage(string _message)
        {
            if (_logDebugMessage == false) return;
            print(_message);
        }

        public KitchenData Settings { get => _settings; }
        public KitchenTile[,] Tiles { get => _tiles; }
    }
}
