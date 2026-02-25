using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public enum TypeGame
{
    PASS_AND_PLAY,
    VS_AI
}

public enum GameState
{
    GameSetUp,
    InProgress,
    Ended
}

public enum AI_Difficulty
{
    nul,
    EASY,
    MEDIUM,
    HARD
}

public class Game : MonoBehaviour
{
    private static Game i;
    public static Game I { get { return i; } }

    void Awake()
    {
        if (i == null)
            i = this;
        else
            DestroyImmediate(this);
    }

    Player RedPlayer;
    Player BluePlayer;
    TypeTeam playerTurn; // player who's turn it is
    GameState gameState = GameState.GameSetUp;
    TypeTeam winningPlayer;
    TypeGame gameType;

    // timer variables
    private int MoveTimer = 0;
    private const int TimerLimit = 10; // in game rules
    private TypeTeam LosingPlayer;
    private bool timer = false;
    private AI_Difficulty BlueDifficulty, RedDifficulty;

    internal TypeTeam GetWinningTeam() { return winningPlayer; }

    public GameState GetGameState()
    {
        return gameState;
    }

    public void SetGameState(GameState s)
    {
        //Debug.Log(s.ToString());
        gameState = s;
        UI_Handler.I.ShowGameSettings(s);

        /*
        if (s != GameState.InProgress)
        {
            GameObject[] g = GameObject.FindGameObjectsWithTag("advancedAI");
            foreach (GameObject i in g)
            {
                DestroyImmediate(i);
            }
        }*/
    }

    internal void ShowDebugMessage(string v)
    {
        Debug.Log(v);
    }

    public Player getRedPlayer()
    {
        return RedPlayer;
    }

    public Player getBluePlayer()
    {
        return BluePlayer;
    }

    public void NewGame(TypePlayer Blue, TypePlayer Red, TypeTeam FirstPlayer, bool random)
    {
        playerTurn = FirstPlayer;

        RedPlayer = GeneratePlayer(TypeTeam.RED, Red);
        BluePlayer = GeneratePlayer(TypeTeam.BLUE, Blue);

        if (Blue == TypePlayer.HUMAN && Red == TypePlayer.HUMAN)
        {
            gameType = TypeGame.PASS_AND_PLAY;
        }
        else
        {
            gameType = TypeGame.VS_AI;
        }

        //resetting timer values
        MoveTimer = 0;
        timer = false;
        LosingPlayer = TypeTeam.nul;

        Board.I.NewBoard(random);

        SetGameState(GameState.InProgress);

        UI_Handler.I.SetUpBoard();

        StartCoroutine("GameFlow");
        //Debug.Log("Game FLow started");
        //NetworkManager manager = new NetworkManager(TypeTeam.BLUE);
        //manager.convertBoard();
    }

    Player GeneratePlayer(TypeTeam t, TypePlayer p)
    {
        if (p != TypePlayer.HUMAN)
            if (t == TypeTeam.BLUE)
            {
                BlueDifficulty = UI_Handler.I.GetDifficulty(t);
                //Debug.Log("Blue difficulty: " + BlueDifficulty.ToString());
            }
            else if (t == TypeTeam.RED)
            {
                RedDifficulty = UI_Handler.I.GetDifficulty(t);
                //Debug.Log("RED difficulty: " + RedDifficulty.ToString());
            }

        switch (p)
        {
            case TypePlayer.HUMAN:
                return new Human(t);
            case TypePlayer.ADHOC:
                return new Ad_hoc(t);

            case TypePlayer.RANDOM:
                return new RandomAI(t);
            default:
                Debug.LogError("Error Generating Player");
                break;
        }
        return null;
    }

    // variables in game flow
    int[] pos1;
    int[] pos2;
    bool takeTurn = false;

    // Game Flow !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private const float AIWaitTime = 0.8f; // how long the player will wait to see the AI move in seconds
    private float AIWaitTimeValue; // other valuse used to speed up game |||||||DO NOT SET AS A VALUE - Values is maintained during runtime|||||||
    private float TurnWaitTime = 0.1f; // how fast the game flow method ticks || can be extremely low move in seconds

