using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Commander : Piece
{
    public Commander(TypeTeam _team) : base(_team)
    {
    }

    public override List<int[]> AttackingTiles(int[] pos)
    {
        List<int[]> tiles = new List<int[]>();
        for (int i = -1; i < 2; i++)
        {
            for (int ii = -1; ii < 2; ii++)
            {
                if (pos[0] + i >= 0 && pos[0] + i < Board.I.Length && pos[1] + ii >= 0 && pos[1] + ii < Board.I.Width)
                {
                    if (i != 0 || ii != 0)
                    {
                        if (Board.I.GetBoard()[pos[0] + i, pos[1] + ii].Team() != Team())
                            tiles.Add(new int[] { pos[0] + i, pos[1] + ii });
                    }
                }
            }
        }
        return tiles;
    }

    public override MoveType CheckMoveType(int[] pos1, int[] pos2)
    {
        if (pos1[0] + 1 == pos2[0] || pos1[0] - 1 == pos2[0])
        {
            if (pos1[1] + 1 == pos2[1] || pos1[1] - 1 == pos2[1] || pos1[1] == pos2[1])
            {
                return MoveType.PLACE;
            }
        }
        else if (pos1[1] + 1 == pos2[1] || pos1[1] - 1 == pos2[1])
        {
            if (pos1[0] == pos2[0])
            {
                return MoveType.PLACE;
            }
        }

        return MoveType.FAIL;
    }

    public override float GetDistanceValue()
    {
        return 0.2f;
    }

    public override float GetEffectiveRange()
    {
        return 1.3f;
    }

    public override float GetPieceValue()
    {
        return 500;
    }

    public override TypePiece Type()
    {
        return TypePiece.COMMANDER;
    }
    // Tell them to hand over the drugg monez
    // I need it to buy into a pyramid scheme
    // Please they are holding my wife and children hositige
    // And i an no literate
    // I don;t know how to read and write
    // kjhgkajhhjad adjahbvajkhbetuiy oia       of oaiu d
    // kjhab aerkhofiubhuhhLWETJLKJD LOKU AOl luisjd fdoifa OIUOOIUJNKNIUYH!!!

}
