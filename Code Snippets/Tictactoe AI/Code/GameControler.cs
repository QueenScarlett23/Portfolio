using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MOVE_RESULT
{
    player_1_win,
    player_2_win,
    nul
}

public class GameControler : MonoBehaviour
{
    private static GameControler i;

    public static GameControler I { get { return i; } }

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

    public readonly string tag_player_1 = "♥";
    public readonly string tag_player_2 = "❖";

    bool PlayerTurn = true;
    bool AI = true;
    bool AI_start = false;

    int Moves = 0;

    private GameObject banner;

    void Start()
    {
        banner = GameObject.FindGameObjectWithTag("WinnerBanner");

        banner.SetActive(false);
    }

    public void Play()
    {
        Game.I.newGame();
        Moves = 0;
    }

    public void AI_Move()
    {
        int[] move = AI_Logic.I.CalculateMove(Moves);
        //Debug.LogWarning("AI : " + move[0] + ", " + move[1]);

        string s = Move(move[0], move[1]);
        if (s != null && s != "")
        {
            string btnNumber = ((move[0] * 3) + move[1] + 1).ToString();
            Button[] Buttons = GameObject.FindObjectsOfType<Button>();
            foreach (Button b in Buttons)
            {
                if (b.name == btnNumber)
                    b.GetComponentInChildren<Text>().text = s;
            }
        }
        DeclareWinner();
    }

    public string Move(int x, int y)
    {
        if (PlayerTurn) // if true then it is player 1's turn to move
        {
            //moveVerify = Game.I.SetPosition(x, y, tag_player_1);
            if (Game.I.SetPosition(x, y, tag_player_1 + ""))
            {
                PlayerTurn = false;
                Moves++;
                return tag_player_1 + "";
            }
        }
        else
        {
            //moveVerify = Game.I.SetPosition(x, y, tag_player_2);
            if (Game.I.SetPosition(x, y, tag_player_2))
            {
                PlayerTurn = true;
                Moves++;
                return tag_player_2 + "";
            }
        }

        return "";
    }

    public void SwitchStartingPlayer()
    {
        if (Moves == 0)
            PlayerTurn = (PlayerTurn) ? false : true;
    }

    public void ButtonPressed(Button buttonPressed)
    {
        string s = "";
        switch (int.Parse(buttonPressed.name))
        {
            case 1:
                s = Move(0, 0); break;
            case 2:
                s = Move(0, 1); break;
            case 3:
                s = Move(0, 2); break;
            case 4:
                s = Move(1, 0); break;
            case 5:
                s = Move(1, 1); break;
            case 6:
                s = Move(1, 2); break;
            case 7:
                s = Move(2, 0); break;
            case 8:
                s = Move(2, 1); break;
            case 9:
                s = Move(2, 2); break;
            default:
                Debug.Log("Input not recognised");
                break;
        }

        if (s != null && s != "")
        {
            //Debug.Log(s);
            buttonPressed.GetComponentInChildren<Text>().text = s;
        }

        DeclareWinner();
    }

    private void DeclareWinner()
    {
        switch (Game.I.game_result)
        {
            case MOVE_RESULT.player_1_win:
                banner.SetActive(true);
                banner.GetComponentInChildren<Text>().text = "Player 1 wins";
                break;
            case MOVE_RESULT.player_2_win:
                banner.SetActive(true);
                banner.GetComponentInChildren<Text>().text = "Player 2 wins";
                break;
            default:
                break;
        }
    }
}
