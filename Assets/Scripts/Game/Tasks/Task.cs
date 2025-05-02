using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Task
{
    public int id;
    public string text;
    public string baseCode;
    public List<Test> tests;
    public int difficulty;
}

[Serializable]
public class TaskProgress
{
    public int taskId;
    public bool isCompleted;
}

public class UserProgressInLevel : ISaveData
{
    public List<TaskProgress> СompletedTasks;
    
    public UserProgressInLevel(string _)
    {
        СompletedTasks = new List<TaskProgress>();
    }
    
    public void SetData(List<TaskProgress> completedTasks)
    {
        СompletedTasks = completedTasks;
        SaveSystem.Save(this);
    }

    public void SetData(List<Task> tasks)
    {
        if(СompletedTasks.Count == tasks.Count) return;
        
        List<TaskProgress> completedTasks = new List<TaskProgress>();
        foreach (var task in tasks)
        {
            completedTasks.Add(new TaskProgress
            {
                taskId = task.id,
                isCompleted = false
            });
        }
        SetData(completedTasks);
    }

    public void AddTask(Task task)
    {
        if(СompletedTasks?.FirstOrDefault(t => t.taskId == task.id) != null) return;
        СompletedTasks.Add(new TaskProgress
            {
                taskId = task.id,
                isCompleted = false
            });
        SaveSystem.Save(this);
    }
    
    public void MarkTaskCompleted(int taskId)
    {
        var progress = СompletedTasks.FirstOrDefault(p => p.taskId == taskId);
        if (progress != null)
        {
            progress.isCompleted = true;
        }
        else
        {
            СompletedTasks.Add(new TaskProgress
            {
                taskId = taskId,
                isCompleted = true
            });
        }
        SaveSystem.Save(this);
    }
    
    public bool IsTaskCompleted(int taskId)
    {
        return СompletedTasks.Any(p => p.taskId == taskId && p.isCompleted);
    }
    
    public (int completed, int total) GetTaskProgress()
    {
        int completed = СompletedTasks.Count(p => p.isCompleted);
        int total = СompletedTasks.Count;
        return (completed, total);
    }
}