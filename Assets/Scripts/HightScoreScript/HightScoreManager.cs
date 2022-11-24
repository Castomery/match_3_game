using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HightScoreManager
{
    private JsonSaveSystem _saveSystem;
    public List<HightScoreEntry> HightScoreEntries { get; private set; }
    public HightScoreEntry CurrentHightScore { get; private set; }

    public HightScoreManager()
    {
        _saveSystem = new JsonSaveSystem();
        SaveData data = _saveSystem.Load();

        HightScoreEntries = data.GetHightScores();
        CurrentHightScore = data.GetMainHightScoreEntry();
    }

    public void SaveHightScoreEntry(int score, string username)
    {
        HightScoreEntries.Add(new HightScoreEntry(score, username));

        SaveData();
    }

    private void SaveData()
    {
        _saveSystem.Save(new SaveData(CurrentHightScore, HightScoreEntries));
    }
}
