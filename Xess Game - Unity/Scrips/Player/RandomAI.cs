using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAI : Player // should be complete
{
    public RandomAI(TypeTeam _team) : base(_team)
    {
    }

    public override int[][] TakeTurn(AI_Difficulty difficulty = AI_Difficulty.nul)
    {
        board = Board.I.GetBoard();

        int[][] move = RandomMove();

        return move;
    }

    public override TypePlayer Type()
    {
        return TypePlayer.RANDOM;
    }

    // Random AI specific stuffffffffffffffffffffff
    Piece[,] board;

    private int[][] RandomMove()
    {
        int[] piece = RandomPiece();
        int[] move = new int[2];
        board = Board.I.GetBoard();

        //Debug.Log("selecting move");
        int counter = 0, counterLimit = 50; // used to prevent an endless loop from occuring
        //int counterOverSeeer = 0;


        while (true)
        {
            if (board[piece[0], piece[1]].Type() == TypePiece.SUPPORTER || board[piece[0], piece[1]].Type() == TypePiece.COMMANDER) // supporter or commander
            {
                //Debug.Log("Other Piece moves: " + board[piece[0], piece[1]].Type().ToString());
                try
                {
                    move = board[piece[0], piece[1]].AttackingTiles(piece)[Random.Range(0, board[piece[0], piece[1]].AttackingTiles(piece).Count)];
                }
                catch (System.Exception )
                {
                    //Debug.LogWarning(e);
                    //Debug.LogWarning("Attempting to goto sweeper code");
                    goto SweeperCode;
                }
            }
            else if (board[piece[0], piece[1]].Type() == TypePiece.PAWN) // pawn
            {
                try
                {
                    //Debug.Log("Pawn moves");
                    List<int[]> possibleMoves = board[piece[0], piece[1]].AttackingTiles(piece);
                    if (possibleMoves[0][1] + 1 < Board.I.Width)
                        possibleMoves.Add(new int[] { possibleMoves[0][0], possibleMoves[0][1] + 1 });

                    if (possibleMoves[0][1] - 1 >= 0)
                        possibleMoves.Add(new int[] { possibleMoves[0][0], possibleMoves[0][1] - 1 });

                    move = possibleMoves[Random.Range(0, possibleMoves.Count)];
                }
                catch (System.Exception )
                {
                    //Debug.LogWarning(e);
                    //Debug.LogWarning("Attempting to goto sweeper code");
                    goto SweeperCode;
                }

            }
            else // should not reach here || added for error checking
            {
                //Debug.LogError("Piece does not have valid type");
                throw new System.NotImplementedException();
            }

            //Debug.Log("Move: " + move[0] + ", " + move[1]);
            if (board[move[0], move[1]].Team() != Team)
            {
                if (board[move[0], move[1]].Team() == GetEnemyTeam(Team))
                {
                    // can only make this move if the move type is Place or take
                    if (board[piece[0], piece[1]].CheckMoveType(piece, move) == MoveType.PLACE ||
                        board[piece[0], piece[1]].CheckMoveType(piece, move) == MoveType.TAKE)
                    {
                        //Debug.Log("Piece:" + piece[0] + ", " + piece[1] +
                        //    "\nMove: " + move[0] + ", " + move[1]);
                        return new int[][] { piece, move };
                    }
                }
                else
                {
                    // can only make this move if the move type is move or place
                    if (board[piece[0], piece[1]].CheckMoveType(piece, move) == MoveType.MOVE ||
                        board[piece[0], piece[1]].CheckMoveType(piece, move) == MoveType.PLACE)
                    {
                        //Debug.Log("Piece:" + piece[0] + ", " + piece[1] +
                        //    "\nMove: " + move[0] + ", " + move[1]);
                        return new int[][] { piece, move };
                    }
                }
            }

        SweeperCode:

            if (Game.I.GetGameState() != GameState.InProgress)
            {
                break;
            }

            counter++;

            if (counter >= counterLimit)
            {
                piece = RandomPiece();
                counter = 0;
                /*if (counterOverSeeer >= 50)
                    counterOverSeeer++;
                else
                    break;*/
            }
        }

        //Debug.LogWarning("WHYYYYYYYYYYYYYYYYYYYY");
        //Debug.LogWarning("If you've reached this point.....how");
        //throw new System.NotImplementedException();
        return new int[][] { new int[] { } };
    }

    private int[] RandomPiece()
    {
        //Debug.Log("Selecting Piece");
        int[] piece = new int[] { Random.Range(0, Board.I.Length), Random.Range(0, Board.I.Width) };

        while (true)
        {
            if (board[piece[0], piece[1]].Team() != Team)
                piece = new int[] { Random.Range(0, Board.I.Length), Random.Range(0, Board.I.Width) };
            else
                break;
        }

        return piece;
    }
}
