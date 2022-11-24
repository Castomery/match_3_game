using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HightScoreTemplate : MonoBehaviour
{
    [SerializeField] private Text _rank;
    [SerializeField] private Text _score;
    [SerializeField] private Text _username;

    public void UpdateData(string rank, string score, string username)
    {
        _rank.text = rank;
        _score.text = score;
        _username.text = username;
    }
}
