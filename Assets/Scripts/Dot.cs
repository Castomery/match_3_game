using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int colum;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    [Header("Swipe")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    public event Action OnDeacreasMovesAction;
    private EndGameManager endGameManager;
    private HintManager hintManager;
    protected FindMatches findMatches;
    protected Board board;
    public Dot otherDot;
    private Vector2 firstTouchPosition = Vector2.zero;
    private Vector2 lastTouchPosition = Vector2.zero;
    private Vector2 tempPosition;
    private ObjectPool _objectPool;

    // Start is called before the first frame update
    public void Start()
    {
        //isColumnBomb = false;
        //isRowBomb = false;
        //isColorBomb = false;
        //isAdjacentBomb = false;
        endGameManager = FindObjectOfType<EndGameManager>();
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        hintManager = FindObjectOfType<HintManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        targetX = colum;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > 0.1)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.6f);
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allDots[colum, row] = this;
        }

        if (Mathf.Abs(targetY - transform.position.y) > 0.1)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.6f);
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allDots[colum, row] = this;
        }
    }

    public void InitObjectPool(ObjectPool objectPool)
    {
        _objectPool = objectPool;
    }

    public void ReturnToPool()
    {
        _objectPool.Return(this);
    }

    protected IEnumerator CheckMoveCor()
    {

        if (this is Bomb)
        {
            Bomb bomb = (Bomb)this;

            if (bomb.tag == "ColorBomb")
            {
                if (otherDot is Bomb)
                {
                    yield return null;
                }
                else
                {
                    findMatches.MatchDotsOfColor(this, otherDot.tag);
                }
                
            }

            bomb.ActivateBomb();   
        }
        else if (otherDot is Bomb)
        {
            Bomb bomb = (Bomb)otherDot;

            if (otherDot.tag == "ColorBomb")
            {
                if (this is Bomb)
                {
                    yield return null;
                }
                else
                {
                    findMatches.MatchDotsOfColor(otherDot, this.tag);
                }
            }
            bomb.ActivateBomb();
        }

        yield return new WaitForSeconds(0.2f);

        if (otherDot != null)
        {
            if (!isMatched && !otherDot.isMatched)
            {
                otherDot.row = row;
                otherDot.colum = colum;
                row = previousRow;
                colum = previousColumn;
                SwitchDots();
                yield return new WaitForSeconds(0.5f);
                board.currentDot = null;
                board._isChangesEnds = true;
            }
            else
            {
                if (endGameManager != null)
                {
                    if (endGameManager.requirments.gameType == GameType.Moves)
                    {
                        OnDecreaceAction();
                    }
                }
                board.DestroyMatches();
            }
            otherDot = null;
        }

    }

    protected void OnMouseDown()
    {
        if (hintManager.currentHint != null)
        {
            hintManager.DestroyHint();
        }

        if (board.currentState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    protected void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            lastTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }

    }

    protected void CalculateAngle()
    {
        if (Mathf.Abs(lastTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(lastTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            board._isChangesEnds = false;
            board.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(lastTouchPosition.y - firstTouchPosition.y, lastTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentDot = this;
        }
        else
        {
            board._isChangesEnds = true;
        }
    }

    protected void MoveDot(Vector2 direction)
    {
        otherDot = board.allDots[colum + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = colum;
        if (otherDot != null)
        {
            otherDot.colum += -1 * (int)direction.x;
            otherDot.row += -1 * (int)direction.y;
            colum += (int)direction.x;
            row += (int)direction.y;
            SwitchDots();
            StartCoroutine(CheckMoveCor());
        }
        else
        {
            board._isChangesEnds = true;
        }
    }

    protected void OnDecreaceAction()
    {
        OnDeacreasMovesAction?.Invoke();
    }

    protected void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && colum < board.width - 1)
        {
            //Right Swipe
            MoveDot(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            //Up Swipe
            MoveDot(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && colum > 0)
        {
            //Left Swipe
            MoveDot(Vector2.left);
        }
        else if (swipeAngle <= -45 && swipeAngle > -135 && row > 0)
        {
            //Down Swipe
            MoveDot(Vector2.down);
        }
        else
        {
            board._isChangesEnds = true;
        }
    }

    private void SwitchDots()
    {
        board.allDots[otherDot.colum, otherDot.row] = otherDot;
        board.allDots[colum, row] = this;
        findMatches.FindAllMatches();
    }
}
