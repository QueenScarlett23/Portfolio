using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Player
{
    public Human(TypeTeam _team) : base(_team)
    {
    }

    public override int[][] TakeTurn(AI_Difficulty difficulty)
    {
        Debug.LogError("Human Turn not meant to be called");
        throw new System.NotImplementedException();
    }

    public override TypePlayer Type()
    {
        return TypePlayer.HUMAN;
    }
}
