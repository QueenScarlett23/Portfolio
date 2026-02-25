using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Empty : Piece
{
    public Empty(TypeTeam _team) : base(_team)
    {
    }

    public override List<int[]> AttackingTiles(int[] pos)
    {
        List<int[]> tiles = new List<int[]>();
        return tiles;
    }

    public override MoveType CheckMoveType(int[] pos1, int[] pos2)
    {
        return MoveType.FAIL;
    }

    public override float GetDistanceValue()
    {
        throw new System.NotImplementedException();
    }

    public override float GetEffectiveRange()
    {
        throw new System.NotImplementedException();
    }

    public override float GetPieceValue()
    {
        throw new System.NotImplementedException();
    }

    public override TypePiece Type()
    {
        return TypePiece.EMPTY;
    }
}
