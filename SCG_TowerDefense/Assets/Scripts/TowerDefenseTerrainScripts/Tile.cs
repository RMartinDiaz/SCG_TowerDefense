using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    public Vector2Int tilePosition;

    public GameObject tileObject;

    public enum TileType
    {
        grass,
        roadborder,
        road
    }

    public TileType tileType;

    public Tile(Vector2Int tilePos, GameObject tileGO, TileType type)
    {
        tilePosition = tilePos;

        tileObject = tileGO;

        tileType = type;
    }
}
