using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Runtime.UI.KitchenEditor
{
    public class SelectionPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject _infoButton;
        [SerializeField]
        private GameObject _moveButton;
        [SerializeField]
        private GameObject _rotateButton;
        [SerializeField]
        private GameObject _validateButton;
        [SerializeField]
        private TMP_Text _text;

        private void Start()
        {
            ShowAll(false);
        }

        public void ShowAll(bool _value)
        {
            _infoButton.SetActive(_value);
            _moveButton.SetActive(_value);
            _rotateButton.SetActive(_value);
            _validateButton.SetActive(_value);
            _text.gameObject.SetActive(_value);
        }

        public void ShowValidateButton(bool _value)
        {
            _validateButton.SetActive(_value);
        }
        public void ShowRotateButton(bool _value)
        {
            _rotateButton.SetActive(_value);
        }
        public void ShowText(bool _value)
        {
            _text.gameObject.SetActive(_value);
        }
        public void ShowInfoButton(bool _value)
        {
            _infoButton.SetActive(_value);
        }
        public void ShowMoveButton(bool _value)
        {
            _moveButton.SetActive(_value);
        }
        public void SetInfoText(string _newtext)
        {
            _text.text = _newtext;
        }
    }
}
