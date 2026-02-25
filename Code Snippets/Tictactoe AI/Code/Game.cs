using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private static Game i;

    public static Game I { get { return i; } }

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

    private string[,] board;
    public MOVE_RESULT game_result;

    public void newGame()
    {
        board = new string[3, 3];
    }

    public bool SetPosition(int x, int y, string c)
    {
        if (board[x, y] == null || board[x, y] == "")
        {
            board[x, y] = c + "";
            CheckGameState();
            //PrintBoard();
            return true;
        }
        else
        {
            Debug.Log("Illegal Move: Space taken");
            return false;
        }
    }

    private void PrintBoard()
    {
        Debug.LogError("Board");
        for (int i = 0; i < 3; i++)
        {
            for (int ii = 0; ii < 3; ii++)
            {
                Debug.Log(board[i, ii]);
            }
        }
    }

    internal string[,] GetBoard()
    {
        return board;
    }

    private void CheckGameState()
    {
        float[,] columnValues = new float[8, 2];

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

        game_result = MOVE_RESULT.nul;
        for (int i = 0; i < 8; i++)
        {
            if (columnValues[i, 0] >= 10)
                game_result = MOVE_RESULT.player_1_win;
            if (columnValues[i, 1] >= 10)
                game_result = MOVE_RESULT.player_2_win;
        }
    }
}
