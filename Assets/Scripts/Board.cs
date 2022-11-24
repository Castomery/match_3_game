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
    [SerializeField] private Tile lightTile;
    [SerializeField] private Tile darkTile;
    [SerializeField] private bool isDark = false;

    [Header("Board Settings")]
    public GameState currentState;
    public int width;
    public int height;
    [SerializeField] private int offSet;

    [Header("Bombs")]
    [SerializeField] private Bomb columnArrow;
    [SerializeField] private Bomb rowArrow;
    [SerializeField] private Bomb colorBomb;
    [SerializeField] private Bomb adjacentBomb;

    //public GameObject breakableTile;
    public GameObject destroyEffect;

    [Header("Dots")]
    [SerializeField] private Dot[] dots;
    private ObjectPool[] _dotPools;
    public Dot[,] allDots;
    public Dot currentDot;

    [SerializeField] private TileType[] boarLayout;
    private bool[,] blankSpaces;
    private FindMatches findMatches;
    private SoundManager _soundManager;
    [SerializeField] private Transform piecesHolder;
    [SerializeField] private Transform tileHolder;
    public bool _isChangesEnds = true;

    [Header("Score Settings")]
    [SerializeField] private int basePieceValue = 20;
    [SerializeField] private int streakValue = 1;
    private ScoreManager scoreManager;
    [SerializeField] private float refillDelay = 0.2f;

    public event Action GameOverAction;
    public event Action DeacreasMovesAction;

    void Start()
    {
        _soundManager = FindObjectOfType<SoundManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allDots = new Dot[width, height];
        InitDotPools();
        SetUp();
    }

    private void InitDotPools()
    {
        _dotPools = new ObjectPool[dots.Length];

        for (int i = 0; i < dots.Length; i++)
        {
            _dotPools[i] = new ObjectPool(20, dots[i], piecesHolder);  
        }
    }

    private void Update()
    {
        if (currentState != GameState.gameOver)
        {
            if (GameMenu.GameIsPaused)
            {
                currentState = GameState.pause;
            }
            else
            {
                if (_isChangesEnds)
                {
                    currentState = GameState.move;
                }  
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

    private void SetUp()
    {
        GenerateBlankSpaces();
        //GenerateBreakbleTiles();
        for (int i = 0; i < width; i++)
        {

            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j);
                    
                    Tile currTile = null;
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
                }
                isDark = !isDark;

            }
            isDark = !isDark;
        }
        RefillBoard();
    }

    private bool MatchesAt(int column, int row, Dot dot)
    {
        if (column > 1)
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

        for (int i = 0; i < matchCopy.Count; i++)
        {
            Dot thisDot = matchCopy[i];
            string color = matchCopy[i].tag;
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
                Checkbombs(typeOfMatch);
            }
        }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].isMatched)
        {
            if (_soundManager != null)
            {
                _soundManager.PlayDestroyAudioClip();
            }
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
            if (allDots[column,row] is Bomb)
            {
                Destroy(allDots[column, row].gameObject);
            }
            else
            {
                allDots[column, row].isMatched = false;
                allDots[column, row].ReturnToPool();
            }
            
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
            if (currentDot != null && currentDot.tag != "ColorBomb")
            {
                CheckToMakeBombs();
            }
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

                    Dot dot = _dotPools[dotToUse].Get();
                    dot.InitObjectPool(_dotPools[dotToUse]);
                    dot.transform.position = tempPos;
                    dot.OnDeacreasMovesAction += () => { DeacreasMovesAction?.Invoke(); };            

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

        _isChangesEnds = true;
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

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    if (i < width -2)
                    {
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }

                    if (j < height-2)
                    {
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }  
                }
            }
            
        }
        return false;
        //findMatches.FindAllMatches();

        //if (findMatches.currentMatches.Count >= 3)
        //{
        //    for (int i = 0; i < findMatches.currentMatches.Count; i++)
        //    {
        //        findMatches.currentMatches[i].isMatched = false;
        //    }
        //    findMatches.currentMatches.Clear();
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
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

    public void Checkbombs(Matchtype matchtype)
    {
        if (currentDot != null)
        {
            if (currentDot.isMatched && currentDot.tag == matchtype.color)
            {
                currentDot.isMatched = false;

                if ((currentDot.swipeAngle > -45 && currentDot.swipeAngle <= 45)
                    || (currentDot.swipeAngle < -135 || currentDot.swipeAngle >= 135))
                {
                    MakeRowBomb(currentDot);
                }
                else
                {
                    MakeColumnBomb(currentDot);
                }
            }
            else if (currentDot.otherDot != null)
            {
                Dot otherDot = currentDot.otherDot;
                if (otherDot.isMatched && otherDot.tag == matchtype.color)
                {
                    otherDot.isMatched = false;

                    if ((currentDot.swipeAngle > -45 && currentDot.swipeAngle <= 45)
                        || (currentDot.swipeAngle < -135 || currentDot.swipeAngle >= 135))
                    {
                        MakeRowBomb(currentDot.otherDot);
                    }
                    else
                    {
                        MakeColumnBomb(currentDot.otherDot);
                    }
                }
            }
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

    public void MakeColorBomb(Dot dot)
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

    public void MakeAdjacentBomb(Dot dot)
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

    public bool IsBomb(Dot dot)
    {
        if (dot is Bomb)
        {
            return true;
        }

        return false;
    }
}
