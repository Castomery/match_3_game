using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HightScoreEntry :IComparable<HightScoreEntry>
{
    public int score;
    public string name; 

    public HightScoreEntry(int score, string name)
    {
        this.score = score;
        this.name = name;   
    }

    public int CompareTo(HightScoreEntry other)
    {
        return score.CompareTo(other.score);
    }
}
