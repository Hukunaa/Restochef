using System.Collections.Generic;
using UnityEngine;

namespace Runtime.ScriptableObjects.DataContainers
{
    [CreateAssetMenu(fileName = "RarityColor", menuName = "ScriptableObjects/Settings/RarityColor", order = 0)]
    public class RarityColors : ScriptableObject
    {
        [SerializeField] 
        private Color[] _rarityColors;

        public Color[] Values => _rarityColors;
    }
}