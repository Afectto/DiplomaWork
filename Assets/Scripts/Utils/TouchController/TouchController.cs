using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class TouchController : ITickable
{
    private IInputHandlerStrategy _inputHandlerStrategy;
    private GraphicRaycaster _graphicRaycaster;
    private bool _isNeedWork;

    public event Action<Vector3> OnStartDrag;
    public event Action<Vector3> OnDrag;
    public event Action<Vector3> OnEndDrag;

    [Inject]
    private void Inject()
    {
        _isNeedWork = true;
        _graphicRaycaster = GameObject.FindWithTag("CanvasOverlay")?.GetComponent<GraphicRaycaster>();
        
        _inputHandlerStrategy = Input.touchSupported
            ? new TouchInputHandler(InvokeStartDrag, InvokeDrag, InvokeEndDrag)
            : new MouseInputHandler(InvokeStartDrag, InvokeDrag, InvokeEndDrag);
    }

    private void InvokeStartDrag(Vector3 position) => OnStartDrag?.Invoke(position);
    private void InvokeDrag(Vector3 position) => OnDrag?.Invoke(position);
    private void InvokeEndDrag(Vector3 position) => OnEndDrag?.Invoke(position);
    
    public void SetNeedWork(bool value)
    {
        _isNeedWork = value;
    }

    public void Tick()
    {
        if (Time.timeScale == 0 || !_isNeedWork) return;

        Vector2 inputPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;

        if (IsPointerOverUI(inputPosition)) return;

        _inputHandlerStrategy.HandleInput(inputPosition);
    }

    private bool IsPointerOverUI(Vector2 position)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current) { position = position };
        List<RaycastResult> results = new List<RaycastResult>();
        _graphicRaycaster.Raycast(pointerEventData, results);
        return results.Count > 1;
    }
}