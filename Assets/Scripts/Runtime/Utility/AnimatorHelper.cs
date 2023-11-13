using UnityEngine;

namespace Runtime.Utility
{
    public static class AnimatorHelper
    {
        public static void ResetAllAnimatorTriggers(Animator _animator)
        {
            foreach (var trigger in _animator.parameters)
            {
                if (trigger.type == AnimatorControllerParameterType.Trigger)
                {
                    _animator.ResetTrigger(trigger.name);
                }
            }
        }
    }
}