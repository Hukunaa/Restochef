using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.Managers
{
    [RequireComponent(typeof(NavMeshSurface))]
    public class NavBaker : MonoBehaviour
    {
        private NavMeshSurface _surface;

        private void Awake()
        {
            _surface = GetComponent<NavMeshSurface>();
        }

        public void BakeSurface()
        {
            _surface.BuildNavMesh();
        }
    }
}
