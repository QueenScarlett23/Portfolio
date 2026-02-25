using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_Handler : MonoBehaviour
{
    private static UI_Handler i;
    public static UI_Handler I { get { return i; } }
    void Awake()
    {
        if (i == null)
            i = this;
        else
            DestroyImmediate(this);
    }

    // assigned in unity
    public GameObject Tile;
    public GameObject boardSpace;
    public GameObject[,] Tiles;
    public GameObject[] BoarderTiles;
    public GameObject GameSetUp;
    public GameObject InGameUI;
    public GameObject EndScreen;
    public Dropdown RedPlayer;
    public Dropdown BluePlayer;
    public Dropdown BoardType;
    public GameObject RedPlayerDifficulty;
    public GameObject BluePlayerDifficulty;
    public Dropdown StartingPlayer;
    public Text txtPlayerTurn;
    public Text txtWinningTeam;
    public GameObject endGameButton;

    // variables
    private int[] SelectedTile;
    private bool randomBoard;

    // perlin noise
    float magnification = 7;
    int[] x_offset;
    int[] y_offset;


    private void Start()
    {
        ShowGameSettings(GameState.GameSetUp);
        RedPlayerDifficulty.SetActive(false);
        BluePlayerDifficulty.SetActive(false);
        colorMap = new Color[10, 7];
        for (int i = 0; i < 10; i++)
        {
            for (int ii = 0; ii < 7; ii++)
            {
                colorMap[i, ii] = Color.blue;
            }
        }
    }

    public void showBlueDifficulty()
    {
        if (BluePlayer.options[BluePlayer.value].text.ToLower() != "human" && BluePlayer.options[BluePlayer.value].text.ToLower() != "random")
        {
            BluePlayerDifficulty.SetActive(true);
        }
        else
        {
            BluePlayerDifficulty.SetActive(false);
        }
    }

    public void showRedDifficulty() // shows the options for the difficulty selection
    {
        if (RedPlayer.options[RedPlayer.value].text.ToLower() != "human" && RedPlayer.options[RedPlayer.value].text.ToLower() != "random")
        {
            RedPlayerDifficulty.SetActive(true);
        }
        else
        {
            RedPlayerDifficulty.SetActive(false);
        }
    }

    public void NewGame()// code resposible for starting new games
    {
        randomBoard = (BoardType.options[BoardType.value].text.ToLower() == "random") ? true:  false;
        TypeTeam t = GetTypeTeam(StartingPlayer.options[StartingPlayer.value].text);
        Game.I.NewGame(
            GetPlayerType(BluePlayer.options[BluePlayer.value].text),   // Blue Player type
            GetPlayerType(RedPlayer.options[RedPlayer.value].text),     // Red Player type
            t,                                                         // Player that goes first
            randomBoard);
        UpdateTeam(t);
        //Game.I.SetGameState(GameState.InProgress);
    }

    public void EndGame()
    {
        BoardWipe();
        Game.I.SetGameState(GameState.GameSetUp);
    }

    internal void TileClicked(int[] t) // Handles what happens when a tile is clicked
    {
        if (SelectedTile == null)
        {
            SelectedTile = t;
            SetTileSelected(true);
        }
        else if (ComparePos(SelectedTile, t))
        {
            SetTileSelected(false);
            SelectedTile = null;
        }
        else
        {
            // makes move
            SetTileSelected(false);
            Game.I.setMove(SelectedTile, t);
            SelectedTile = null;
        }
    }

    private void SetTileSelected(bool select)// colours the tile so the player knows what tile was clicked
    {
        if (!randomBoard)
            Tiles[SelectedTile[0], SelectedTile[1]].GetComponent<Image>().color = (select) ? GetColor(TypeTeam.NONE) : GetColor(TypeTeam.nul);
        else
            Tiles[SelectedTile[0], SelectedTile[1]].GetComponent<Image>().color = (select) ? GetColor(TypeTeam.NONE) : colorMap[SelectedTile[0], SelectedTile[1]];
    }

    private bool ComparePos(int[] tile1, int[] t) // checkis if the tiles are the same ones clicked
    {
        if (tile1[0] == t[0])
            if (tile1[1] == t[1])
                return true;
        return false;
    }

    internal void ShowGameSettings(GameState state) // handles what the player sees
    {
        switch (state)
        {
            case GameState.GameSetUp:
                GameSetUp.SetActive(true);
                InGameUI.SetActive(false);
                EndScreen.SetActive(false);

                BoardWipe();

                break;
            case GameState.InProgress:
                InGameUI.SetActive(true);
                GameSetUp.SetActive(false);
                EndScreen.SetActive(false);

                endGameButton.SetActive(true);

                break;
            case GameState.Ended:
                if (Game.I.GetWinningTeam() != TypeTeam.NONE)
                {
                    //Debug.Log(Game.I.GetWinningTeam().ToString());
                    txtWinningTeam.text = Game.I.GetWinningTeam().ToString();
                }
                else if (Game.I.GetWinningTeam() == TypeTeam.NONE)
                {
                    //Debug.Log("Draw");
                    txtWinningTeam.text = "Draw";
                }
                else
                {
                    Debug.LogError("Error ending game");
                    return;
                }
                GameSetUp.SetActive(false);
                InGameUI.SetActive(true);
                EndScreen.SetActive(true);

                endGameButton.SetActive(false);

                break;
            default:
                break;
        }
    }

    private void BoardWipe()
    {
        try
        {
            foreach (GameObject t in Tiles)
            {
                Destroy(t);
            }
        }
        catch (Exception)
        {
            //Debug.LogWarning(e);
        }

        try
        {
            foreach (GameObject t in BoarderTiles)
            {
                Destroy(t);
            }
        }
        catch (Exception)
        {
            //Debug.LogError(e);
        }
    }

    private Color[,] colorMap; // map co colours for wach tile

    // sets up board by creating buttons with images on the buttons
    public void SetUpBoard()
    {
        if (randomBoard)
            AssignPerlinColoursMap();

        Piece[,] board = Board.I.GetBoard();
        Tiles = new GameObject[Board.I.Length, Board.I.Width];
        BoarderTiles = new GameObject[Board.I.Length + Board.I.Width];
        bool tileType = false;

        float tileXedge = 625;
        float tileYedge = 150;

        int temp = 0;

        string[] letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

        for (int i = -1; i < Board.I.Length; i++)
        {
            tileType = (i % 2 == 0) ? true : false;
            for (int ii = -1; ii < Board.I.Width; ii++)
            {
                if (i == -1 || ii == -1)
                {
                    if (i == -1 && ii == -1)
                    {
                    }
                    else
                    {
                        //edge of board 
                        BoarderTiles[temp] = Instantiate(Tile, boardSpace.transform);
                        BoarderTiles[temp].transform.localPosition = new Vector3(tileXedge + (ii * 100f), (tileYedge + (i * 100f)), 0);
                        BoarderTiles[temp].GetComponent<Button>().interactable = false;
                        if (i == -1)
                        {
                            BoarderTiles[temp].GetComponentInChildren<Text>().text = (letters[ii]) + "";
                        }
                        else
                        {
                            BoarderTiles[temp].GetComponentInChildren<Text>().text = (i + 1) + "";
                        }
                        temp++;
                    }
                }
                else
                {
                    //board
                    boardSpace.transform.position = new Vector3(0, 0, 0);
                    Tiles[i, ii] = Instantiate(Tile, boardSpace.transform);
                    Tiles[i, ii].transform.localPosition = new Vector3(tileXedge + (ii * 100f), (tileYedge + (i * 100f)), 0);
                    if (randomBoard)
                        Tiles[i, ii].GetComponent<Image>().color = colorMap[i, ii];
                    else
                        Tiles[i, ii].GetComponent<Image>().color = GetColor(TypeTeam.nul);
                    Tiles[i, ii].GetComponent<TileController>().SetCoordinate(i, ii);

                    if (tileType) // makes tile colours
                    {
                        Tiles[i, ii].GetComponent<Image>().sprite = AssetMaster.I.Tile_Light;
                        tileType = false;
                    }
                    else
                    {
                        Tiles[i, ii].GetComponent<Image>().sprite = AssetMaster.I.Tile_Dark;
                        tileType = true;
                    }

                    Tiles[i, ii].GetComponentInChildren<Text>().GetComponentInChildren<Image>().sprite = GetPiece(board[i, ii].Type());
                    Tiles[i, ii].GetComponentInChildren<Text>().GetComponentInChildren<Image>().color = GetColor(board[i, ii].Team());
                }
            }
        }
    }

    public void UpdateBoard(int[] pos1, int[] pos2, Piece p)
    {
        Tiles[pos1[0], pos1[1]].GetComponentInChildren<Text>().GetComponentInChildren<Image>().sprite = GetPiece(TypePiece.EMPTY);
        Tiles[pos2[0], pos2[1]].GetComponentInChildren<Text>().GetComponentInChildren<Image>().sprite = GetPiece(p.Type());
        Tiles[pos2[0], pos2[1]].GetComponentInChildren<Text>().GetComponentInChildren<Image>().color = GetColor(p.Team());
    }

    // Colour Stufff *************************************************************************************************************
    private Color GetColor(TypeTeam typeTeam)
    {
        switch (typeTeam)
        {
            case TypeTeam.RED:
                return new Color(0.9f, 0.2f, 0.2f);
            case TypeTeam.BLUE:
                return new Color(0.2f, 0.2f, 0.8f);
            case TypeTeam.NONE:
                return new Color(1, 0.9f, 0.8f);
            default:
                return new Color(0.7f, 0.4f, 0.95f);

        }
    }

    private void AssignPerlinColoursMap()
    {
        x_offset = new int[] { UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100) };
        y_offset = new int[] { UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100) };
        magnification = UnityEngine.Random.Range(4f, 20f);

        for (int i = 0; i < 10; i++)
        {
            for (int ii = 0; ii < 7; ii++)
            {
                colorMap[i, ii] = GetPerlinColour(i, ii);
            }
        }
    }

    private Color GetPerlinColour(int x, int y)
    {
        float noise1 = Mathf.Clamp(
            (float)Mathf.PerlinNoise(
                (x + x_offset[0]) / (magnification),
                (y + y_offset[0]) / (magnification)),
            0f, 1f);
        float noise2 = Mathf.Clamp(
            (float)Mathf.PerlinNoise(
                (x + x_offset[1]) / (magnification),
                (y + y_offset[1]) / (magnification)),
            0f, 1f);
        float noise3 = Mathf.Clamp(
            (float)Mathf.PerlinNoise(
                (x + x_offset[2]) / (magnification),
                (y + y_offset[2]) / (magnification)),
            0f, 1f);

        return new Color(noise1, noise2, noise3, 1);
    }

    // Difficulty stuff **********************************************************************************************************
    internal AI_Difficulty GetDifficulty(TypeTeam t)
    {
        if (t == TypeTeam.BLUE)
        {
            return GetDifficulty(BluePlayerDifficulty.GetComponent<Dropdown>().options[BluePlayerDifficulty.GetComponent<Dropdown>().value].text);
        }
        else if (t == TypeTeam.RED)
        {
            return GetDifficulty(RedPlayerDifficulty.GetComponent<Dropdown>().options[RedPlayerDifficulty.GetComponent<Dropdown>().value].text);
        }

        Debug.LogError("Method not called properly");   //  reaching this point in this code will mean that the method is not used correctly
        throw new System.NotImplementedException();     // and mean that code is not implimented properly
        /// should not reach this point
        /// what are you doing snooping around my code there
        /// trying to find an error?
        /// you'll neve find one!!!!!!!!!!
        /// never I tell you!
        /// NEVERRRRRRRRRRRRRRRRR
    }

    private AI_Difficulty GetDifficulty(string t)
    {
        switch (t.ToLower())
        {
            case "hard":
                return AI_Difficulty.HARD;
            case "medium":
                return AI_Difficulty.MEDIUM;
            case "easy":
                return AI_Difficulty.EASY;
        }
        return AI_Difficulty.nul;
    }

    // Get items *****************************************************************************************************************
    private Sprite GetPiece(TypePiece t)
    {
        //return AssetMaster.I.Commander;
        switch (t)
        {
            case TypePiece.EMPTY:
                //Debug.LogWarning("Empty tile requested");
                return AssetMaster.I.Empty;
            case TypePiece.PAWN:
                return AssetMaster.I.Pawn;
            case TypePiece.SUPPORTER:
                return AssetMaster.I.Supporter;
            case TypePiece.COMMANDER:
                return AssetMaster.I.Commander;
            default:
                Debug.LogError("Piece not found: Returning empty");
                return AssetMaster.I.Empty;
        }
    }

    private TypePlayer GetPlayerType(string t)
    {
        switch (t.ToLower())
        {
            case "human":
                return TypePlayer.HUMAN;
            case "ad-hoc":
                return TypePlayer.ADHOC;
            case "random":
                return TypePlayer.RANDOM;
            case "advanced":
                return TypePlayer.ADVANCED;
            default:
                Debug.LogError("Player type not specified: Returning Human Player Type");
                return TypePlayer.HUMAN;
        }
        // please don't break
    }

    private TypeTeam GetTypeTeam(string t)
    {
        switch (t.ToLower())
        {
            case "red":
                return TypeTeam.RED;
            case "blue":
                return TypeTeam.BLUE;
            default:
                if (UnityEngine.Random.Range(-1f, 1f) > 0)
                    return TypeTeam.RED;
                else return TypeTeam.BLUE;
        }
    }

    // misc **********************************************************************************************************************
    internal void UpdateTeam(TypeTeam t)
    {
        txtPlayerTurn.text = t.ToString();
    }

    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}