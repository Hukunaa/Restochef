using UnityEngine;
using UnityEngine.UI;

namespace Voodoo.Sauce.Internal
{
    public class PrivacySettingsButton : MonoBehaviour
    {
        [SerializeField] private Button gdprButton;

        private void Start()
        {
            gdprButton.onClick.AddListener(PrivacyScreenUIManager.Instance.OpenPrivacyScreen);
        }
    }
}
