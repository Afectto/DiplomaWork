using System;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;

    private static readonly Vector3 PositionOffset = new Vector3(0.5f,.5f);

    private MapGenerator _mapGenerator;
    private Grid<GridObject> _grid;

    [Inject]
    private void Inject(MapGenerator mapGenerator)
    {
        _mapGenerator = mapGenerator;
    }

    private void Start()
    {
        _grid = _mapGenerator.GetGrid();
        transform.position = _grid.GetWorldPositionByCenterCell(0, 0);
    }

    private void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector3 newPosition = transform.localPosition + new Vector3(moveX, moveY, 0) * speed * Time.deltaTime;

        if (CanPlayerEnterCell(newPosition))
        {
            transform.localPosition = newPosition;
        }
    }

    private bool CanPlayerEnterCell(Vector3 worldPosition)
    {
        var offsetPosition = worldPosition;
        int x, y;
        _grid.GetXY(offsetPosition, out x, out y);
        var cell = _grid.GetGridObject(x, y);
        return cell != null && cell.isWalkable;
    }


}