using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI_Logic : MonoBehaviour
{
    private static AI_Logic i;

    public static AI_Logic I { get { return i; } }

    void Awake()
    {
        if (i == null)
        {
            i = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    public void test()
    {
        string[,] test = {
        {"O", "X", "O" },
        {"X", "X", "" },
        {"", "O", "X" } };

        CalculateValues(test);
    }

    /*
     * 
     * AI needs fixing
     * AI needs to take victory instead of delaying enemy
     * 
     */

    private float[,] CalculateValues(string[,] board) // not done
    {
        float[,] values = new float[9, 2]; // values of each space 
        float[,] columnValues = new float[8, 2]; // all columns so you dont need to calculate them repetedly
                                                 /* 
                                                  * x 0   1   2  3 
                                                  *   0 | 1 | 2  4
                                                  *  ___________
                                                  *   3 | 4 | 5  5
                                                  *  ___________
                                                  *   6 | 7 | 8  6
                                                  *              7
                                                  *              y
                                                  *              
                                                  * How the collums are sorted
                                                  */

        //adjacent tiles score 
        // tile value done per player
        // time valie = 0,1 * 10 ^ (x - 1) x is amount of tiles in a line
        // do for each space and each line

        for (int i = 0; i < 3; i++) // setting of liniar values
        {
            int col1X = 0, col1Y = 0, col2X = 0, col2Y = 0;
            for (int ii = 0; ii < 3; ii++)
            {
                //first column
                string tag = board[2 - i, ii];
                if (tag == GameControler.I.tag_player_1 + "")
                {
                    col2X++;
                }
                else if (tag == GameControler.I.tag_player_2 + "")
                {
                    col2Y++;
                }

                // second column
                tag = board[2 - ii, i];
                if (tag == GameControler.I.tag_player_1 + "")
                {
                    col1X++;
                }
                else if (tag == GameControler.I.tag_player_2 + "")
                {
                    col1Y++;
                }
            }
            columnValues[i, 0] = (Mathf.Pow(10, (col1X - 2)));
            columnValues[i, 1] = (Mathf.Pow(10, (col1Y - 2)));
            columnValues[6 - i, 0] = (Mathf.Pow(10, (col2X - 2)));
            columnValues[6 - i, 1] = (Mathf.Pow(10, (col2Y - 2)));
        }

        // setting of diagonal values
        int colX = 0, colY = 0;
        string boardVal;
        for (int i = 0; i < 3; i++)
        {
            boardVal = board[i, i];
            if (boardVal == GameControler.I.tag_player_1 + "")
                colX++;
            else if (boardVal == GameControler.I.tag_player_2 + "")
                colY++;

        }
        columnValues[7, 0] = (Mathf.Pow(10, (colX - 2)));
        columnValues[7, 1] = (Mathf.Pow(10, (colY - 2)));

        colX = 0; colY = 0;
        for (int i = 0; i < 3; i++)
        {
            boardVal = board[2 - i, i];
            if (boardVal == GameControler.I.tag_player_1 + "")
                colX++;
            else if (boardVal == GameControler.I.tag_player_2 + "")
                colY++;
        }

        columnValues[3, 0] = (Mathf.Pow(10, (colX - 2)));
        columnValues[3, 1] = (Mathf.Pow(10, (colY - 2)));

        // checking of column values
        for (int i = 0; i < 8; i++)
        {
            if (columnValues[i, 0] < 0.1)
            {
                columnValues[i, 0] = 0;
            }
            if (columnValues[i, 1] < 0.1)
            {
                columnValues[i, 1] = 0;
            }
            //Debug.Log(i + " : " + columnValues[i, 0] + ", " + columnValues[i, 1]);
        }
        // setting of board values

        //values[0, 0] = ;
        //values[0, 1] = ;

        for (int i = 0; i < 3; i++)
        {
            values[i, 0] += columnValues[4, 0];
            values[i, 1] += columnValues[4, 1];

            values[i + 3, 0] += columnValues[5, 0];
            values[i + 3, 1] += columnValues[5, 1];

            values[i + 6, 0] += columnValues[6, 0];
            values[i + 6, 1] += columnValues[6, 1];

            values[i * 3, 0] += columnValues[0, 0];
            values[i * 3, 1] += columnValues[0, 1];

            values[1 + (i * 3), 0] += columnValues[1, 0];
            values[1 + (i * 3), 1] += columnValues[1, 1];

            values[2 + (i * 3), 0] += columnValues[2, 0];
            values[2 + (i * 3), 1] += columnValues[2, 1];
        }

        for (int i = 0; i < 3; i++)
        {
            values[i * 4, 0] += columnValues[7, 0];
            values[i * 4, 1] += columnValues[7, 1];

            values[2 + (i * 2), 0] += columnValues[3, 0];
            values[2 + (i * 2), 1] += columnValues[3, 1];
        }
        /*
        for (int i = 0; i < 9; i++)
        {
            Debug.Log(i + " :" + values[i, 0] + ", " + values[i, 1] + " -- " + (values[i, 0] + values[i, 1]));
        }*/
        return values;
    }

    public int[] CalculateMove(int MoveNumber) // not done
    {

        //Debug.Log("MoveNumber: " + MoveNumber);

        int[] move = { 0, 0 }; // default move is middle of board
        // no need to check middle board
        string[,] board = Game.I.GetBoard(); // board with spaces

        // check for states
        if (MoveNumber == 0)
        {
            move[0] = 1;
            move[1] = 1;
            return move;
        }
        else if (MoveNumber == 1)
        {
            if (board[1, 1] == null || board[1, 1] == "")
            {
                move[0] = 1;
                move[1] = 1;
            }
            return move;
        }
        else if (MoveNumber == 2)
        {
            if (board[1, 1] == GameControler.I.tag_player_1)
            {
                if (board[0, 0] == GameControler.I.tag_player_2)
                {
                    move[0] = 2;
                    return move;
                }
                else
                {
                    return move;
                }
            }
        }

        if (MoveNumber == 3)
        {
            //check values of corners
            if (board[0, 0] == GameControler.I.tag_player_1.ToString())
            {
                if (board[1, 1] == GameControler.I.tag_player_2.ToString() && board[2, 2] == GameControler.I.tag_player_1.ToString())
                {
                    move[1] = 1;
                    Debug.LogError("Move taken wring cause bitch");
                    return move;
                }
            }
            else if (board[0, 2] == GameControler.I.tag_player_1.ToString())
            {
                if (board[1, 1] == GameControler.I.tag_player_2.ToString() && board[2, 0] == GameControler.I.tag_player_1.ToString())
                {
                    move[1] = 1;
                    Debug.LogError("Move taken wring cause bitch");
                    return move;
                }
            }

        }

        float[,] boardValues = CalculateValues(board);
        int topValue = -1;
        float topValueTotal = 0;

        //Debug.LogWarning("Board");
        for (int i = 0; i < 9; i++)
        {

            if (board[(int)(i / 3), i % 3] == null || board[(int)(i / 3), i % 3] == "")
            {
                //Debug.Log("board Space" + i + " :" + boardValues[i, 0] + ", " + boardValues[i, 1] + "--" +  i % 2);
                if (topValue == -1)
                {
                    topValue = i;
                    topValueTotal = (boardValues[i, 0] + boardValues[i, 1]);
                }
                else
                {
                    if ((boardValues[i, (MoveNumber) % 2]) >= 1)
                    {
                        move[0] = (int)(i / 3);
                        move[1] = (i % 3);
                        return move; 
                    }
                    float temp = (boardValues[i, 0] + boardValues[i, 1]);
                    if (temp >= topValueTotal)
                    {
                        if (temp == topValueTotal || temp >= 1)
                        {
                            if (boardValues[i, MoveNumber % 2] > boardValues[topValue, MoveNumber % 2]) // use mod 2  to deturmin which values favours AI more
                            {

                                topValue = i;
                                topValueTotal = temp;
                            }
                            if (temp > topValueTotal)
                            {
                                topValue = i;
                                topValueTotal = temp;
                            }
                        }
                        else
                        {
                            topValue = i;
                            topValueTotal = temp;
                            //Debug.Log("iteration : " + i + "\n temp value: " + temp + "\n top Values: " + topValueTotal +  "\n prevvious board values: " + boardValues[topValue, 0] + ", " + boardValues[topValue, 1]);
                            

                        }

                    }
                }
            }
        }

        move[0] = (int)(topValue / 3);
        move[1] = (topValue % 3);

        /*
        Debug.LogError("Board Vlaues");
        for (int i = 0; i < 9; i++)
        {
            Debug.Log("Board Values: " + i + " :" + boardValues[i, 0] + ", " + boardValues[i, 1]);
        }*/
        return move;
    }
}
