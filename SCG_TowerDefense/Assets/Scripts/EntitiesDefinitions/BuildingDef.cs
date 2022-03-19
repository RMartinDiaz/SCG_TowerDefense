using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Building Type", menuName = ProjectValues.devPrefix + "_" + ProjectValues.gameName + "/New Building Type", order = 1)]
public class BuildingDef : ScriptableObject
{
    // The ID of the building definition to be referenced by other scripts.
    public int buildingID;

    // The mesh that the building will appear as.
    public GameObject buildingPrefab;

    // The cost of the building.
    public int buildingCost;
    public float timeToBuildInSeconds;

    public float timeToActivateBehaviorInSeconds;
    public int buildingRange;

    // The type of building, not really the best but functionnal.
    public enum BuildingType
    {
        ranged,
        spawner
    }

    public BuildingType buildingType;

    // Which behavior the building has, depends on its type.
    // Again, some things could be better for developpement but time constraints.
    public void BuildingBehavior(Tile tile)
    {
        if (buildingType == BuildingType.ranged)
        {
            foreach(EntityManager.EntityGroup entityGroup in GameManager.gameManagerInstance.entityManagerInstance.entityGroups)
            {
                if (entityGroup.currentTile.tilePosition.x > tile.tilePosition.x - buildingRange && entityGroup.currentTile.tilePosition.x < tile.tilePosition.x + buildingRange)
                {
                    if (entityGroup.currentTile.tilePosition.y > tile.tilePosition.y - buildingRange && entityGroup.currentTile.tilePosition.y < tile.tilePosition.y + buildingRange)
                    {
                        foreach(EntityManager.Entity entity in entityGroup.entitiesInGroup)
                        {
                            Debug.Log("Group at tile " + entityGroup.currentTile.tilePosition + "Killed by tower at " + tile.tilePosition + " of range " + buildingRange);
                            entity.currentState = EntityManager.Entity.EntityState.die;
                        }
                    }
                }
            }
        }
    }

    IEnumerator SpawnerBehavior()
    {
        

        yield break;
    }

    
}
