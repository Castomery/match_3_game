using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    public int score;
    public Text finalScoreText;

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
