using UnityEngine;
using UnityEngine.UI;

public class SwitchToggle : MonoBehaviour
{
    [SerializeField] private RectTransform handleRectTransform;
    [SerializeField] private Color backgroundActiveColor;
    [SerializeField] private Color handleActiveColor;
    
    private Toggle toggle;
    private Vector2 handlePosition;

    private Image backgroundImage, handleImage;
    private Color backgroundDefaultColor, handleDefaultColor;
    
    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        handlePosition = handleRectTransform.anchoredPosition;
        
        backgroundImage = handleRectTransform.parent.GetComponent<Image>();
        handleImage = handleRectTransform.GetComponent<Image>();

        backgroundDefaultColor = backgroundImage.color;
        handleDefaultColor = handleImage.color;

        toggle.onValueChanged.AddListener(OnSwitch);

        if (toggle.isOn)
        {
            OnSwitch(true);
        }
    }

    void OnSwitch(bool on)
    {
        handleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition;
        backgroundImage.color = on ? backgroundActiveColor : backgroundDefaultColor;
        handleImage.color = on ? handleActiveColor : handleDefaultColor;
    }

    void OnDestroy()
    {
        toggle.onValueChanged.RemoveAllListeners();
    }
}
