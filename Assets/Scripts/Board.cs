using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GameState
{
    wait,
    move,
    pause,
    gameOver
}

public enum TileKind
{
    Brekable,
    Blank,
    Normal
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

[System.Serializable]
public class Matchtype
{
    public int type;
    public string color;
}

public class Board : MonoBehaviour
{
    [Header("Tiles")]
    public GameObject lightTile;
    public GameObject darkTile;
    private bool isDark = false;

    public GameState currentState;
    public int width;
    public int height;
    public int offSet;

    public Bomb columnArrow;
    public Bomb rowArrow;
    public Bomb colorBomb;
    public Bomb adjacentBomb;

    public GameObject breakableTile;
    public GameObject destroyEffect;

    public Dot[] dots;
    public Dot[,] allDots;
    public Dot currentDot;

    public TileType[] boarLayout;
    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    private FindMatches findMatches;
    public Transform piecesHolder;
    public Transform tileHolder;

    public int basePieceValue = 20;
    private int streakValue = 1;
    private ScoreManager scoreManager;
    public float refillDelay = 0.3f;

    public event Action GameOverAction;
    public event Action DeacreasMovesAction;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allDots = new Dot[width, height];
        breakableTiles = new BackgroundTile[width, height];
        SetUp();
    }

    private void Update()
    {
        if (currentState != GameState.gameOver)
        {
            if (PauseMenu.GameIsPaused)
            {
                currentState = GameState.pause;
            }
            else
            {
                currentState = GameState.move;
            }
        }
    }

    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < boarLayout.Length; i++)
        {
            if (boarLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boarLayout[i].x, boarLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakbleTiles()
    {
        for (int i = 0; i < boarLayout.Length; i++)
        {
            if (boarLayout[i].tileKind == TileKind.Brekable)
            {
                Vector2 tempPos = new Vector2(boarLayout[i].x, boarLayout[i].y);
                GameObject tile = Instantiate(breakableTile, tempPos, Quaternion.identity);
                breakableTiles[boarLayout[i].x, boarLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }


    private void SetUp()
    {
        GenerateBlankSpaces();
        GenerateBreakbleTiles();
        for (int i = 0; i < width; i++)
        {

            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j);
                    Vector2 dotTempPosition = new Vector2(i, j + offSet);
                    GameObject currTile = null;
                    if (isDark)
                    {
                        currTile = Instantiate(darkTile, tempPosition, Quaternion.identity);
                    }
                    else
                    {
                        currTile = Instantiate(lightTile, tempPosition, Quaternion.identity);
                    }

                    if (currTile != null)
                    {
                        currTile.transform.parent = tileHolder.transform;
                    }


                    //int dotToUse = Random.Range(0, dots.Length);
                    //while (MatchesAt(i, j, dots[dotToUse]))
                    //{
                    //    dotToUse = Random.Range(0, dots.Length);
                    //}
                    //Dot dot = Instantiate(dots[dotToUse], dotTempPosition, Quaternion.identity);
                    //dot.OnDeacreasMovesAction += () => { DeacreasMovesAction?.Invoke(); };
                    //dot.transform.parent = piecesHolder.transform;


                    //dot.row = j;
                    //dot.colum = i;
                    //allDots[i, j] = dot;
                }
                isDark = !isDark;

            }
            isDark = !isDark;
        }
        RefillBoard();
    }

    private bool MatchesAt(int column, int row, Dot dot)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == dot.tag && allDots[column - 2, row].tag == dot.tag)
                {
                    return true;
                }
            }
        }
        if (row > 1)
        {
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == dot.tag && allDots[column, row - 2].tag == dot.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private Matchtype ColumnOrRow()
    {

        List<Dot> matchCopy = findMatches.currentMatches;
        Matchtype matchType = new Matchtype();
        matchType.type = 0;
        matchType.color = "";

        for (int i = 0; i < matchCopy.Count; i++)
        {
            Dot thisDot = matchCopy[i];
            string color = matchCopy[i].tag;
            int column = thisDot.colum;
            int row = thisDot.row;
            int columnMatch = 0;
            int rowMatch = 0;

            for (int j = 0; j < matchCopy.Count; j++)
            {
                Dot nextDot = matchCopy[j];
                if (nextDot == thisDot)
                {
                    continue;
                }
                if (nextDot.colum == thisDot.colum && nextDot.tag == color)
                {
                    columnMatch++;
                }
                if (nextDot.row == thisDot.row && nextDot.tag == color)
                {
                    rowMatch++;
                }
            }

            if (columnMatch == 4 || rowMatch == 4)
            {
                matchType.type = 1;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 2 && rowMatch == 2)
            {
                matchType.type = 2;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 3 || rowMatch == 3)
            {
                matchType.type = 3;
                matchType.color = color;
                return matchType;
            }
        }

        return matchType;

    }

    private void CheckToMakeBombs()
    {

        if (findMatches.currentMatches.Count > 3)
        {

            Matchtype typeOfMatch = ColumnOrRow();

            if (typeOfMatch.type == 1)
            {
                if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                {

                    currentDot.isMatched = false;
                    MakeColorBomb(currentDot);

                }
                else
                {
                    if (currentDot.otherDot != null)
                    {
                        Dot otherDot = currentDot.otherDot;
                        if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                        {
                            otherDot.isMatched = false;
                            MakeColorBomb(otherDot);

                        }
                    }
                }


            }
            else if (typeOfMatch.type == 2)
            {
                if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                {


                    currentDot.isMatched = false;
                    MakeAdjacentBomb(currentDot);

                }
                else
                {
                    if (currentDot.otherDot != null)
                    {
                        Dot otherDot = currentDot.otherDot;
                        if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                        {
                            otherDot.isMatched = false;
                            MakeAdjacentBomb(otherDot);
                        }
                    }
                }

            }
            else if (typeOfMatch.type == 3)
            {
                findMatches.Checkbombs(typeOfMatch);
            }
        }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].isMatched)
        {

            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
            Destroy(allDots[column, row].gameObject);


            scoreManager.IncreaceScore(basePieceValue * streakValue);
            allDots[column, row] = null;           
        }
    }

    private void CheckForGameOver()
    {
        if (currentState == GameState.gameOver)
        {
            GameOverAction?.Invoke();
        }
    }

    public void DestroyMatches()
    {

        if (findMatches.currentMatches.Count >= 4)
        {
            CheckToMakeBombs();
        }

        foreach (var item in findMatches.currentMatches)
        {
            DestroyMatchesAt(item.colum, item.row);
        }

        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCor2());

    }

    private IEnumerator DecreaseRowCor2()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && allDots[i, j] == null)
                {
                    for (int k = j + 1; k < height; k++)
                    {
                        if (allDots[i, k] != null)
                        {
                            allDots[i, k].row = j;
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.2f);
        StartCoroutine(FillBoardCor());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i, j])
                {
                    Vector2 tempPos = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);

                    while (MatchesAt(i, j, dots[dotToUse]))
                    {
                        dotToUse = Random.Range(0, dots.Length);
                    }

                    Dot dot = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                    dot.OnDeacreasMovesAction += () => { DeacreasMovesAction?.Invoke(); };
                    dot.transform.parent = piecesHolder.transform;

                    dot.row = j;
                    dot.colum = i;
                    allDots[i, j] = dot;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        findMatches.FindAllMatches();
        if (findMatches.currentMatches.Count >= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator FillBoardCor()
    {

        yield return new WaitForSeconds(refillDelay);
        RefillBoard();
        findMatches.FindAllMatches();
        yield return new WaitForSeconds(refillDelay);

        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield break;
        }

        CheckForGameOver();

        currentDot = null;
        yield return new WaitForSeconds(refillDelay);
        if (IsDeadlocked())
        {
            ShuffleBoard();
        }

        if (currentState != GameState.pause && currentState != GameState.gameOver)
        {
            currentState = GameState.move;
        }
        streakValue = 1;
    }
    private void SwitchDots(int column, int row, Vector2 dir)
    {
        Dot holder = allDots[column + (int)dir.x, row + (int)dir.y];
        allDots[column + (int)dir.x, row + (int)dir.y] = allDots[column, row];
        allDots[column, row] = holder;
    }

    private bool CheckForMatches()
    {
        findMatches.FindAllMatches();

        if (findMatches.currentMatches.Count >= 3)
        {
            findMatches.currentMatches.Clear();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SwitchAndCheck(int column, int row, Vector2 dir)
    {
        SwitchDots(column, row, dir);
        if (CheckForMatches())
        {
            SwitchDots(column, row, dir);
            return true;
        }
        SwitchDots(column, row, dir);
        return false;
    }

    private bool IsDeadlocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }

                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private void ShuffleBoard()
    {
        List<Dot> newBoard = new List<Dot>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    int dotToUse = Random.Range(0, newBoard.Count);
                    while (MatchesAt(i, j, newBoard[dotToUse]))
                    {
                        dotToUse = Random.Range(0, newBoard.Count);
                    }

                    Dot dot = newBoard[dotToUse].GetComponent<Dot>();
                    dot.colum = i;
                    dot.row = j;
                    allDots[i, j] = newBoard[dotToUse];
                    newBoard.Remove(newBoard[dotToUse]);
                }
            }
        }

        if (IsDeadlocked())
        {
            ShuffleBoard();
        }
    }

    public void MakeRowBomb(Dot dot)
    {
        if (dot.tag != "ColumnBomb" && dot.tag != "ColorBomb" && dot.tag != "AdjacentBomb")
        {
            Bomb rowBomb = Instantiate(rowArrow, dot.transform.position, dot.transform.rotation);
            rowBomb.colum = dot.colum;
            rowBomb.row = dot.row;
            allDots[dot.colum, dot.row] = rowBomb;
            rowBomb.OnDeacreasMovesAction += () => { DeacreasMovesAction?.Invoke(); };
            Destroy(dot.gameObject);

        }
    }

    public void MakeColumnBomb(Dot dot)
    {
        if (dot.tag != "RowBomb" && dot.tag != "ColorBomb" && dot.tag != "AdjacentBomb")
        {
            Bomb columnB = Instantiate(columnArrow, dot.transform.position, dot.transform.rotation);
            columnB.colum = dot.colum;
            columnB.row = dot.row;
            allDots[dot.colum, dot.row] = columnB;
            columnB.OnDeacreasMovesAction += () => { DeacreasMovesAction?.Invoke(); };
            Destroy(dot.gameObject);
        }
    }

    private void MakeColorBomb(Dot dot)
    {
        if (dot.tag != "RowBomb" && dot.tag != "ColumnBomb" && dot.tag != "AdjacentBomb")
        {
            Bomb colorB = Instantiate(colorBomb, dot.transform.position, dot.transform.rotation);
            colorB.colum = dot.colum;
            colorB.row = dot.row;
            allDots[dot.colum, dot.row] = colorB;
            colorB.OnDeacreasMovesAction += () => { DeacreasMovesAction?.Invoke(); };
            Destroy(dot.gameObject);
        }
    }

    private void MakeAdjacentBomb(Dot dot)
    {

        if (dot.tag != "RowBomb" && dot.tag != "ColumnBomb" && dot.tag != "ColorBomb")
        {
            Bomb adjB = Instantiate(adjacentBomb, dot.transform.position, dot.transform.rotation);
            adjB.colum = dot.colum;
            adjB.row = dot.row;
            allDots[dot.colum, dot.row] = adjB;
            adjB.OnDeacreasMovesAction += () => { DeacreasMovesAction?.Invoke(); };
            Destroy(dot.gameObject);
        }
    }
}
