using System;
using UnityEngine;

[Serializable]
public class TaskData
{
    public MyVector3 Position; 
    public int ID; 
    public bool IsCompleted;
    
    public TaskData(string _) { }
    public TaskData() { }
    
    public TaskData(Vector2Int taskKey, int valueID, bool isCompleted)
    {
        Position = taskKey;
        ID = valueID;
        IsCompleted = isCompleted;
    }
}