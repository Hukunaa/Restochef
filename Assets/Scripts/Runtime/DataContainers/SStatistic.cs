using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.DataContainers
{
    [System.Serializable]
    public struct SStatistic
    {
        private string _statName;
        private int _min;
        private int _max;
        private int _value;

        public SStatistic(string _name, int _Min, int _Max, int _Value)
        {
            _statName = _name;
            _min = _Min;
            _max = _Max;

            _value = Mathf.Clamp(_Value, _min, _max);
        }

        public int Min { get => _min; }
        public int Max { get => _max; }
        public int Value { get => _value; }
        public string StatName { get => _statName; }
    }
}
