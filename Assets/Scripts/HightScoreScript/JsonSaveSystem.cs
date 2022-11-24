using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonSaveSystem
{
    private readonly string _filePath;

    public JsonSaveSystem()
    {
        _filePath = Application.persistentDataPath + "/save_data.json";
    }

    public void Save(SaveData saveData)
    {
        var json = JsonUtility.ToJson(saveData);

        using (StreamWriter writer = new StreamWriter(_filePath))
        {
            writer.WriteLine(json);
        }
    }

    public SaveData Load()
    {
        string receivedJson = "";

        if (!File.Exists(_filePath))
        {
            return new SaveData();
        }

        using(StreamReader reader = new StreamReader(_filePath))
        {
            receivedJson = reader.ReadToEnd();
        }

        if (string.IsNullOrEmpty(receivedJson))
        {
            return new SaveData();
        }

        return JsonUtility.FromJson<SaveData>(receivedJson);
    }
}
