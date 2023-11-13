using UnityEngine;
using Runtime.DataContainers;
using Runtime.Managers;

namespace Runtime.UI 
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(KitchenLayoutManager))]
    public class GridRenderer : MonoBehaviour
    {
        [SerializeField]
        private KitchenLayoutManager _grid;

        [SerializeField] 
        private bool _showGrid;

        [SerializeField][Range(0,1)]
        private float _gridTransparency = .5f;
        
        
        private void Start()
        {
            _grid = GetComponent<KitchenLayoutManager>();
        }

        private void OnDrawGizmos()
        {
            if (_showGrid == false)
            {
                return;
            }
            
            if(_grid.Settings != null)
            {
                if(_grid.Tiles != null)
                {
                    for(int x = 0; x < _grid.Settings.kitchenSizeX; ++x)
                    {
                        for (int y = 0; y < _grid.Settings.kitchenSizeY; ++y)
                        {
                            switch (_grid.Tiles[x, y].TileType)
                            {
                                case TileType.WALKABLE:
                                    Gizmos.color = new Color(0, 1, 0, _gridTransparency);
                                    Gizmos.DrawCube(new Vector3(x, 0.5f, y), Vector3.one * 0.8f);
                                    break;
                                case TileType.BENCH:
                                    Gizmos.color = new Color(0, 0, 1, _gridTransparency);
                                    Gizmos.DrawCube(new Vector3(x, 0.5f, y), Vector3.one * 0.8f);
                                    break;
                                case TileType.TABLE:
                                    Gizmos.color = new Color(1, 0, 0, _gridTransparency);
                                    Gizmos.DrawCube(new Vector3(x, 0.5f, y), Vector3.one * 0.8f);
                                    break;
                                case TileType.ENTRYPOINT:
                                    Gizmos.color = new Color(0, 1, 1, _gridTransparency);
                                    Gizmos.DrawCube(new Vector3(x, 0.5f, y), Vector3.one * 0.8f);
                                    break;
                                case TileType.STORAGE:
                                    Gizmos.color = new Color(1, 0, 1, _gridTransparency);
                                    Gizmos.DrawCube(new Vector3(x, 0.5f, y), Vector3.one * 0.8f);
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}

