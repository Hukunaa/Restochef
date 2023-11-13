using Runtime.DataContainers;
using Runtime.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Managers
{
    public class WallBuilder : MonoBehaviour
    {
        public List<GameObject> _walls;
        public List<GameObject> _cornerWall;

        private List<GameObject> _wallInstances;
        private KitchenLayoutManager _gridManager;
        private GameObject _empty;

        [SerializeField]
        private Material _opaqueWallMaterial;
        [SerializeField]
        private Material _transparentWallMaterial;
        [SerializeField] 
        private bool _logDebugMessage = false;

        private void Start()
        {
            _wallInstances = new List<GameObject>();
            _gridManager = KitchenLayoutManager.Instance;
        }

        public void GenerateWalls()
        {
            _empty = new GameObject("Walls");
            LogDebugMessage("Generating Walls");
            if (_walls.Count == 0 || _cornerWall.Count == 0)
            {
                return;
            }

            int sizeX = _gridManager.Settings.kitchenSizeX;
            int sizeY = _gridManager.Settings.kitchenSizeY;

            for (int x = 0; x < sizeX; ++x)
            {
                for (int y = 0; y < sizeY; ++y)
                {
                    bool xLimit = (x == 0 || x == sizeX - 1) ? true : false;
                    bool yLimit = (y == 0 || y == sizeY - 1) ? true : false;
                    bool Iscorner = xLimit && yLimit ? true : false;
                    bool Isvalid = xLimit || yLimit ? true : false;

                    Vector2 _lookDir = _gridManager.GetInsideDirectionFromTile(_gridManager.Tiles[x, y]);
                    Quaternion _direction = Quaternion.identity;

                    if (Isvalid)
                    {
                        _direction = Quaternion.LookRotation(new Vector3(_lookDir.x, 0, _lookDir.y));
                    }

                    CreateWall(new Vector3(x, 0, y), _direction, Iscorner, Isvalid);
                }
            }
        }

        void CreateWall(Vector3 _position, Quaternion _rotation, bool _corner, bool Isvalid)
        {
            if (!Isvalid)
                return;

            GameObject prefab = _corner ? _cornerWall[Random.Range(0, _cornerWall.Count)] : _walls[Random.Range(0, _walls.Count)];
            GameObject _instance = Instantiate(prefab, _position, _rotation);
            _instance.transform.parent = _empty.transform;

            if (Vector3.Dot(_instance.transform.forward, Vector3.Cross(Camera.main.transform.right, Vector3.up)) > 0)
                MakeWallTransparent(_instance);

            _wallInstances.Add(_instance);
        }

        void MakeWallTransparent(GameObject _wall)
		{
            MeshRenderer[] _renderers = _wall.GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer r in _renderers)
                r.material = _transparentWallMaterial;
		}
        private void LogDebugMessage(string _message)
        {
            if (_logDebugMessage == false) return;
            print(_message);
        }
    }
}
