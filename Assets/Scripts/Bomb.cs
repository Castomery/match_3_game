using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Dot
{
    float clicked = 0;
    float clicktime = 0;
    float clickdelay = 0.5f;

    bool DoubleClick()
    {
        if (clicked == 1) clicktime = Time.time;
        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            clicked = 0;
            clicktime = 0;
            return true;
        }
        else if (clicked > 2 || Time.time - clicktime > 1) clicked = 0;
        return false;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clicked++;
            if (DoubleClick())
            {
                otherDot = null;
                Bomb bomb = (Bomb)this;
                ActivateBomb();
                board.DestroyMatches();
                OnDecreaceAction();
            }
        }
    }

    //private void OnMouseOver()
    //{
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        otherDot = null;
    //        Bomb bomb = (Bomb)this;
    //        ActivateBomb();
    //        board.DestroyMatches();
    //        OnDecreaceAction();
    //    }
    //}

    public void ActivateBomb()
    {
        if (this.tag == "RowBomb")
        {
            findMatches.GetRowDots(row);
        }
        else if (otherDot != null && otherDot.tag == "RowBomb")
        {
            findMatches.GetRowDots(otherDot.row);
        }

        if (this.tag == "ColumnBomb")
        {
            findMatches.GetColumnDots(colum);
        }
        else if (otherDot != null && otherDot.tag == "ColumnBomb")
        {
            findMatches.GetColumnDots(otherDot.colum);
        }

        if (this.tag == "AdjacentBomb")
        {
            findMatches.GetAdjacentDots(colum, row);
        }
        else if (otherDot != null && otherDot.tag == "AdjacentBomb")
        {
            findMatches.GetAdjacentDots(otherDot.colum, otherDot.row);
        }
    }
}
