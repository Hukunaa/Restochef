using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Runtime.UI.KitchenEditor
{
    public class Drag : MonoBehaviour
    {
        private Vector3 _lastCursorPosSnapped;
        private KitchenEditorMovementHandler _handler;
        private bool _isDragged;

        private void Start()
        {
            _isDragged = false;
        }
        public void SetHandler(KitchenEditorMovementHandler _newHandler)
        {
            _handler = _newHandler;
        }

        private void OnMouseDown()
        {
            /*if(_handler != null)
            {
                _cursorOffset = _handler.SelectedTile.LinkedEntity.transform.position - _handler.CursorPosSnapped;
            }*/
        }

        private void OnMouseUp()
        {
            _isDragged = false;
        }

        private void OnMouseDrag()
        {
            if (_handler != null)
            {
                _isDragged = true;
                if (_handler.IsMovingEntity)
                {
                    if(_lastCursorPosSnapped != _handler.CursorPosSnapped)
                    {
                        _handler.SelectedTile.LinkedEntity.transform.DOKill();
                        _handler.SelectedTile.LinkedEntity.transform.DOMove(_handler.CursorPosSnapped + Vector3.up * 0.2f, 0.25f).SetEase(Ease.OutCirc);
                        _lastCursorPosSnapped = _handler.CursorPosSnapped;
                    }
                }
            }
        }
        public bool IsDragged { get => _isDragged; }
    }
}
