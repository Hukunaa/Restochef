using System;
using UnityEngine;

namespace Runtime.DataContainers.Stats
{
    [Serializable]
    public enum SKILL_TYPE
    {
        DEXTERITY,
        DETAIL,
        INTUITION
    }

    [Serializable]
    public class Skill
    {
        [SerializeField]
        private SKILL_TYPE _type;
        [SerializeField]
        private string _name;
        [SerializeField]
        private int _level;
        [SerializeField]

        public event Action<int> OnLevelUpdated;

        public Skill(string _skillName, SKILL_TYPE _skillType, int _startingLevel, int[] _range, float _baseMultiplier = 1.0f)
        {
            _name = _skillName;
            _type = _skillType;
            _level = _startingLevel;
        }

        public void LevelUp()
        {
            _level += 1;
            OnLevelUpdated?.Invoke(_level);
        }

        public void LevelDown()
        {
            _level -= 1;
            OnLevelUpdated?.Invoke(_level);
        }

        public void ResetLevel()
        {
            _level = 1;
            OnLevelUpdated?.Invoke(_level);
        }

        public SKILL_TYPE Type { get => _type; }
        public int Level { get => _level; }
        public string Name { get => _name; }
    }
}
