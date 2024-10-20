using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PathfindingJob
{
    private Grid<GridObject> _grid;
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public PathfindingJob(Grid<GridObject> grid)
    {
        _grid = grid;
    }

    public List<int2> FindPath(FindPathSetting setting, bool canDiagonalMove)
    {
        NativeList<int2> path = new NativeList<int2>(Allocator.TempJob);
        NativeArray<bool> isWalkableArray = new NativeArray<bool>(_grid.GetWight() * _grid.GetHeight(), Allocator.TempJob);

        for (int x = 0; x < _grid.GetWight(); x++)
        {
            for (int y = 0; y < _grid.GetHeight(); y++)
            {
                var index = CalculateIndex(x, y, _grid.GetWight());
                isWalkableArray[index] = setting.IsWalkableArray[index];
            }
        }
        var findPathJob = new FindPathJob
        {
            startPosition = new int2(setting.StartX, setting.StartY),
            endPosition = new int2(setting.EndX, setting.EndY),
            gridSize = new int2(_grid.GetWight(), _grid.GetHeight()),
            pathJob = path,
            IsCanDiagonalMove = canDiagonalMove,
            isWalkableArray = isWalkableArray
        };
        JobHandle jobHandle = findPathJob.Schedule();
        jobHandle.Complete();

        List<int2> resultingPath = new List<int2>();
        foreach (var pos in path)
        {
            resultingPath.Add(pos);
        }

        path.Dispose();
        isWalkableArray.Dispose();

        return resultingPath;
    }

    private int CalculateIndex(int x, int y, int gridWidth)
    {
        return x + y * gridWidth;
    }

    [BurstCompile]
    private struct FindPathJob : IJob
    {
        public int2 startPosition;
        public int2 endPosition;
        public int2 gridSize;
        public bool IsCanDiagonalMove;
        public NativeArray<bool> isWalkableArray;

        public NativeList<int2> pathJob;

        public void Execute()
        {
            NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PathNode pathNode = new PathNode();
                    pathNode.x = x;
                    pathNode.y = y;
                    pathNode.index = CalculateIndex(x, y, gridSize.x);

                    pathNode.gCost = int.MaxValue;
                    pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
                    pathNode.CalculateFCost();
                    
                    pathNode.isWalkable = isWalkableArray[pathNode.index];
                    pathNode.cameFromNodeIndex = -1;

                    pathNodeArray[pathNode.index] = pathNode;
                }
            }

            NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
            neighbourOffsetArray[0] = new int2(-1, 0); // Left
            neighbourOffsetArray[1] = new int2(+1, 0); // Right
            neighbourOffsetArray[2] = new int2(0, +1); // Up
            neighbourOffsetArray[3] = new int2(0, -1); // Down
            neighbourOffsetArray[4] = new int2(-1, -1); // Left Down
            neighbourOffsetArray[5] = new int2(-1, +1); // Left Up
            neighbourOffsetArray[6] = new int2(+1, -1); // Right Down
            neighbourOffsetArray[7] = new int2(+1, +1); // Right Up

            int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);

            PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            pathNodeArray[startNode.index] = startNode;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.index);

            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
                PathNode currentNode = pathNodeArray[currentNodeIndex];

                if (currentNodeIndex == endNodeIndex)
                {
                    break;
                }

                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closedList.Add(currentNodeIndex);

                for (int i = 0; i < (IsCanDiagonalMove ? 8 : 4); i++)
                {
                    int2 neighbourOffset = neighbourOffsetArray[i];
                    int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                    if (!IsPositionInsideGrid(neighbourPosition, gridSize))
                    {
                        continue;
                    }

                    int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);
                    PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];

                    if (closedList.Contains(neighbourNodeIndex) || !neighbourNode.isWalkable)
                    {
                        continue;
                    }
                    
                    int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

                    int tentativeGCost =
                        currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNodeIndex = currentNodeIndex;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.CalculateFCost();
                        pathNodeArray[neighbourNodeIndex] = neighbourNode;

                        if (!openList.Contains(neighbourNode.index))
                        {
                            openList.Add(neighbourNode.index);
                        }
                    }

                }
            }

            PathNode endNode = pathNodeArray[endNodeIndex];
            if (endNode.cameFromNodeIndex != -1)
            {
                CalculatePath(pathNodeArray, endNode);
            }

            pathNodeArray.Dispose();
            neighbourOffsetArray.Dispose();
            openList.Dispose();
            closedList.Dispose();
        }

        private void CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
        {
            pathJob.Add(new int2(endNode.x, endNode.y));

            PathNode currentNode = endNode;
            while (currentNode.cameFromNodeIndex != -1)
            {
                PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                pathJob.Add(new int2(cameFromNode.x, cameFromNode.y));
                currentNode = cameFromNode;
            }
        }

        private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
        {
            return
                gridPosition.x >= 0 &&
                gridPosition.y >= 0 &&
                gridPosition.x < gridSize.x &&
                gridPosition.y < gridSize.y;
        }

        private int CalculateIndex(int x, int y, int gridWidth)
        {
            return x + y * gridWidth;
        }

        private int CalculateDistanceCost(int2 aPosition, int2 bPosition)
        {
            int xDistance = math.abs(aPosition.x - bPosition.x);
            int yDistance = math.abs(aPosition.y - bPosition.y);
            int remaining = math.abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }


        private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
        {
            PathNode lowestCostPathNode = pathNodeArray[openList[0]];
            for (int i = 1; i < openList.Length; i++)
            {
                PathNode testPathNode = pathNodeArray[openList[i]];
                if (testPathNode.fCost < lowestCostPathNode.fCost)
                {
                    lowestCostPathNode = testPathNode;
                }
            }

            return lowestCostPathNode.index;
        }

        private struct PathNode
        {
            public int x;
            public int y;

            public int index;

            public int gCost;
            public int hCost;
            public int fCost;

            public bool isWalkable;

            public int cameFromNodeIndex;

            public void CalculateFCost()
            {
                fCost = gCost + hCost;
            }
        }

    }
}