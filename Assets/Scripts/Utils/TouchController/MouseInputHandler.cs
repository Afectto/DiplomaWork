using System;
using UnityEngine;

public class MouseInputHandler : IInputHandlerStrategy
{
    private readonly Action<Vector3> _onStartDrag;
    private readonly Action<Vector3> _onDrag;
    private readonly Action<Vector3> _onEndDrag;
    private bool _isDragging;

    public MouseInputHandler(Action<Vector3> onStartDrag, Action<Vector3> onDrag, Action<Vector3> onEndDrag)
    {
        _onStartDrag = onStartDrag;
        _onDrag = onDrag;
        _onEndDrag = onEndDrag;
    }

    public void HandleInput(Vector2 inputPosition)
    {
        if (Input.GetMouseButtonDown(0))
        {
            _onStartDrag?.Invoke(inputPosition);
            _isDragging = true;
        }
        if (Input.GetMouseButton(0) && _isDragging)
        {
            _onDrag?.Invoke(inputPosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            _onEndDrag?.Invoke(inputPosition);
            _isDragging = false;
        }
    }
}