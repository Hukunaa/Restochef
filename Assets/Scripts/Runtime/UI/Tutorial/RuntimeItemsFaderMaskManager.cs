using UnityEngine;

namespace Runtime.UI.Tutorial
{
    public class RuntimeItemsFaderMaskManager : MonoBehaviour
    {
        public void MaskChildren()
        {
            var tutorialFaderMasks = gameObject.GetComponentsInChildren<TutorialFaderMask>();

            foreach (var faderMask in tutorialFaderMasks)
            {
                faderMask.MaskFader();
            }
        }

        public void UnmaskChildren()
        {
            var tutorialFaderMasks = gameObject.GetComponentsInChildren<TutorialFaderMask>();
            
            foreach (var faderMask in tutorialFaderMasks)
            {
                faderMask.UnmaskFader();
            }
        }
    }
}