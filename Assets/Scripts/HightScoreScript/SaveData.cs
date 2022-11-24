using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public HightScoreEntryDTO mainHightscoreEntry;

    public List<HightScoreEntryDTO> hightScores;

    public SaveData()
    {
        hightScores = new List<HightScoreEntryDTO>();
    }

    public SaveData(HightScoreEntry receivedHightScoreEntry, List<HightScoreEntry> receivedhightScoreEntries)
    {
        mainHightscoreEntry = new HightScoreEntryDTO(receivedHightScoreEntry);
        hightScores = new List<HightScoreEntryDTO>();

        foreach (HightScoreEntry hightScoreEntry in receivedhightScoreEntries)
        {
            hightScores.Add(new HightScoreEntryDTO(hightScoreEntry));
        }
    }

    public List<HightScoreEntry> GetHightScores()
    {
        List<HightScoreEntry> returnHightScoreEntries = new List<HightScoreEntry>();

        foreach (var data in hightScores)
        {
            returnHightScoreEntries.Add(new HightScoreEntry(data.score, data.name));
        }

        return returnHightScoreEntries;
    }

    public HightScoreEntry GetMainHightScoreEntry()
    {
        if (mainHightscoreEntry == null)
        {
            return new HightScoreEntry(0, "Main");
        }
        return new HightScoreEntry(mainHightscoreEntry.score, mainHightscoreEntry.name);
    }
}
