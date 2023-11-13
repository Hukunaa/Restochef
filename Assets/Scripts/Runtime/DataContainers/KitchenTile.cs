using Runtime.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.DataContainers
{
    [Serializable]
    public enum TileType
    {
        WALKABLE,
        BENCH,
        TABLE,
        ENTRYPOINT,
        STORAGE
    }

    public enum EntryPoint
    {
        RIGHT,
        UP,
        LEFT,
        DOWN,
        NONE
    }

    [Serializable]
    public class KitchenTile
    {
        [SerializeField]
        private TileType _tileType;
        [SerializeField]
        private string _tileData;
        [SerializeField]
        private int _posX;
        [SerializeField]
        private int _posY;
        [SerializeField]
        private EntryPoint _entrypoint;
        [SerializeField]
        Upgradable _upgradableData;

        private GameObject _linkedEntity;
        public KitchenTile()
        {
            _linkedEntity = null;
            _tileType = TileType.WALKABLE;
            _tileData = "walkable_area";
            _posX = 0;
            _posY = 0;
            _entrypoint = EntryPoint.NONE;
            _upgradableData = null;
        }

        public KitchenTile(int _newId, string _data, int _x, int _y, TileType _type = TileType.WALKABLE, EntryPoint _entry = EntryPoint.NONE, GameObject _entity = null, Upgradable upgradable = null)
        {
            _tileType = _type;
            _tileData = _data;
            _posX = _x;
            _posY = _y;
            _entrypoint = _entry;
            _linkedEntity = _entity;
            _upgradableData = upgradable;
        }
        public void SetEntryPoint(EntryPoint _entry)
        {
            _entrypoint = _entry;
        }
        public void SetPosition(int _x, int _y)
        {
            _posX = _x;
            _posY = _y;
        }

        public void SetEntity(GameObject _entity)
        {
            _linkedEntity = _entity;
        }

        public void SetData(string _entityType)
        {
            _tileData = _entityType;
        }

        public TileType TileType { get => _tileType; }
        public string TileData { get => _tileData; }

        public Vector2Int Position { get => new Vector2Int(_posX, _posY); }
        public EntryPoint Entrypoint { get => _entrypoint; }
        public GameObject LinkedEntity { get => _linkedEntity; }
        public Upgradable UpgradableData { get => _upgradableData; }
    }
}
