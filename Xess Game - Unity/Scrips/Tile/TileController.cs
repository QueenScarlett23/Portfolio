using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    private int[] pos = new int[2];
    public void SetCoordinate(int x, int y)
    {
        pos[0] = x;
        pos[1] = y;
    }

    public void TileCLicked()
    {
        //Debug.Log("Tile Clicked: " + ToString());
        UI_Handler.I.TileClicked(GetCoordinates());
    }

    public int[] GetCoordinates()
    {
        return pos;
    }

    public override string ToString()
    {
        return "[ " + pos[0] + ", " + pos[1] + " ]";
    }
}