    IEnumerator GameFlow() // handles the flow of the game
    {
        yield return new WaitForSeconds(0.05f);
        Player nextPlayer; // checks if AI is going first
        if (playerTurn == TypeTeam.BLUE)
            nextPlayer = BluePlayer;
        else
            nextPlayer = RedPlayer;

        if (nextPlayer.Type() != TypePlayer.HUMAN)
        {
            AIWaitTimeValue = 1f; // sets value so player can load in before AI has made its move
            StartCoroutine("AIMOVE");
        }

        while (true)
        {
            if (gameState != GameState.InProgress)
                break;
            if (takeTurn)
            {
                Turn(pos1, pos2);
                takeTurn = false;

                yield return new WaitForSeconds(0.05f);

                if (playerTurn == TypeTeam.BLUE)
                    nextPlayer = BluePlayer;
                else
                    nextPlayer = RedPlayer;

                if (nextPlayer.Type() != TypePlayer.HUMAN)
                {
                    StartCoroutine("AIMOVE");
                }

            }
            yield return new WaitForSeconds(TurnWaitTime); // waits to not use too many resources
        }
    }

    IEnumerator AIMOVE() // handles waiting for the AI to move
    {
        //Debug.Log("AI Move to be expected");
        Player nextPlayer;
        AI_Difficulty diff;
        if (playerTurn == TypeTeam.BLUE)
        {
            nextPlayer = BluePlayer;
            diff = BlueDifficulty;
        }
        else
        {
            nextPlayer = RedPlayer;
            diff = RedDifficulty;
        }

        if (nextPlayer.Type() == TypePlayer.ADVANCED)
        {
            if (gameState != GameState.InProgress) // checks if game is still in progress
            {
                StopCoroutine("AIMOVE");
                StopCoroutine("GameFlow");
            }
            GetAdvancedMove(nextPlayer, diff);
        }
        else
        {
            yield return new WaitForSeconds(AIWaitTimeValue); //waits to let code not clash and to let  the player experience playing againced an A player
            AIWaitTimeValue = 0.005f; // used to speed up game if AI makes a wrong move
            if (gameState != GameState.InProgress) // checks if game is still in progress
            {
                StopCoroutine("AIMOVE");
                StopCoroutine("GameFlow");
            }
            GetAIToMove(nextPlayer, diff);
        }
    }

    private void GetAdvancedMove(Player nextPlayer, AI_Difficulty diff)
    {
        nextPlayer.TakeTurn();
    }

    void GetAIToMove(Player nextPlayer, AI_Difficulty diff) // Gets AI to take their turn
    {
        int[][] move = nextPlayer.TakeTurn(diff);
        setMove(move[0], move[1], true);

        //Debug.LogWarning("AI move");
    }

    public void setMove(int[] pos1, int[] pos2, bool isAI = false) // Set move function sets the move that will be taken by the game
    {
        if (!isAI) // prevents player from intervening with AI movement
        {
            //Debug.Log("not AI");
            if (playerTurn == TypeTeam.BLUE)
            {
                //Debug.Log(BluePlayer.Type().ToString());
                if (BluePlayer.Type() != TypePlayer.HUMAN)
                    return;
            }
            else
            {
                //Debug.Log(RedPlayer.Type().ToString());
                if (RedPlayer.Type() != TypePlayer.HUMAN)
                    return;
            }
        }
        else
        {
            //Debug.Log("is AI");
            if (playerTurn == TypeTeam.BLUE)
            {
                //Debug.Log(BluePlayer.Type().ToString());
                if (BluePlayer.Type() == TypePlayer.HUMAN)
                    return;
            }
            else
            {
                //Debug.Log(RedPlayer.Type().ToString());
                if (RedPlayer.Type() == TypePlayer.HUMAN)
                    return;
            }
        }

        this.pos1 = pos1;
        this.pos2 = pos2;
        takeTurn = true;
    }
    /*
    static void CallToChildThread() // testing multithreading
    {
        try
        {
            Debug.Log("Child thread starts");

            // do some work, like counting to 10
            for (int counter = 0; counter <= 10; counter++)
            {
                Thread.Sleep(500);
                Debug.Log(counter);
            }

            Debug.Log("Child Thread Completed");
        }
        catch (ThreadAbortException)
        {
            Debug.LogWarning("Thread Abort Exception");
        }
        finally
        {
            Debug.LogWarning("Couldn't catch the Thread Exception");
        }
    }
    
    public static void StartMultithreading()
    {
        ThreadStart childref = new ThreadStart(CallToChildThread);

        Thread childThread = new Thread(childref);
        childThread.Start();
    }
    */
    internal void Turn(int[] pos1, int[] pos2) // Called each time a turn is taken
    {

        if (timer)
        {
            if (playerTurn == LosingPlayer)
            {
                MoveTimer++;
                if (MoveTimer >= TimerLimit)
                {
                    if (LosingPlayer == TypeTeam.BLUE)
                    {
                        winningPlayer = TypeTeam.BLUE;
                        DeclareWinner(TypeTeam.RED);
                    }
                    else
                    {
                        winningPlayer = TypeTeam.RED;
                        DeclareWinner(TypeTeam.BLUE);
                    }
                    return; // ends turn
                }
                //Debug.Log("Timer tick: " + MoveTimer);
            }
        }

        if (Board.I.GetTeamOfPiece(pos1) == playerTurn)
        {
            MoveType move = Board.I.AttemptMove(pos1, pos2);

            if (move != MoveType.FAIL) // security against error
            {
                TogglePlayer();
                if (move == MoveType.TAKE && timer)
                {
                    //Debug.Log(move.ToString() + "Timer reset");
                    MoveTimer = 0;
                }
            }
            //need to record move

        }

        TypeTeam t = Board.I.CheckForWinner();

        if (t != TypeTeam.nul)
        {
            winningPlayer = t;
            DeclareWinner(t);
        }
    }

