using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypePiece
{
    EMPTY,
    PAWN,
    SUPPORTER,
    COMMANDER
}

public enum MoveType
{
    MOVE,
    TAKE,
    PLACE,
    FAIL
}

[System.Serializable]
public abstract class Piece
{
    private TypeTeam team;
    public abstract TypePiece Type();
    public abstract MoveType CheckMoveType(int[] pos1, int[] pos2);
    public abstract float GetEffectiveRange();
    public abstract float GetDistanceValue();
    public abstract float GetPieceValue();
    public abstract List<int[]> AttackingTiles(int[] pos);

    public Piece(TypeTeam _team)
    {
        team = _team;
    }

    public  TypeTeam Team()
    {
        return team;
    }
}
