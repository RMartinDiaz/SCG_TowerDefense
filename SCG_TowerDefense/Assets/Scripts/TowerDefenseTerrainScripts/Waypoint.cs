using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint
{
    public Tile linkedTile;

    public enum WaypointTypes
    {
        enemySpawn,
        waypoint,
        enemyExit
    }

    public WaypointTypes waypointType;

    public List<Waypoint> destinations = new List<Waypoint>();

    [System.Serializable]
    public class Road
    {
        public Waypoint roadDestination;

        public List<Tile> roadTiles = new List<Tile>();
    }

    public List<Road> roads = new List<Road>();

    public Road GetRoadToWaypoint(Waypoint destination)
    {
        if (destination == this)
        {
            return new Road();
        }

        if (!destinations.Contains(destination))
        {
            Debug.LogError("Current Waypoint doesn't have a defined road to destination waypoint.");
            return null;
        }

        else
        {
            if (roads.Count == 0)
            {
                Debug.LogError("Road wasn't set. Not supposed to happen.");
            }

            foreach (Road road in roads)
            {
                Road foundRoad = null;
                if (road.roadDestination == destination)
                {
                    Debug.Log("Road was found.");
                    foundRoad = road;

                    return road;
                }

                else if (GameManager.gameManagerInstance.tdTerrain.GetWaypointOnTile(road.roadDestination.linkedTile) == GameManager.gameManagerInstance.tdTerrain.GetWaypointOnTile(destination.linkedTile))
                {
                    Debug.Log("Road was found.");
                    foundRoad = road;

                    return road;
                }
            }

            Debug.LogError("Current Waypoint doesn't have a defined road to destination waypoint.");
            return null;
        }
    }
}
