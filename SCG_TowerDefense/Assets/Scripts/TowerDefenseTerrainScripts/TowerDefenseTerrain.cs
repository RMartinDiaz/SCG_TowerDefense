using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TowerDefenseTerrain : MonoBehaviour
{
    // Defines the size of the terrain.
    public Vector2Int terrainSize = new Vector2Int(32, 18);

    // Keeps the tiles stored.
    public List<Tile> tiles = new List<Tile>();
    public List<Waypoint> waypoints = new List<Waypoint>();
#if (UNITY_EDITOR)
    [MenuItem("GameObject/" + ProjectValues.devPrefix + "/Tower Defense Terrain", false, 10)]
    static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        // Create a game object with the Tower Defense Terrain script.
        GameObject go = new GameObject("Tower Defense Terrain", typeof(TowerDefenseTerrain));

        // Ensure it gets reparented if this was a context click (otherwise does nothing).
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

        // Register the creation in the undo system.
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;

        // Create the default tile map.
        TowerDefenseTerrain TDTerrain = go.GetComponent<TowerDefenseTerrain>();
        TDTerrain.CreateTileMap(TDTerrain.terrainSize.x, TDTerrain.terrainSize.y);
    }
#endif

    // Creates a tile map depending on the size X and size Y.
    public void CreateTileMap(int mapSizeX, int mapSizeY)
    {
        for (int yPointer = 0; yPointer < terrainSize.y; yPointer++)
        {
            for (int xPointer = 0; xPointer < terrainSize.x; xPointer++)
            {
                GameObject newTileGO = new GameObject("Tile_" + xPointer + "_" + yPointer, typeof(SpriteRenderer));

                newTileGO.transform.SetParent(this.transform);
                newTileGO.transform.position = new Vector3(xPointer, 0.0f, yPointer);
                newTileGO.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));

                newTileGO.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("MapSpriteSheet")[2] as Sprite;

                Tile newTile = new Tile(new Vector2Int(xPointer, yPointer), newTileGO, Tile.TileType.grass);

                tiles.Add(newTile);
            }
        }
    }

    public Tile GetTileAt(int xPosition, int yPosition)
    {
        //Debug.Log("Requested tile at : " + xPosition + "_" + yPosition);

        if (tiles == null)
        {
            Debug.LogError("Tile map has been unset, not supposed to happen.");
            return null;
        }

        if (xPosition < 0 || yPosition < 0)
        {
            //Debug.LogError("Tile at : " + xPosition + "_" + yPosition + " is out of bounds.");
            return null;
        }

        if (xPosition > terrainSize.x || yPosition >= terrainSize.y)
        {
            //Debug.LogError("Tile at : " + xPosition + "_" + yPosition + " is out of bounds.");
            return null;
        }

        if (tiles[xPosition + terrainSize.x * yPosition] == null)
        {
            Debug.LogError("Wanted tile was empty, not supposed to happen.");
            return null;
        }

        else
        {
            //Debug.Log("Returned tile " + tiles[xPosition + terrainSize.x * yPosition].tilePosition);
        }

        return tiles[xPosition + terrainSize.x * yPosition];
    }

    public Vector3 GetTilePositionInScene(Tile tile)
    {
        return new Vector3(tile.tilePosition.x, 0.0f, tile.tilePosition.y);
    }

    public void CreateWaypointOnTile(Tile tile)
    {
        //tile.AddComponent<Waypoint>();
        //WaypointList.Add(tile.GetComponent<Waypoint>());
        ////tile.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("MapSpriteSheet")[3] as Sprite;

        Waypoint newWaypoint = new Waypoint();
        newWaypoint.linkedTile = tile;

        waypoints.Add(newWaypoint);
    }

    public Waypoint GetWaypointOnTile(Tile tile)
    {
        foreach(Waypoint waypoint in waypoints)
        {
            if (waypoint.linkedTile == tile)
            {
                Debug.Log(tile);
                return waypoint;
            }
        }

        return null;
    }

    public void RecalculateWaypoints()
    {
        foreach (Waypoint waypoint in waypoints)
        {
            waypoint.waypointType = Waypoint.WaypointTypes.enemySpawn;
        }

        foreach (Waypoint waypoint in waypoints)
        {
            waypoint.roads = new List<Waypoint.Road>();

            foreach (Waypoint destination in waypoint.destinations)
            {
                if (destination.destinations.Count > 0)
                {
                    destination.waypointType = Waypoint.WaypointTypes.waypoint;
                }

                else
                {
                    destination.waypointType = Waypoint.WaypointTypes.enemyExit;
                }

                RecalculateRoad(new Vector2(waypoint.linkedTile.tilePosition.x, waypoint.linkedTile.tilePosition.y), new Vector2(destination.linkedTile.tilePosition.x, destination.linkedTile.tilePosition.y), waypoint);
            }

            Debug.Log(waypoint.roads.Count);
        }
    }

    public void RecalculateRoad(Vector2 startingTilePosition, Vector2 endingTilePosition, Waypoint waypointToStock)
    {
        Vector2 road;
        road = new Vector2(endingTilePosition.x, endingTilePosition.y) - new Vector2(startingTilePosition.x, startingTilePosition.y);

        float roadSlope = road.x / road.y;

        Debug.Log(roadSlope);

        Waypoint.Road currentRoad = new Waypoint.Road();
        currentRoad.roadDestination = GetWaypointOnTile(GetTileAt((int)endingTilePosition.x, (int)endingTilePosition.y));

        if (Mathf.Abs(road.x) >= Mathf.Abs(road.y))
        {
            for (int i = 0; i < Mathf.Abs(road.x); i++)
            {
                Tile roadTile = GetTileAt((int)startingTilePosition.x + (int)(Mathf.Sign(road.x) * i), (int)startingTilePosition.y + (int)(Mathf.Sign(road.x) * Mathf.RoundToInt(i / roadSlope)));
                roadTile.tileType = Tile.TileType.road;

                roadTile.tileObject.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("MapSpriteSheet")[1] as Sprite;

                currentRoad.roadTiles.Add(roadTile);
            }
        }

        else
        {
            for (int i = 0; i < Mathf.Abs(road.y); i++)
            {
                Tile roadTile = GetTileAt((int)startingTilePosition.x + (int)(Mathf.Sign(road.y) * Mathf.RoundToInt(i * roadSlope)), (int)startingTilePosition.y + (int)(Mathf.Sign(road.y) * i));
                roadTile.tileType = Tile.TileType.road;

                roadTile.tileObject.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("MapSpriteSheet")[1] as Sprite;

                currentRoad.roadTiles.Add(roadTile);
            }
        }

        Tile endRoadTile = GetTileAt((int)endingTilePosition.x, (int)endingTilePosition.y);

        Tile startingTile = GetTileAt((int)startingTilePosition.x, (int)startingTilePosition.y);

        GetWaypointOnTile(startingTile).roads.Add(currentRoad);

        Debug.Log(GetWaypointOnTile(GetTileAt((int)startingTilePosition.x, (int)startingTilePosition.y)).roads.Count);
        endRoadTile.tileType = Tile.TileType.road;
        
        endRoadTile.tileObject.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("MapSpriteSheet")[1] as Sprite;

        CalculateRoadBorder(currentRoad);
    }

    public void CalculateRoadBorder(Waypoint.Road road)
    {
        foreach(Tile roadTile in road.roadTiles)
        {
            for (int yPointer = -1; yPointer <= 1; yPointer++)
            {
                for (int xPointer = -1; xPointer <= 1; xPointer++)
                {
                    Tile possibleBorderTile = GetTileAt(roadTile.tilePosition.x + xPointer, roadTile.tilePosition.y + yPointer);

                    if (possibleBorderTile != null)
                    {
                        if (possibleBorderTile.tileType == Tile.TileType.grass)
                        {
                            possibleBorderTile.tileType = Tile.TileType.roadborder;
                            possibleBorderTile.tileObject.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("MapSpriteSheet")[3] as Sprite;
                        }
                        //possibleBorderTile.tileObject.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("MapSpriteSheet")[3] as Sprite;
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        foreach(Waypoint waypointInstance in waypoints)
        {
            switch (waypointInstance.waypointType)
            {
                case Waypoint.WaypointTypes.enemySpawn:
                    Gizmos.color = Color.green;
                    break;

                case Waypoint.WaypointTypes.waypoint:
                    Gizmos.color = Color.yellow;
                    break;

                case Waypoint.WaypointTypes.enemyExit:
                    Gizmos.color = Color.red;
                    break;
            }
            Gizmos.DrawSphere(GetTilePositionInScene(waypointInstance.linkedTile), 0.4f);

            foreach (Waypoint destination in waypointInstance.destinations)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(GetTilePositionInScene(waypointInstance.linkedTile), GetTilePositionInScene(destination.linkedTile));
                Gizmos.DrawCube(GetTilePositionInScene(destination.linkedTile) - Vector3.Normalize(GetTilePositionInScene(destination.linkedTile) - GetTilePositionInScene(waypointInstance.linkedTile)) / 2.5f, Vector3.one / 5);
            }
        }
    }
}