    internal void StartTimer(TypeTeam losingTeam) // starts the draw timer
    {
        //Debug.Log("Timer Started");
        LosingPlayer = losingTeam;
        timer = true;
    }

    internal void DeclareWinner(TypeTeam t) // declares the winner of the game and 
    {
        winningPlayer = t;
        SetGameState(GameState.Ended);
        StopCoroutine("AIMOVE");
        StopCoroutine("GameFlow");
    }

    private void TogglePlayer()
    {
        if (playerTurn == TypeTeam.BLUE)
        {
            playerTurn = TypeTeam.RED;
        }
        else if (playerTurn == TypeTeam.RED)
        {
            playerTurn = TypeTeam.BLUE;
        }
        AIWaitTimeValue = AIWaitTime;

        UI_Handler.I.UpdateTeam(playerTurn);
    }

    // Utility testing code +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    /// <summary>
    /// Code past this point was used to test the utility function. The complete function is used in the ADhocAI class
    /// </summary>
    /*Piece[,] board;
    float[,] points;

    public void TestUtilityFunction() // Feels finished
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
        Debug.LogError("Enemy Not found");
        throw new System.NotImplementedException();

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
                                // maky worky
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
                            // maky worky
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
                                // maky worky
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
                            // maky worky
                            if (board[pos[0], pos[1]].Team() == TypeTeam.RED)
                                points[i, ii] += board[pos[0], pos[1]].GetPieceValue();
                        }
                    }
                }
            }
        }
        PrintToFile();
    }

    private float Distance(int[] pos1, int[] pos2)
    {
        return Mathf.Sqrt((Mathf.Pow(pos1[0] - pos2[0], 2) + Mathf.Pow(pos1[1] - pos2[1], 2)));
    }

    private float GetTotalPoints(float[,] boardPoints, TypeTeam t)
    {
        float points = 0f;
        for (int i = 0; i < Board.I.Length; i++)
        {
            for (int ii = 0; ii < Board.I.Width; ii++)
            {
                if (Board.I.GetBoard()[i, ii].Team() == t)
                {
                    points += boardPoints[i, ii];
                }
            }
        }
        return points;
    }

    private TypeTeam GetEnemyTeam(TypeTeam t)
    {
        if (t == TypeTeam.BLUE)
            return TypeTeam.RED;
        else
            return TypeTeam.BLUE;
    }

    // Printing variables
    int printnumber = 0;
    public void PrintToFile()// comtains specific directries 
    {
        if (!File.Exists(printnumber.ToString()))
            using (StreamWriter writer = File.CreateText("D:\\School\\3rd Year\\GADE7311\\POE\\Michael Pansegrauw 19013147 GADE7311  POE\\_TestingData\\" + printnumber.ToString() + ".txt"))
            {
                printnumber++;
                string output = "Print number:" + printnumber +
                    "\nTeam: " + TypeTeam.BLUE +
                    "\nPoints: " + GetTotalPoints(points, TypeTeam.BLUE) +
                    "\nTeam: " + TypeTeam.RED +
                    "\nPoints: " + GetTotalPoints(points, TypeTeam.RED) + "\n";

                for (int i = 0; i < Board.I.Length; i++)
                {
                    for (int ii = 0; ii < Board.I.Width; ii++)
                    {
                        if (points[i, ii] != 0)
                            output += "||\t" + points[i, ii];
                        else
                            output += "||\t\t\t" + points[i, ii];
                    }
                    output += "||\n";
                }
                output += "\n\n";
                for (int i = 0; i < Board.I.Length; i++)
                {
                    for (int ii = 0; ii < Board.I.Width; ii++)
                    {
                        output += "||\t" + board[i, ii];
                    }
                    output += "||\n";
                }

                writer.WriteLine(output);
            }
    }*/
}
