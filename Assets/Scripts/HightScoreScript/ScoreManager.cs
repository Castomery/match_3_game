using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private int score;
    [SerializeField] private Text finalScoreText;

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
        finalScoreText.text = scoreText.text;
    }

    public void IncreaceScore(int amountToIncrease)
    {
        score += amountToIncrease;
    }
}
