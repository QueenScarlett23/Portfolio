using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class AssetMaster : MonoBehaviour
{
    private static AssetMaster i;
    public static AssetMaster I { get { return i; } }

    private void Awake()
    {
        if (i == null)
        {
            i = this;
        } else
        {
            DestroyImmediate(this);
        }
    }

    public Sprite Tile_Light;
    public Sprite Tile_Dark;
    public Sprite Empty;
    public Sprite Pawn;
    public Sprite Supporter;
    public Sprite Commander;
}
