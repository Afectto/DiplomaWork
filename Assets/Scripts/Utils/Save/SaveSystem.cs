using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;

public class SaveSystem
{
    public static T Save<T>(T data) where T : ISaveData
    {
        string contents = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(Path<T>(), contents);
        return data;
    }

    public static T Load<T>() where T : ISaveData
    {
        string path = Path<T>();
        if (!File.Exists(path))
            return Save((T)Activator.CreateInstance(typeof(T), ""));
        string json = File.ReadAllText(path);
        var data = JsonConvert.DeserializeObject<T>(json);
        data ??= Save((T)Activator.CreateInstance(typeof(T), ""));
        return data;
    }

    private static string Path<T>() where T : ISaveData
    {
        return Application.persistentDataPath + "/" + typeof(T).Name + ".json";
    }
}