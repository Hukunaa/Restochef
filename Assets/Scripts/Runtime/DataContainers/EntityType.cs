using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.DataContainers
{
    public class EntityType : MonoBehaviour
    {
        [SerializeReference]
        private string _type;
        [SerializeReference]
        private string _name;
        [SerializeReference]
        private string _description;
        public string Type { get => _type; }
        public string Name { get => _name; }
        public string Description { get => _description; }
    }
}
