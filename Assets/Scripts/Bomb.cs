using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Dot
{
    private float clicked = 0;
    private float clicktime = 0;
    private float clickdelay = 0.5f;

    private bool DoubleClick()
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
                board.currentDot = this;
                otherDot = null;
                ActivateBomb();
                board.DestroyMatches();
                OnDecreaceAction();
            }
        }
    }

    public void ActivateBomb()
    {
        if (this.tag == "ColorBomb")
        {
            findMatches.GetRandomNearColorDot(this);
        }

        if (this.tag == "RowBomb")
        {
            findMatches.GetRowDots(row);
        }

        if (this.tag == "ColumnBomb")
        {
            findMatches.GetColumnDots(colum);
        }

        if (this.tag == "AdjacentBomb")
        {
            findMatches.GetAdjacentDots(colum, row);
        }
    }

   
}
