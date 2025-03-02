using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Zenject;

public class TaskManager
{
    private Dictionary<Vector2Int, Task> _tasks = new Dictionary<Vector2Int, Task>();
    private List<Task> _tasksJson;

    [Inject]
    private void Inject()
    {
        LoadTasks();
    }
    
    public void AssignTaskToTile(Vector2Int position, Task task)
    {
        if (!_tasks.ContainsKey(position))
        {
            _tasks.Add(position, task);
        }
    }

    public Task GetTaskForTile(Vector2Int position)
    {
        return _tasks.ContainsKey(position) ? _tasks[position] : null;
    }

    public void GenerateTasks(Dictionary<Vector2Int, TileManager.TileData> tileManagerTileDataInfo, (int minTasksDifficulty, int maxTasksDifficulty) difficultyRange)
    {
        var filteredTasks = _tasksJson
            .Where(task => task.difficulty >= difficultyRange.minTasksDifficulty && task.difficulty <= difficultyRange.maxTasksDifficulty)
            .ToList();

        if (!filteredTasks.Any())
        {
            Debug.LogWarning("No tasks found in the specified difficulty range!");
            return;
        }

        foreach (var tile in tileManagerTileDataInfo)
        {
            if (tile.Value.Type == TileManager.TileType.Interest)
            {
                var randomTask = filteredTasks[Random.Range(0, filteredTasks.Count)];
                AssignTaskToTile(tile.Key, randomTask);
                filteredTasks.Remove(randomTask);
            }
        }
    }
    
    private void LoadTasks()
    {
        string jsonFilePath = Path.Combine(Application.streamingAssetsPath, "Tasks.json");
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            _tasksJson = JsonUtility.FromJson<TasksWrapper>(json).tasks;
            Debug.Log("Задачи загружены успешно.");
        }
        else
        {
            Debug.LogError("Файл задач не найден!");
        }
    }
}