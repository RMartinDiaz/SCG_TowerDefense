using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public List<EntityGroup> entityGroups = new List<EntityGroup>();

    public Coroutine updatingGroupCoroutine;

    [System.Serializable]
    public class EntityGroup
    {
        public Tile currentTile;

        public List<Entity> entitiesInGroup = new List<Entity>();
        public List<GameObject> entitiesGameObjects = new List<GameObject>();

        public void AddObjectToGroup(Entity entity)
        {
            entitiesInGroup.Add(entity);
            entitiesGameObjects.Add(entity.entityObject);
            entity.currentGroup = this;

            UpdateGroupObjects();
        }

        public void RemoveObjectFromGroup(Entity entity)
        {
            entitiesGameObjects.Remove(entity.entityObject);
            entitiesInGroup.Remove(entity);
            
            //entity.currentGroup = null;

            UpdateGroupObjects();
        }

        public void MoveEntityToNewTile(Entity entity, Tile tile)
        {
            if (tile != currentTile)
            {
                if (GameManager.gameManagerInstance.entityManagerInstance.GetEntityGroupOnTile(GameManager.gameManagerInstance.tdTerrain.GetTileAt(tile.tilePosition.x, tile.tilePosition.y)) != null) 
                {
                    //Debug.Log("Group found on tile " + tile.tilePosition);

                    GameManager.gameManagerInstance.entityManagerInstance.GetEntityGroupOnTile(tile).AddObjectToGroup(entity);
                }

                else if (GameManager.gameManagerInstance.entityManagerInstance.GetEntityGroupOnTile(tile) == null)
                {
                    //Debug.Log("No group found on tile " + tile.tilePosition);
                    EntityGroup newEntityGroup = GameManager.gameManagerInstance.entityManagerInstance.CreateNewEntityGroup(tile);
                    newEntityGroup.AddObjectToGroup(entity);
                }

                else
                {
                    //Debug.Log("Group found on tile " + tile.tilePosition);

                    GameManager.gameManagerInstance.entityManagerInstance.GetEntityGroupOnTile(tile).AddObjectToGroup(entity);
                }

                RemoveObjectFromGroup(entity);
            }
        }

        public void KillEntity(Entity entity)
        {
            GameManager.gameManagerInstance.AddMoneyToPlayer(entity.moneyOnKill);

            Destroy(entity.entityObject);

            entitiesGameObjects.Remove(entity.entityObject);
            entitiesInGroup.Remove(entity);

            UpdateGroupObjects();
        }

        public void EndEntity(Entity entity)
        {
            GameManager.gameManagerInstance.AddHPToPlayer(-entity.playerDamage);

            Destroy(entity.entityObject);

            entitiesGameObjects.Remove(entity.entityObject);
            entitiesInGroup.Remove(entity);

            UpdateGroupObjects();
        }

        public void UpdateEntitiesBehavior()
        {
            List<Entity> entitiesToMove = new List<Entity>();
            List<Entity> entitiesToKill = new List<Entity>();
            List<Entity> entitiesAtEnd = new List<Entity>();

            foreach (Entity entity in entitiesInGroup)
            {
                switch (entity.currentState)
                {
                    case Entity.EntityState.moving :
                        
                        if (entity.timeBeforeMoving >= 1/entity.entityMoveSpeed)
                        {
                            entitiesToMove.Add(entity);
                        }

                        else
                        {
                            entity.timeBeforeMoving += 0.2f;
                        }
                        break;

                    case Entity.EntityState.die :
                        if (!entitiesToKill.Contains(entity))
                        {
                            Debug.Log("Added entity to kill.");
                            entitiesToKill.Add(entity);
                        }

                        break;

                    case Entity.EntityState.reachedEnd:
                        if (!entitiesAtEnd.Contains(entity))
                        {
                            Debug.Log("Added entity to end.");
                            entitiesAtEnd.Add(entity);
                        }

                        break;
                }
            }

            foreach(Entity entityToMove in entitiesToMove)
            {
                MoveEntityToNewTile(entityToMove, entityToMove.destination);
                entityToMove.OnDestinationReached();
            }

            foreach (Entity entityToKill in entitiesToKill)
            {
                KillEntity(entityToKill);
            }

            foreach(Entity entityAtEnd in entitiesAtEnd)
            {
                EndEntity(entityAtEnd);
            }
        }

        public void UpdateGroupObjects()
        {
            if (entitiesInGroup.Count == 1)
            {
                entitiesInGroup[0].entityObject.transform.position = currentTile.tileObject.transform.position;
            }

            else if (entitiesInGroup.Count > 1)
            {
                for (int i = 0; i < entitiesInGroup.Count; i++)
                {
                    //Debug.Log(Quaternion.Euler(new Vector3(0.0f, 360f / (i + 1), 0.0f)) * Vector3.forward);
                    entitiesInGroup[i].entityObject.transform.position = currentTile.tileObject.transform.position + Quaternion.Euler(new Vector3(0.0f, 360f / entitiesInGroup.Count * ( i + 1), 0.0f)) * Vector3.forward * 0.2f;
                }
            }
        }
    }

    [System.Serializable]
    public class Entity
    {
        public string entityName;

        public GameObject entityObject;

        public EntityGroup currentGroup;

        public Tile destination;

        public float entityMoveSpeed = 0.5f;
        public float timeBeforeMoving = 0;

        public int playerDamage = 1;
        public int moneyOnKill = 1;

        public enum EntityTeam
        {
            player,
            enemy
        }

        public EntityTeam currentTeam;

        public enum EntityState
        {
            idle,
            moving,
            die,
            reachedEnd
        }

        public EntityState currentState;

        public virtual void SetDestination(Tile tile)
        {
            timeBeforeMoving = 0;

            currentState = EntityState.moving;

            destination = tile;
        }

        public virtual void OnDestinationReached()
        {
            currentState = EntityState.idle;
            timeBeforeMoving = 0;
        }
    }

    [System.Serializable]
    public class EnemyEntity : Entity
    {
        Waypoint currentWaypoint;
        Waypoint destinationWaypoint;

        Waypoint.Road currentRoad;
        public int tileFromRoadStart;

        
        public EnemyEntity(Waypoint startingWaypoint, EntityDef entityDef)
        {
            entityMoveSpeed = entityDef.moveSpeed;
            currentTeam = EntityTeam.enemy;

            playerDamage = entityDef.damageToPlayerHealth;
            moneyOnKill = entityDef.moneyOnKill;

            currentWaypoint = startingWaypoint;
            SetDestinationWaypoint(currentWaypoint.destinations[Random.Range(0, startingWaypoint.destinations.Count)]);
        }

        public void SetDestinationWaypoint(Waypoint newDestinationWaypoint)
        {
            destinationWaypoint = newDestinationWaypoint;

            tileFromRoadStart = 0;
            Debug.Log(newDestinationWaypoint.linkedTile.tilePosition);

            Debug.Log(currentWaypoint.linkedTile.tilePosition);

            currentRoad = currentWaypoint.GetRoadToWaypoint(destinationWaypoint);

            SetDestination(currentRoad.roadTiles[tileFromRoadStart]);
        }

        public override void SetDestination(Tile tile)
        {
            timeBeforeMoving = 0;

            currentState = EntityState.moving;

            destination = currentRoad.roadTiles[tileFromRoadStart];
        }

        public override void OnDestinationReached()
        {
            
            if (tileFromRoadStart < currentRoad.roadTiles.Count)
            {
                SetDestination(currentRoad.roadTiles[tileFromRoadStart]);
                tileFromRoadStart += 1;
            }

            else if (tileFromRoadStart == currentRoad.roadTiles.Count && destinationWaypoint.waypointType != Waypoint.WaypointTypes.enemyExit)
            {
                tileFromRoadStart = 0;
                currentWaypoint = destinationWaypoint;

                Debug.Log(currentWaypoint.destinations.Count);
                int choosenDestination = Random.Range(0, currentWaypoint.destinations.Count);
                Debug.Log(choosenDestination);
                SetDestinationWaypoint(currentWaypoint.destinations[choosenDestination]);
            }

            else if (tileFromRoadStart == currentRoad.roadTiles.Count && destinationWaypoint.waypointType == Waypoint.WaypointTypes.enemyExit)
            {
                currentState = EntityState.reachedEnd;
            }

            else
            {
                currentState = EntityState.idle;
                timeBeforeMoving = 0;
            }
        }
    }

    public class PlayerEntity : Entity
    {

    }

    public Entity CreateEnemyOnTile(Waypoint waypoint, EntityDef entityDef, Tile tile)
    {
        EnemyEntity newEnemyEntity = new EnemyEntity(waypoint, entityDef);
        newEnemyEntity.entityName = entityDef.entityName;
        newEnemyEntity.entityObject = Instantiate(entityDef.entityPrefab);

        if (GetEntityGroupOnTile(tile) == null)
        {
            EntityGroup newEntityGroup = CreateNewEntityGroup(tile);
            newEntityGroup.AddObjectToGroup(newEnemyEntity);
        }

        else
        {
            GetEntityGroupOnTile(tile).AddObjectToGroup(newEnemyEntity);
        }

        return newEnemyEntity;
    }

    public Entity CreateEntityOnTile(EntityDef entityDef, Tile tile)
    {
        Entity newEntity = new Entity();

        newEntity.entityName = entityDef.entityName;
        newEntity.entityObject = Instantiate(entityDef.entityPrefab);

        if (GetEntityGroupOnTile(tile) == null)
        {
            EntityGroup newEntityGroup = CreateNewEntityGroup(tile);
            newEntityGroup.AddObjectToGroup(newEntity);
        }

        else
        {
            GetEntityGroupOnTile(tile).AddObjectToGroup(newEntity);
        }

        return newEntity;
    }

    public EntityGroup CreateNewEntityGroup(Tile tile)
    {
        EntityGroup newEntityGroup = new EntityGroup();
        newEntityGroup.currentTile = tile;
        entityGroups.Add(newEntityGroup);


        return newEntityGroup;
    }

    public EntityGroup GetEntityGroupOnTile(Tile tile)
    {
        //Debug.Log("Checking tile at " + tile.tilePosition);
        foreach(EntityGroup entityGroup in entityGroups)
        {
            if (entityGroup.currentTile == tile)
            {
                return entityGroup;
            }
        }

        return null;
    }

    public IEnumerator UpdateGroupStates()
    {
        while (true)
        {
            yield return new WaitUntil(() => GameManager.gameManagerInstance.matchState == GameManager.MatchState.WavePhase);
            
            List<EntityGroup> entityGroupsAtUpdate = new List<EntityGroup>();

            foreach(EntityGroup currentEntityGroup in entityGroups)
            {
                entityGroupsAtUpdate.Add(currentEntityGroup);
            }

            foreach (EntityGroup entityGroup in entityGroupsAtUpdate)
            {
                entityGroup.UpdateEntitiesBehavior();
            }
            yield return new WaitForSeconds(0.2f);

            List<EntityGroup> groupsToRemove = new List<EntityGroup>();

            foreach (EntityGroup entityGroup in entityGroupsAtUpdate)
            {
                if (entityGroup.entitiesInGroup.Count <= 0)
                {
                    groupsToRemove.Add(entityGroup);
                }
            }

            foreach(EntityGroup groupToRemove in groupsToRemove)
            {
                entityGroups.Remove(groupToRemove);
            }
        }

        yield break;
    }
}
