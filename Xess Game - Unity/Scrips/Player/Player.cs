using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypePlayer
{
    HUMAN,
    ADHOC,
    ADVANCED,
    RANDOM
}

public abstract class Player
{
    private TypeTeam team;
    private TypePlayer type;
    public TypeTeam Team { get { return team; } }

    public Player(TypeTeam _team)
    {
        team = _team;
    }

    public abstract TypePlayer Type();

    public abstract int[][] TakeTurn(AI_Difficulty difficulty = AI_Difficulty.nul);

    internal float Distance(int[] pos1, int[] pos2)
    {
        return Mathf.Sqrt((Mathf.Pow(pos1[0] - pos2[0], 2) + Mathf.Pow(pos1[1] - pos2[1], 2)));
    }

    internal float GetTotalPoints(float[,] boardPoints, TypeTeam t = TypeTeam.nul, Piece[,] board = null)
    {
        float points = 0f;
        if (t == TypeTeam.nul)
            foreach (float p in boardPoints)
            {
                points += p;
            }
        else
        {
            for (int i = 0; i < Board.I.Length; i++)
            {
                for (int ii = 0; ii < Board.I.Width; ii++)
                {
                    if (board[i, ii].Team() == t)
                        points += boardPoints[i, ii];
                }
            }
        }
        return points;
    }

    internal TypeTeam GetEnemyTeam(TypeTeam t)
    {
        if (t == TypeTeam.BLUE)
            return TypeTeam.RED;
        else
            return TypeTeam.BLUE;
    }

    protected float setDifficulty(AI_Difficulty d)
    {
        switch (d)
        {
            case AI_Difficulty.nul:
                return 0f;
            case AI_Difficulty.EASY:
                return 0.5f;
            case AI_Difficulty.MEDIUM:
                return 0.3f;
            case AI_Difficulty.HARD:
                return 0.1f;
            default:
                Debug.LogError("could not assign difficulty");
                throw new System.NotImplementedException();
        }
    }
}
