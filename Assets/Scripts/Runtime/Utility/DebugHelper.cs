using UnityEngine;

namespace Runtime.Utility
{
    public static class DebugHelper
    {
        public static void PrintDebugMessage(string _message, bool _show = true)
        {
            if (_show == false) return;
            Debug.Log(_message);
        }
    }
}