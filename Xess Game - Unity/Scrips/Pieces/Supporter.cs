using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Supporter : Piece
{
    public Supporter(TypeTeam _team) : base(_team)
    {
    }

    public override List<int[]> AttackingTiles(int[] pos)
    {
        List<int[]> tiles = new List<int[]>();
        if (pos[0] - 1 >= 0 && pos[1] + 1 < Board.I.Width) // checked
        {
            if (Board.I.GetBoard()[pos[0] - 1, pos[1] + 1].Team() != Team())
                tiles.Add(new int[] { pos[0] - 1, pos[1] + 1 });
        }
        if (pos[0] - 2 >= 0 && pos[1] + 2 < Board.I.Width) // checked
        {
            if (Board.I.GetBoard()[pos[0] - 2, pos[1] + 2].Team() != Team())
                tiles.Add(new int[] { pos[0] - 2, pos[1] + 2 }); // was a problem
        }
        if (pos[0] - 1 >= 0 && pos[1] - 1 >= 0) // checked
        {
            if (Board.I.GetBoard()[pos[0] - 1, pos[1] - 1].Team() != Team())
                tiles.Add(new int[] { pos[0] - 1, pos[1] - 1 });
        }
        if (pos[0] - 2 >= 0 && pos[1] - 2 >= 0) // checked
        {
            if (Board.I.GetBoard()[pos[0] - 2, pos[1] - 2].Team() != Team())
                tiles.Add(new int[] { pos[0] - 2, pos[1] - 2 });
        }


        if (pos[0] + 1 < Board.I.Length && pos[1] + 1 < Board.I.Width) // checked
        {
            if (Board.I.GetBoard()[pos[0] + 1, pos[1] + 1].Team() != Team())
                tiles.Add(new int[] { pos[0] + 1, pos[1] + 1 });
        }
        if (pos[0] + 2 < Board.I.Length && pos[1] + 2 < Board.I.Width) // checked
        {
            if (Board.I.GetBoard()[pos[0] + 2, pos[1] + 2].Team() != Team())
                tiles.Add(new int[] { pos[0] + 2, pos[1] + 2 });
        }
        if (pos[0] + 1 < Board.I.Length && pos[1] - 1 >= 0)// checked
        {
            if (Board.I.GetBoard()[pos[0 + 1], pos[1] - 1].Team() != Team())
                tiles.Add(new int[] { pos[0] + 1, pos[1] - 1 }); // was a problem
        }
        if (pos[0] + 2 < Board.I.Length && pos[1] - 2 >= 0)
        {
            if (Board.I.GetBoard()[pos[0] + 2, pos[1] - 2].Team() != Team())
                tiles.Add(new int[] { pos[0] + 2, pos[1] - 2 });
        }

        return tiles;
    }

    public override MoveType CheckMoveType(int[] pos1, int[] pos2)
    {
        if (pos1[0] + 2 == pos2[0] || pos1[0] - 2 == pos2[0])
        {
            if (pos1[1] + 2 == pos2[1] || pos1[1] - 2 == pos2[1])
            {
                return MoveType.PLACE;
            }
        }
        else if (pos1[0] + 1 == pos2[0] || pos1[0] - 1 == pos2[0])
        {
            if (pos1[1] + 1 == pos2[1] || pos1[1] - 1 == pos2[1])
            {
                return MoveType.TAKE;
            }
        }
        /*else if (pos1[1] + 1 == pos2[1] || pos1[1] - 1 == pos2[1])
        {
            if (pos1[0] == pos2[0])
            {
                return MoveType.TAKE;
            }
        }*/

        return MoveType.FAIL;
    }

    public override float GetDistanceValue()
    {
        return 0.9f;
    }

    public override float GetEffectiveRange()
    {
        return 1.8f;
    }

    public override float GetPieceValue()
    {
        return 2.2f;
    }

    public override TypePiece Type()
    {
        return TypePiece.SUPPORTER;
    }
}
