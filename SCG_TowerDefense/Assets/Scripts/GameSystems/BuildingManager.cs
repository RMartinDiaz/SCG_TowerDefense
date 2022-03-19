using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    // Stores all building built during the game, so it's easier to manage.
    public List<Building> buildings = new List<Building>();

    public GameObject pendingBuildingPrefab;

    public Coroutine updatingBuildingCoroutine;

    // A class that defines the building.
    [System.Serializable]
    public class Building
    {
        // Store the tile the building was built on for referencing.
        public Tile linkedTile;

        public bool isBuilt;
        public float timeToBuild;

        // The variable that will stock the linked game object.
        public GameObject buildingObject;

        // The building definition that holds the variable the building is based on.
        public BuildingDef buildingDefinition;

        // The constructor to create new building classes.
        public Building(Tile tileToBuild, BuildingDef buildingDef)
        {
            linkedTile = tileToBuild;

            isBuilt = false;
            timeToBuild = buildingDef.timeToBuildInSeconds;

            buildingObject = null;

            buildingDefinition = buildingDef;
        }
    }

    





    // Create a new building class and mesh and links all of it together before storing them.
    public void AddBuildingOnTile(Tile tile, BuildingDef building)
    {
        Building newBuilding = new Building(tile, building);

        newBuilding.buildingObject = Instantiate(pendingBuildingPrefab, tile.tileObject.transform);
        newBuilding.buildingObject.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        newBuilding.buildingObject.transform.localPosition = Vector3.zero;
        newBuilding.buildingObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        buildings.Add(newBuilding);
    }

    // Use this to get buildings on a specific tile.
    public Building GetBuildingOnTile(Tile tile)
    {
        foreach (Building building in buildings)
        {
            if (building.linkedTile == tile)
            {
                return building;
            }
        }

        return null;
    }

    public IEnumerator UpdateBuildingStates()
    {
        while (true)
        {
            yield return new WaitUntil(() => GameManager.gameManagerInstance.matchState == GameManager.MatchState.WavePhase);
            foreach (Building building in buildings)
            {
                if (!building.isBuilt)
                {
                    if (building.timeToBuild > 0)
                    {
                        Debug.Log(building.timeToBuild);
                        building.timeToBuild -= 0.2f;
                    }

                    else
                    {
                        Destroy(building.buildingObject);

                        building.buildingObject = Instantiate(building.buildingDefinition.buildingPrefab, building.linkedTile.tileObject.transform);
                        building.buildingObject.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                        building.buildingObject.transform.localPosition = Vector3.zero;
                        building.buildingObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                        building.isBuilt = true;
                    }
                }

                else
                {
                    building.buildingDefinition.BuildingBehavior(building.linkedTile);
                }
            }

            yield return new WaitForSeconds(0.2f);
        }

        yield break;
    }
}
