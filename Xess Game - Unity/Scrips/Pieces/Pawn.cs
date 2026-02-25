using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pawn : Piece
{
    public Pawn(TypeTeam _team) : base(_team)
    {
    }

    public override List<int[]> AttackingTiles(int[] pos)
    {
        List<int[]> tiles = new List<int[]>();
        if (Team() == TypeTeam.RED)
        {
            if (pos[0] + 1 < Board.I.Length)
                if (Board.I.GetBoard()[pos[0] + 1, pos[1]].Team() != Team())
                    tiles.Add(new int[] { pos[0] + 1, pos[1] });
        }
        else if (Team() == TypeTeam.BLUE)
        {
            if (pos[0] - 1 >= 0)
                if (Board.I.GetBoard()[pos[0] - 1, pos[1]].Team() != Team())
                    tiles.Add(new int[] { pos[0] - 1, pos[1] });
        }

        return tiles;
    }

    public override MoveType CheckMoveType(int[] pos1, int[] pos2)
    {
        if (Team() == TypeTeam.RED) // Red piece => moves forwards
        {
            if (pos1[0] < pos2[0])
            {
                if (pos1[0] + 1 == pos2[0] && pos1[1] == pos2[1])
                {
                    return MoveType.TAKE; // move type for this position is take
                }
                else if (pos1[1] + 1 == pos2[1] || pos1[1] - 1 == pos2[1])
                {
                    if (pos1[0] + 1 == pos2[0])
                        return MoveType.MOVE; // move type for this position is move
                }
            }
        }
        else // Blue piece => moves backwards
        {
            if (pos1[0] > pos2[0])
            {
                if (pos1[0] - 1 == pos2[0] && pos1[1] == pos2[1])
                {
                    return MoveType.TAKE;  // move type for this position is take
                }
                else if (pos1[1] + 1 == pos2[1] || pos1[1] - 1 == pos2[1])
                {
                    if (pos1[0] + 1 == pos2[0] || pos1[0] - 1 == pos2[0])
                        return MoveType.MOVE; // move type for this position is 
                }
            }
        }

        return MoveType.FAIL;
    }

    public override float GetDistanceValue()
    {
        return 0.9f;
    }

    public override float GetEffectiveRange()
    {
        return 1f;
    }

    public override float GetPieceValue()
    {
        return 1f;
    }

    public override TypePiece Type()
    {
        return TypePiece.PAWN;
    }
}
