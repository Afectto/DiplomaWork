using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FindPathSetting
{
    public int StartX;
    public int StartY;
    public int EndX;
    public int EndY;
}
public class Pathfinding : MonoBehaviour
{
    [SerializeField] private bool isCanDiagonalMove = true;
    
    private Grid<GridObject> _grid;
    private List<GridObject> _openList;
    private List<GridObject> _closeList;

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private void Start()
    {
        _grid = FindAnyObjectByType<CreateGridObject>().Grid;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mosePosition = Utils.GetMouseWorldPosition();
            var gridObject = _grid.GetGridObject(mosePosition);
            List<GridObject> path = FindPath(new FindPathSetting { StartX = 0, StartY = 0, EndX = gridObject.x, EndY = gridObject.y});
            if (path != null)
            {
                float cellSize = _grid.GetCellSize();
                Vector3 gridOrigin = _grid.GetOriginPosition();

                for (var i = 0; i < path.Count - 1; i++)
                {
                    var pathNode = path[i];
                    var nextPathNode = path[i + 1];

                    Vector3 currentNodeCenter = gridOrigin + new Vector3(pathNode.x, pathNode.y) * cellSize + new Vector3(cellSize / 2, cellSize / 2);
                    Vector3 nextNodeCenter = gridOrigin + new Vector3(nextPathNode.x, nextPathNode.y) * cellSize + new Vector3(cellSize / 2, cellSize / 2);

                    Debug.DrawLine(currentNodeCenter, nextNodeCenter, Color.green, 10);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            var mosePosition = Utils.GetMouseWorldPosition();
            var gridObject = _grid.GetGridObject(mosePosition);
            gridObject.isWalkable = !gridObject.isWalkable;
        }
    }

    private List<GridObject> FindPath(FindPathSetting setting)
    {
        GridObject start = _grid.GetGridObject(setting.StartX, setting.StartY);
        GridObject end = _grid.GetGridObject(setting.EndX, setting.EndY);
        
        _openList = new List<GridObject>{start};
        _closeList = new List<GridObject>();

        for (int x = 0; x < _grid.GetWight(); x++)
        {
            for (int y = 0; y < _grid.GetHeight(); y++)
            {
                GridObject node = _grid.GetGridObject(x, y);
                node.gCost = Int32.MaxValue;
                node.CalculateFConst();
                node.cameFrom = null;
            }
        }

        start.gCost = 0;
        start.hCost = CalculateDistance(start, end);
        start.CalculateFConst();

        while (_openList.Count > 0)
        {
            var currentNode = GetLowestFConsNode(_openList);
            if (currentNode == end)
            {
                return CalculatePath(end);
            }

            _openList.Remove(currentNode);
            _closeList.Add(currentNode);

            foreach (var neighborNode in GetNeighborList(currentNode))
            {
                if(_closeList.Contains(neighborNode)) continue;
                if(!neighborNode.isWalkable)
                {
                    _closeList.Add(neighborNode);
                    continue;
                }
                
                int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighborNode);

                if (tentativeGCost < neighborNode.gCost)
                {
                    neighborNode.cameFrom = currentNode;
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = CalculateDistance(neighborNode, end);
                    neighborNode.CalculateFConst();
                    if (!_openList.Contains(neighborNode))
                    {
                        _openList.Add(neighborNode);
                    }
                }
            }
        }

        return null;
    }

    private List<GridObject> GetNeighborList(GridObject currentNode)
    {
        List<GridObject> neighborList = new List<GridObject>();
        if (currentNode.x - 1 >= 0)
        {
            //Left
            neighborList.Add(GetNode(currentNode.x - 1, currentNode.y));
            if (isCanDiagonalMove)
            {
                //Left Down
                if (currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
                //Left Up
                if (currentNode.y + 1 < _grid.GetHeight()) neighborList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
            }
        }
        if (currentNode.x + 1 < _grid.GetWight())
        {
            //Right
            neighborList.Add(GetNode(currentNode.x + 1, currentNode.y));
            if (isCanDiagonalMove)
            {
                //Right Down
                if (currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
                //Right Up
                if (currentNode.y + 1 < _grid.GetHeight())
                    neighborList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
            }
        }
        //Down
        if(currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x, currentNode.y - 1));
        //Up
        if(currentNode.y + 1 < _grid.GetHeight()) neighborList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighborList;
    }

    private GridObject GetNode(int x, int y)
    {
        return _grid.GetGridObject(x, y);
    }
    private List<GridObject> CalculatePath(GridObject endNode)
    {
        List<GridObject> path = new List<GridObject>();

        path.Add(endNode);
        GridObject currentNode = endNode;
        while (currentNode.cameFrom != null)
        {
            path.Add(currentNode.cameFrom);
            currentNode = currentNode.cameFrom;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistance(GridObject a, GridObject b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private GridObject GetLowestFConsNode(List<GridObject> pathNodeList)
    {
        GridObject lowestNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestNode.fCost)
            {
                lowestNode = pathNodeList[i];
            }
        }

        return lowestNode;
    }
}
