using System;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    
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

    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector3 newPosition = transform.localPosition + new Vector3(moveX, moveY, 0) * speed * Time.deltaTime;

        if (CanPlayerEnterCell(newPosition))
        {
            transform.localPosition = newPosition;
        }
        else
        {
            transform.localPosition = newPosition - 4 * new Vector3(moveX, moveY, 0) * speed * Time.deltaTime;
        }
    }

    private bool CanPlayerEnterCell(Vector3 worldPosition)
    {
        int x, y;
        _grid.GetXY(worldPosition, out x, out y);
        var cell = _grid.GetGridObject(x, y);
        return cell != null && cell.isWalkable;
    }


}