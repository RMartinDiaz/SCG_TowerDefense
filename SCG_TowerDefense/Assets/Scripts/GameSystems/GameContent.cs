using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContent
{
    public static BuildingDef[] loadedBuildings = new BuildingDef[0];

    public static void LoadBuildings()
    {
        BuildingDef[] unsortedBuildings = new BuildingDef[Resources.LoadAll("PlayerBuildings", typeof(BuildingDef)).Length];

        for (int i = 0; i < Resources.LoadAll("PlayerBuildings", typeof(BuildingDef)).Length; i++)
        {
            unsortedBuildings[i] = (BuildingDef)Resources.LoadAll("PlayerBuildings", typeof(BuildingDef))[i];
        }

        int lastBuildingID = 0;

        for (int i = 0; i < Resources.LoadAll("PlayerBuildings", typeof(BuildingDef)).Length; i++)
        {
            if (unsortedBuildings[i].buildingID > lastBuildingID)
            {
                lastBuildingID = unsortedBuildings[i].buildingID;
            }
        }

        loadedBuildings = new BuildingDef[lastBuildingID + 1];

        for (int i = 0; i < Resources.LoadAll("PlayerBuildings", typeof(BuildingDef)).Length; i++)
        {
            loadedBuildings[unsortedBuildings[i].buildingID] = unsortedBuildings[i];

            Debug.Log("Building " + unsortedBuildings[i].buildingID + " was loaded at " + unsortedBuildings[i].buildingID + " ID.");
        }
    }

    public static EntityDef[] loadedEnemyEntities = new EntityDef[0];

    public static void LoadEnemies()
    {
        EntityDef[] unsortedEnemies = new EntityDef[Resources.LoadAll("EnemyEntities", typeof(EntityDef)).Length];

        for (int i = 0; i < Resources.LoadAll("EnemyEntities", typeof(EntityDef)).Length; i++)
        {
            unsortedEnemies[i] = (EntityDef)Resources.LoadAll("EnemyEntities", typeof(EntityDef))[i];
        }

        int lastEnemyID = 0;

        for (int i = 0; i < Resources.LoadAll("EnemyEntities", typeof(EntityDef)).Length; i++)
        {
            if (unsortedEnemies[i].entityID > lastEnemyID)
            {
                lastEnemyID = unsortedEnemies[i].entityID;
            }
        }

        loadedEnemyEntities = new EntityDef[lastEnemyID + 1];

        for (int i = 0; i < Resources.LoadAll("EnemyEntities", typeof(EntityDef)).Length; i++)
        {
            loadedEnemyEntities[unsortedEnemies[i].entityID] = unsortedEnemies[i];

            Debug.Log("Enemy " + unsortedEnemies[i].entityID + " was loaded at " + unsortedEnemies[i].entityID + " ID.");
        }
    }
}
