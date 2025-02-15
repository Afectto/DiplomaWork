using System;
using UnityEngine;

public class TouchInputHandler : IInputHandlerStrategy
{
    private readonly Action<Vector3> _onStartDrag;
    private readonly Action<Vector3> _onDrag;
    private readonly Action<Vector3> _onEndDrag;
    private bool _isDragging;

    public TouchInputHandler(Action<Vector3> onStartDrag, Action<Vector3> onDrag, Action<Vector3> onEndDrag)
    {
        _onStartDrag = onStartDrag;
        _onDrag = onDrag;
        _onEndDrag = onEndDrag;
    }

    public void HandleInput(Vector2 inputPosition)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _onStartDrag?.Invoke(touch.position);
                    _isDragging = true;
                    break;
                case TouchPhase.Moved:
                    if (_isDragging)
                    {
                        _onDrag?.Invoke(touch.position);
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    _onEndDrag?.Invoke(touch.position);
                    _isDragging = false;
                    break;
            }
        }
    }
}