using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Entity Type", menuName = ProjectValues.devPrefix + "_" + ProjectValues.gameName + "/New Entity Type", order = 1)]

public class EntityDef : ScriptableObject
{
    // The ID of the entity definition to be referenced by other scripts.
    public int entityID;

    // The name that the entity will appear named as.
    public string entityName;

    // The mesh that the entity will appear as.
    public GameObject entityPrefab;

    // The max health that the entity will appear with.
    public int entityHealthPoints;
    // How much damage it deals to other entities.
    public int entityDamageToOtherEntities;

    // The speed the entity is moving at.
    public float moveSpeed;
    // How much damage it does to player health when reaching the end point.
    public int damageToPlayerHealth;

    // How much money it drops on kill.
    public int moneyOnKill;
}
