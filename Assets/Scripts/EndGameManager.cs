using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType
{
    Moves,
    Time
}
[System.Serializable]
public class EndGameRequirments
{
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{
    public GameObject gameOverMenuUI;
    public Text counter;
    public EndGameRequirments requirments;
    private Board _board;
    public int currentCounterValue;
    private float timerSeconds = 1;

    // Start is called before the first frame update
    void Start()
    {
        _board = FindObjectOfType<Board>();
        SetUpGame();
    }

    private void SetUpGame()
    {
        _board.DeacreasMovesAction += DecreaseCounterValue;
        _board.GameOverAction += ShowGameOverUI;
        currentCounterValue = requirments.counterValue;
        counter.text = currentCounterValue.ToString();
    }

    public void DecreaseCounterValue()
    {
        currentCounterValue--;
        counter.text = currentCounterValue.ToString();

        if (currentCounterValue <= 0)
        {
            _board.currentState = GameState.gameOver;
            Debug.Log("Game Over!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (requirments.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }

    public void ShowGameOverUI()
    {
        gameOverMenuUI.SetActive(true);
    }
}
