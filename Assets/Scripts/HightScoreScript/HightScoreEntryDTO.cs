using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HightScoreEntryDTO
{
    public int score;
    public string name;

    public HightScoreEntryDTO(HightScoreEntry hightScoreEntry)
    {
        score = hightScoreEntry.score;
        name = hightScoreEntry.name;
    }
}
