using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<Dot> currentMatches = new List<Dot>();
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        FindAllMatches();
        board.DestroyMatches();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCor());
    }

    private void AddToListAndMatch(Dot dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.isMatched = true;
    }

    private void GetNearbyDots(Dot dot1, Dot dot2, Dot dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }

    private IEnumerator FindAllMatchesCor()
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                Dot currentDot = board.allDots[i, j];

                if (currentDot != null)
                {
                    if (i > 0 && i < board.width - 1)
                    {
                        Dot leftDot = board.allDots[i - 1, j];
                        Dot rightDot = board.allDots[i + 1, j];

                        if (leftDot != null && rightDot != null)
                        {
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                GetNearbyDots(leftDot, currentDot, rightDot);
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        Dot upDot = board.allDots[i, j + 1];
                        Dot downDot = board.allDots[i, j - 1];

                        if (upDot != null && downDot != null)
                        {
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                GetNearbyDots(upDot, currentDot, downDot);
                            }
                        }
                    }
                }
            }
        }
        yield return null;
    }

    public void MatchDotsOfColor(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    if (board.allDots[i, j].tag == color || board.allDots[i, j].tag == "ColorBomb")
                    {
                        AddToListAndMatch(board.allDots[i, j]);
                    }
                }
            }
        }
    }

    public void GetAdjacentDots(int column, int row)
    {
        board.allDots[column, row].isMatched = true;
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                if (i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    if (board.allDots[i, j] != null)
                    {
                        Dot dot = board.allDots[i, j];

                        if (dot.tag == "RowBomb")
                        {
                            GetRowDots(j);

                        }
                        else if (dot.tag == "ColumnBomb")
                        {

                            GetColumnDots(i);

                        }
                        else if (dot.tag == "AdjacentBomb")
                        {
                            if (!dot.isMatched)
                            {
                                GetAdjacentDots(dot.colum, dot.row);
                            }
                        }

                        AddToListAndMatch(board.allDots[i, j]);
                    }
                }
            }
        }
    }

    public void GetColumnDots(int column)
    {

        for (int i = 0; i < board.height; i++)
        {
            if (board.allDots[column, i] != null)
            {
                Dot dot = board.allDots[column, i];

                if (dot.tag == "RowBomb")
                {

                    GetRowDots(i);

                }
                else if (dot.tag == "AdjacentBomb")
                {
                    if (!dot.isMatched)
                    {
                        GetAdjacentDots(column, i);
                    }
                }

                AddToListAndMatch(board.allDots[column, i]);
            }
        }
    }

    public void GetRowDots(int row)
    {

        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                Dot dot = board.allDots[i, row];

                if (dot.tag == "ColumnBomb")
                {

                    GetColumnDots(i);

                }
                else if (dot.tag == "AdjacentBomb")
                {
                    if (!dot.isMatched)
                    {
                        GetAdjacentDots(i, row);
                    }
                }

                AddToListAndMatch(board.allDots[i, row]);
            }
        }

    }

    public void Checkbombs(Matchtype matchtype)
    {
        if (board.currentDot != null)
        {
            if (board.currentDot.isMatched && board.currentDot.tag == matchtype.color)
            {
                board.currentDot.isMatched = false;

                if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                    || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                {
                    board.MakeRowBomb(board.currentDot);
                }
                else
                {
                    board.MakeColumnBomb(board.currentDot);
                }
            }
            else if (board.currentDot.otherDot != null)
            {
                Dot otherDot = board.currentDot.otherDot;
                if (otherDot.isMatched && otherDot.tag == matchtype.color)
                {
                    otherDot.isMatched = false;

                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                        || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                    {
                        board.MakeRowBomb(board.currentDot.otherDot);
                    }
                    else
                    {
                        board.MakeColumnBomb(board.currentDot.otherDot);
                    }
                }
            }
        }
    }
}
