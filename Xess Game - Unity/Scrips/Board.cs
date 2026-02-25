using System.Collections.Generic;
using UnityEngine;

public enum TypeTeam
{
    RED,
    BLUE,
    NONE,
    nul
}

[System.Serializable]
public class Board : MonoBehaviour
{
    private static Board i;
    public static Board I { get { return i; } }

    void Awake()
    {
        if (i == null)
            i = this;
        else
            DestroyImmediate(this);
    }

    Piece[,] board;

    int _length;
    int _width;

    public int Length { get { return _length; } }
    public int Width { get { return _width; } }

    public void NewBoard(bool rand, int length = 10, int width = 7)
    {
        // decide where your obsticles go
        // decide where players go
        if (!rand)
        {
            // sets board to default board
            board = DefaultBoard();
        }
        else
        {
            // random board
            //Debug.LogWarning("Random Board");
            board = new Piece[length, width];
            _length = length;
            _width = width;

            for (int i = 0; i < Length; i++)
            {
                for (int ii = 0; ii < width; ii++)
                {
                    if (ii == width - 1 || ii == 0)
                    {
                        board[i, ii] = new Empty(TypeTeam.nul);
                    }
                    else
                    {
                        if (i == 0)
                        {
                            // Blue Pieces + commander
                            if (ii == (int)((float)(width / 2) + 0.1f))
                                board[i, ii] = new Commander(TypeTeam.RED);
                            else
                                board[i, ii] = RandomPiece(Random.Range(0, 10), TypeTeam.RED);
                        }
                        else if (i == 1)
                        {
                            // Red Pieces 
                            board[i, ii] = RandomPiece(Random.Range(0, 10), TypeTeam.RED);
                        }
                        else if (i == Length - 2)
                        {
                            // Blue Pieces
                            board[i, ii] = RandomPiece(Random.Range(0, 10), TypeTeam.BLUE);
                        }
                        else if (i == Length - 1)
                        {
                            // Blue Pieces + commander
                            if (ii == (int)((float)(width / 2) + 0.1f))
                                board[i, ii] = new Commander(TypeTeam.BLUE);
                            else
                                board[i, ii] = RandomPiece(Random.Range(0, 10), TypeTeam.BLUE);
                        }
                        else
                        {
                            board[i, ii] = new Empty(TypeTeam.nul);
                        }
                    }
                    //Debug.Log(i + ", " + ii + " - " + board[i, ii].Type());
                }
            }
        }
    }

    private Piece RandomPiece(int v, TypeTeam team)
    {
        if (v < 5)
        {
            return new Supporter(team);
        }
        else
        {
            return new Pawn(team);
        }
    }

    private Piece[,] DefaultBoard()
    {
        _length = 10;
        _width = 7;

        Piece[,] defaultBoard = new Piece[Length, Width];

        for (int i = 0; i < Length; i++)
        {
            for (int ii = 0; ii < Width; ii++)
            {
                defaultBoard[i, ii] = new Empty(TypeTeam.NONE);
            }
        }

        for (int i = 0; i < 10; i++)
        {
            if (i > 4)
            {
                defaultBoard[1, i - 4] = new Pawn(TypeTeam.RED);
                defaultBoard[Length - 2, i - 4] = new Pawn(TypeTeam.BLUE);
            }
            else
            {
                defaultBoard[0, i + 1] = new Supporter(TypeTeam.RED);
                defaultBoard[Length - 1, i + 1] = new Supporter(TypeTeam.BLUE);
            }
        }

        defaultBoard[0, 3] = new Commander(TypeTeam.RED);
        defaultBoard[Length - 1, 3] = new Commander(TypeTeam.BLUE);

        return defaultBoard;
    }

