using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ad_hoc : Player
{
    public Ad_hoc(TypeTeam _team) : base(_team)
    {
    }

    public override TypePlayer Type()
    {
        return TypePlayer.ADHOC;
    }

    public override int[][] TakeTurn(AI_Difficulty difficulty)
    {
        board = Board.I.GetBoard(); // all parts of making a move involve refrenceing the board values \\ board values are coppied here as they are refrenced very often
        int[][] move = new int[][] { new int[] { -1, -1 }, new int[] { -1, -1 } };
        float[,] points = UtilityFunction();
        ThreashHold = GetThreashHold(Team);

        //Debug.Log("Turn being taken");
        if (Random.Range(0f, 1f) <= setDifficulty(difficulty)) // checks if random move should be made 
        {
            //Debug.Log(Team.ToString() + " made a random move");
            return RandomMove(); // Returns to reduce wasted resources
        }
        else
        {
            int[] chosenPiece = new int[] { -1 };
            float tempScore = -1;
            List<int[]> bestPieces = new List<int[]>();

            for (int i = 0; i < Board.I.Length; i++)
            {
                for (int ii = 0; ii < Board.I.Width; ii++)
                {
                    if (board[i, ii].Team() == Team)
                    {
                        if (board[i, ii].AttackingTiles(new int[] { i, ii }).Count > 0)
                        {
                            if (points[i, ii] > tempScore)
                            {
                                tempScore = points[i, ii];
                                chosenPiece = new int[] { i, ii };
                            }
                            if (points[i, ii] > ThreashHold)
                            {
                                bestPieces.Add(new int[] { i, ii });
                            }
                        }
                    }
                    else if (board[i, ii].Team() == GetEnemyTeam(Team))
                    {
                        if (points[i, ii] > 500)
                        {
                            move = MoveCommander();
                        }
                    }
                }
            }

            /*Debug.LogWarning("Piece: " + move[0][0] + ", " + move[0][1] +
                    "\nMove :" + move[1][0] + ", " + move[1][1] +
                    "\nChosen Piece: " + chosenPiece[0] + ", " + chosenPiece[1]);*/

            if (points[chosenPiece[0], chosenPiece[1]] >= 500)
            {
                move = new int[][] { chosenPiece, GetPieceMove(chosenPiece, AttackMovePriority.High) };
                //Debug.LogWarning("Taking the COMMANDER !!!!!!!!!!!!!!");
                return move;
            }
            else if (move[0][0] != -1)
            {
                if (points[move[0][0], move[0][1]] > 500)
                {
                    //Debug.LogWarning("RUUUUUUUUUUUUUUUUUUUUN!!!!!!!!!!!!!!!!!!!!!!!!");
                    return move;
                }
            }
            if (bestPieces.Count > 1)
            {
                //Debug.Log("Best pieces number: " + bestPieces.Count);
                chosenPiece = bestPieces[Random.Range(0, bestPieces.Count - 1)];
            }


            if (chosenPiece[0] != -1)
                switch (difficulty)
                {
                    case AI_Difficulty.EASY:
                        move = new int[][] { new int[] { chosenPiece[0], chosenPiece[1] }, GetPieceMove(chosenPiece, AttackMovePriority.Low) };
                        break;
                    case AI_Difficulty.MEDIUM:
                        move = new int[][] { new int[] { chosenPiece[0], chosenPiece[1] }, GetPieceMove(chosenPiece, AttackMovePriority.Rand) };
                        break;
                    case AI_Difficulty.HARD:
                        move = new int[][] { new int[] { chosenPiece[0], chosenPiece[1] }, GetPieceMove(chosenPiece, AttackMovePriority.High) };
                        break;
                    default:
                        Debug.LogError("No Difficulty given");
                        move = new int[][] { chosenPiece, GetPieceMove(chosenPiece) };
                        break;
                }
            else
            {
                Debug.LogError("Something wrong hereeee"); // Don't ask me what. This is a precautionary thing
            }
        }
        return move;
        //Debug.LogWarning("FinalMove!\nPiece: " + move[0][0] + ", " + move[0][1] + "\nMove :" + move[1][0] + ", " + move[1][1]);
        // add random move function ==========================================================[X] no bugs
        // if there is a piece that can take the enemy commander then take that  move ========[X] 
        // add percentage chance tha AI will fail ============================================[X] 
        // use utility function ==============================================================[X] 
    }
    // Ad_hoc specific code

    public float ThreashHold { get; private set; }
    //private int Turn = 0;
    Piece[,] board;
    float[,] points;
    public float[,] UtilityFunction() // Is finished
    {
        points = new float[Board.I.Length, Board.I.Width];
        board = Board.I.GetBoard();

        int[] BluePosition = null;
        int[] RedPosition = null;


        // getting enemy information
        for (int i = 0; i < Board.I.Length; i++)
        {
            for (int ii = 0; ii < Board.I.Width; ii++)
            {
                if (board[i, ii].Type() == TypePiece.COMMANDER)
                {
                    if (board[i, ii].Team() == TypeTeam.BLUE)
                        BluePosition = new int[] { i, ii };
                    else
                        RedPosition = new int[] { i, ii };
                }
            }
            if (BluePosition != null && RedPosition != null)
            {
                goto calculatePoints;
            }
        }
        //Debug.LogError("Enemy Not found");
        //throw new System.NotImplementedException();
        goto end;

    calculatePoints:
        // calculates points
        //Debug.Log("Calculating points");
        for (int i = 0; i < Board.I.Length; i++)
        {
            for (int ii = 0; ii < Board.I.Width; ii++)
            {
                if (board[i, ii].Team() == TypeTeam.RED) // debate whether or not to include the commander
                {
                    //points[i, ii] = board[i, ii].GetPieceValue() * (Mathf.Pow(board[i, ii].GetDistanceValue(), Mathf.Abs(board[i, ii].GetEffectiveRange() - Distance(BluePosition, new int[] { i, ii }))));
                    if (board[i, ii].Type() == TypePiece.PAWN)
                    {
                        if (board[i, ii].AttackingTiles(new int[] { i, ii }).Count > 0)
                        {
                            points[i, ii] = (Mathf.Pow(board[i, ii].GetDistanceValue(), Mathf.Abs(board[i, ii].GetEffectiveRange() - Distance(BluePosition, new int[] { i, ii }))));
                            foreach (int[] pos in board[i, ii].AttackingTiles(new int[] { i, ii }))
                            {
                                if (board[pos[0], pos[1]].Team() == TypeTeam.BLUE)
                                    points[i, ii] += board[pos[0], pos[1]].GetPieceValue();
                            }
                        }
                    }
                    else
                    {
                        points[i, ii] = (Mathf.Pow(board[i, ii].GetDistanceValue(), Mathf.Abs(board[i, ii].GetEffectiveRange() - Distance(BluePosition, new int[] { i, ii }))));
                        foreach (int[] pos in board[i, ii].AttackingTiles(new int[] { i, ii }))
                        {
                            if (board[pos[0], pos[1]].Team() == TypeTeam.BLUE)
                                points[i, ii] += board[pos[0], pos[1]].GetPieceValue();
                        }
                    }

                }
                else if (board[i, ii].Team() == TypeTeam.BLUE)
                {
                    if (board[i, ii].Type() == TypePiece.PAWN)
                    {
                        if (board[i, ii].AttackingTiles(new int[] { i, ii }).Count > 0)
                        {
                            points[i, ii] = (Mathf.Pow(board[i, ii].GetDistanceValue(), Mathf.Abs(board[i, ii].GetEffectiveRange() - Distance(RedPosition, new int[] { i, ii }))));
                            foreach (int[] pos in board[i, ii].AttackingTiles(new int[] { i, ii }))
                            {
                                if (board[pos[0], pos[1]].Team() == TypeTeam.BLUE)
                                    points[i, ii] += board[pos[0], pos[1]].GetPieceValue();
                            }
                        }
                    }
                    else
                    {
                        points[i, ii] = (Mathf.Pow(board[i, ii].GetDistanceValue(), Mathf.Abs(board[i, ii].GetEffectiveRange() - Distance(RedPosition, new int[] { i, ii }))));
                        foreach (int[] pos in board[i, ii].AttackingTiles(new int[] { i, ii }))
                        {
                            if (board[pos[0], pos[1]].Team() == TypeTeam.RED)
                                points[i, ii] += board[pos[0], pos[1]].GetPieceValue();
                        }
                    }
                }
            }
        }
        return points;

    end:
        return null;
    }


    private int[][] MoveCommander()
    {
        int[][] move = new int[][] { new int[] { -1 } };

        for (int i = 0; i < Board.I.Length; i++)
        {
            for (int ii = 0; ii < Board.I.Width; ii++)
            {
                if (board[i, ii].Type() == TypePiece.COMMANDER && board[i, ii].Team() == Team)
                {
                    move = new int[2][];
                    move[0] = new int[] { i, ii };
                    break;
                }
            }
            if (move[0][0] != -1)
            {
                break;
            }
        }
        if (board[move[0][0], move[0][1]].AttackingTiles(new int[] { move[0][0], move[0][1] }).Count == 0)
        {
            Debug.Log("Commander has no moves left");
            // end game
            Game.I.DeclareWinner(GetEnemyTeam(Team));
            return new int[][] { new int[] { -1 } };
        }
        else
            move[1] = GetPieceMove(move[0], AttackMovePriority.High);

        return move;
    }

    private int CountPieces(TypeTeam t)
    {
        int temp = 0;
        for (int i = 0; i < Board.I.Length; i++)
        {
            for (int ii = 0; ii < Board.I.Width; ii++)
            {
                if (board[i, ii].Team() == t)
                    temp++;
            }
        }
        return temp;
    }

    private float GetThreashHold(TypeTeam t)
    {
        int temp = 0;
        float tempPoints = 0;
        for (int i = 0; i < Board.I.Length; i++)
        {
            for (int ii = 0; ii < Board.I.Width; ii++)
            {
                if (board[i, ii].Team() == t)
                {
                    temp++;
                    tempPoints += points[i, ii];
                }
            }
        }

        return tempPoints / temp;
    }

    private enum AttackMovePriority
    {
        High,
        Low,
        Rand
    }

    private int[] GetPieceMove(int[] piece, AttackMovePriority priority = AttackMovePriority.Rand)
    {
        List<int[]> AllMoves = board[piece[0], piece[1]].AttackingTiles(new int[] { piece[0], piece[1] });
        if (board[piece[0], piece[1]].Type() == TypePiece.PAWN) // special case if piece is pawn
        {
            if (priority == AttackMovePriority.High)
            {
                if (AllMoves.Count > 0)
                {
                    if (points[AllMoves[0][0], AllMoves[0][1]] > 0)
                    {
                        return AllMoves[0];
                    }
                }
            }

            if (AllMoves[0][1] + 1 < Board.I.Width)
            {
                if (AllMoves[0][1] - 1 >= 0)
                {
                    int temp = (Random.Range(0f, 1f) > -0.5f) ? -1 : 1;
                    return new int[] { AllMoves[0][0], AllMoves[0][1] + temp };
                }
            }
            else if (AllMoves[0][1] - 1 >= 0)
            {
                return new int[] { AllMoves[0][0], AllMoves[0][1] - 1 };
            }
            return AllMoves[0]; // will force the game to ask the AI to make another move
        }
        else // Commander or supporter moves
        {
            if (priority == AttackMovePriority.High)
            {
                List<int> indexOfHighestValue = new List<int>();
                int temp = 0;

                foreach (int[] attack in AllMoves)
                {
                    if (board[AllMoves[temp][0], AllMoves[temp][1]].Type() != TypePiece.EMPTY)
                    {
                        float tempValue = board[AllMoves[temp][0], AllMoves[temp][1]].GetPieceValue();
                        if (indexOfHighestValue.Count > 0)
                        {
                            if (tempValue == board[AllMoves[indexOfHighestValue[0]][0], AllMoves[indexOfHighestValue[0]][1]].GetPieceValue())
                            {
                                //Debug.Log("another move of same value: " + tempValue);
                                indexOfHighestValue.Add(temp);
                            }
                            else if (tempValue > board[AllMoves[indexOfHighestValue[0]][0], AllMoves[indexOfHighestValue[0]][1]].GetPieceValue())
                            {
                                //Debug.Log("new heighest value:" + tempValue);
                                indexOfHighestValue.Clear();
                                indexOfHighestValue.Add(temp);
                            }
                        }
                        else
                        {
                            //Debug.Log("Setting initial value: " + tempValue +
                            //"Position:" + AllMoves[temp][0] + ", " + AllMoves[temp][1]);
                            indexOfHighestValue.Add(temp);
                        }
                    }
                    temp++;
                }
                if (indexOfHighestValue.Count == 0)
                    return AllMoves[Random.Range(0, AllMoves.Count)];
                else if (indexOfHighestValue.Count == 1)
                    return AllMoves[indexOfHighestValue[0]];
                //else if (indexOfHighestValue.Count > 1)
                else
                    return AllMoves[indexOfHighestValue[Random.Range(0, indexOfHighestValue.Count)]];
                //Debug.Log("Normal Move");
                // return AllMoves[0];
            }
            else if (priority == AttackMovePriority.Rand)
            {
                if (AllMoves.Count > 0)
                    return AllMoves[Random.Range(0, AllMoves.Count)];
                else
                    return AllMoves[0];
            }
            else
            {
                // make move valued lowest
                List<int> indexOfHighestValue = new List<int>();
                int temp = 0;
                //Debug.Log("Huh????");
                foreach (int[] attack in AllMoves)
                {
                    if (board[AllMoves[temp][0], AllMoves[temp][1]].Type() != TypePiece.EMPTY)
                    {
                        float tempValue = board[AllMoves[temp][0], AllMoves[temp][1]].GetPieceValue();
                        if (indexOfHighestValue.Count > 0)
                        {
                            if (tempValue == board[AllMoves[indexOfHighestValue[0]][0], AllMoves[indexOfHighestValue[0]][1]].GetPieceValue())
                            {
                                indexOfHighestValue.Add(temp);
                            }
                            else if (tempValue < board[AllMoves[indexOfHighestValue[0]][0], AllMoves[indexOfHighestValue[0]][1]].GetPieceValue())
                            {
                                indexOfHighestValue.Clear();
                                indexOfHighestValue.Add(temp);
                            }
                        }
                        else
                        {
                            indexOfHighestValue.Add(temp);
                        }
                    }
                    temp++;
                }

                if (indexOfHighestValue.Count > 0)
                    return AllMoves[indexOfHighestValue[Random.Range(0, indexOfHighestValue.Count)]];
                else
                {
                    //Debug.Log("All move count; " + AllMoves.Count);
                    if (board[piece[0], piece[1]].Type() == TypePiece.SUPPORTER)
                        foreach (int[] p in AllMoves)
                        {
                            return AllMoves[0];
                        }
                    return AllMoves[Random.Range(0, AllMoves.Count)];
                }
            }
        }
    }

    private int[][] RandomMove()
    {
        int[] piece = RandomPiece();
        int[] move = new int[2];
        board = Board.I.GetBoard();

        //Debug.Log("selecting move");
        int counter = 0, counterLimit = 50; // used to prevent an endless loop from occuring
        //int counterOverSeeer = 0; // helps

        while (true)
        {
            if (board[piece[0], piece[1]].Type() == TypePiece.SUPPORTER || board[piece[0], piece[1]].Type() == TypePiece.COMMANDER) // supporter or commander
            {
                //Debug.Log("Other Piece moves: " + board[piece[0], piece[1]].Type().ToString());
                try
                {
                    move = board[piece[0], piece[1]].AttackingTiles(piece)[Random.Range(0, board[piece[0], piece[1]].AttackingTiles(piece).Count)];
                }
                catch (System.Exception)
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
                catch (System.Exception)
                {
                    //Debug.LogWarning(e);
                    //Debug.LogWarning("Attempting to goto sweeper code");
                    goto SweeperCode;
                }

            }
            else // should not reach here || added for error checking
            {
                Debug.LogError("Piece does not have valid type");
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

        Debug.LogWarning("WHYYYYYYYYYYYYYYYYYYYY");
        Debug.LogWarning("If you've reached this point.....how");
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