    // checks for winner and checks if game timer needs to be started
    internal TypeTeam CheckForWinner()
    {

        bool Red = false, Blue = false;
        int RedPieces = 0, BluePieces = 0;

        // counting pieces
        for (int i = 0; i < _length; i++)
        {
            for (int ii = 0; ii < _width; ii++)
            {
                if (board[i, ii].Type() == TypePiece.COMMANDER)
                {
                    if (board[i, ii].Team() == TypeTeam.RED)
                    {
                        Red = true;
                    }
                    else if (board[i, ii].Team() == TypeTeam.BLUE)
                    {
                        Blue = true;
                    }
                }
                else if (board[i, ii].Type() != TypePiece.EMPTY)
                {
                    if (board[i, ii].Team() == TypeTeam.BLUE)
                    {
                        if (board[i, ii].AttackingTiles(new int[] { i, ii }).Count != 0)
                            BluePieces++;
                    }
                    else if (board[i, ii].Team() == TypeTeam.RED)
                    {
                        if (board[i, ii].AttackingTiles(new int[] { i, ii }).Count != 0)
                            RedPieces++;
                    }
                }
            }
        }

        /* 
         * if statement made this way to make way for possible expansion on game rules
         */

        if (Blue) // blue alive
        {
            if (!Red) // red dead
            {
                return TypeTeam.BLUE;
            }
            else // both alive
            {
                if (BluePieces == 0) // no blue pieces left
                {
                    if (RedPieces == 0) // no pieces left
                    {
                        return TypeTeam.NONE;
                    }
                    else // no blue pieces left + red pieces left
                    {
                        Game.I.StartTimer(TypeTeam.BLUE);
                        return TypeTeam.nul;
                    }
                }
                else if (RedPieces == 0) // no red pieces left + blue pieces remaining
                {
                    Game.I.StartTimer(TypeTeam.RED);
                    return TypeTeam.nul;
                }
                else // no timer started
                {
                    return TypeTeam.nul;
                }
            }
        }
        else if (Red) // red alive + blue dead
        {
            return TypeTeam.RED;
        }

        //should never reach this point unless there is an error
        Debug.LogError("Something is wrong");
        return TypeTeam.nul;
    }

    internal MoveType AttemptMove(int[] pos1, int[] pos2)
    {
        MoveType move = board[pos1[0], pos1[1]].CheckMoveType(pos1, pos2);
        if (move == MoveType.FAIL)
        {
            return move;
        }
        else
        {
            bool moveResult = false;
            switch (move)
            {
                case MoveType.MOVE:
                    moveResult = ValidateMove(pos1, pos2);
                    break;

                case MoveType.TAKE:
                    moveResult = ValidateTake(pos1, pos2);
                    break;

                case MoveType.PLACE:
                    move = DeturminPlace(pos1, pos2);
                    if (MoveType.FAIL != move)
                        moveResult = true;
                    break;
            }

            if (moveResult)
            {
                Move(pos1, pos2);
                return move;
            }
        }
        return MoveType.FAIL;
    }

    private void Move(int[] pos1, int[] pos2)
    {
        UI_Handler.I.UpdateBoard(pos1, pos2, board[pos1[0], pos1[1]]);

        board[pos2[0], pos2[1]] = board[pos1[0], pos1[1]];
        board[pos1[0], pos1[1]] = new Empty(TypeTeam.NONE);
    }

    internal bool ValidateMove(int[] pos1, int[] pos2)
    {
        if (board[pos2[0], pos2[1]].Type() == TypePiece.EMPTY)
        {
            return true;
        }
        return false;
    }

    internal bool ValidateTake(int[] pos1, int[] pos2)
    {
        if (board[pos2[0], pos2[1]].Type() != TypePiece.EMPTY &&
            board[pos1[0], pos1[1]].Team() != board[pos2[0], pos2[1]].Team())
        {
            return true;
        }
        return false;
    }

    internal MoveType DeturminPlace(int[] pos1, int[] pos2)
    {
        if (board[pos2[0], pos2[1]].Type() == TypePiece.EMPTY)
        {
            if (ValidateMove(pos1, pos2))
            {
                return MoveType.MOVE;
            }
        }
        else
        {
            if (ValidateTake(pos1, pos2))
            {
                return MoveType.TAKE;
            }
        }
        return MoveType.FAIL;
    }

    internal Piece[,] GetBoard()
    {
        return board;
    }

    internal TypeTeam GetTeamOfPiece(int[] pos)
    {
        return board[pos[0], pos[1]].Team();
    }
    //mytiddies
}
