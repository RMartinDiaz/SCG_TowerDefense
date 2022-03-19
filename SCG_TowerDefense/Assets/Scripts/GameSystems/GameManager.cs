using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Stores itself so it's easier to reference other scripts in game.
    public static GameManager gameManagerInstance { get; private set; }

    // Stores the building manager so you can call it easily.
    public BuildingManager buildingManagerInstance { get; private set; }

    // Stores the entity manager so you can call it easily.
    public EntityManager entityManagerInstance { get; private set; }

    // Stores the UI manager so you can call it easily.
    public UIManager uiManagerInstance { get; private set; }

    // Stores the map the game will be played on.
    public TowerDefenseTerrain tdTerrain;
    public List<Waypoint> spawnWaypoints = new List<Waypoint>();

    //THE STARTING VALUES HAS TO BE DEFINED BY MAPS GAME MODE.
    public static int playerStartingMoney = 300;
    public static int playerMaxMoney = 500;

    public static int playerStartingHealth = 80;
    public static int playerMaxHealth = 80;

    public static int defaultMaxWave = 10;


    // Class that has all player variables like health and money.
    public class PlayerValues
    {
        public int playerMoney;
        public int playerHealthPoints;

        public int currentWave;
        public int maxWave;

        public PlayerValues()
        {
            playerMoney = playerStartingMoney;
            playerHealthPoints = playerStartingHealth;

            currentWave = 0;
            maxWave = defaultMaxWave;
        }
    }

    public PlayerValues playerValues;

    public enum MatchState
    {
        PreparationPhase,
        WavePhase
    }

    public MatchState matchState;
    public Button startWaveButton;

    // Sets it all up on awake so it doesn't return null references (hopefully).
    private void Awake()
    {
        gameManagerInstance = this;

        GameContent.LoadBuildings();
        GameContent.LoadEnemies();

        buildingManagerInstance = this.GetComponent<BuildingManager>();
        buildingManagerInstance.updatingBuildingCoroutine = StartCoroutine(buildingManagerInstance.UpdateBuildingStates());

        entityManagerInstance = this.GetComponent<EntityManager>();
        entityManagerInstance.updatingGroupCoroutine = StartCoroutine(entityManagerInstance.UpdateGroupStates());

        uiManagerInstance = this.GetComponent<UIManager>();

        tdTerrain = FindObjectOfType<TowerDefenseTerrain>();
        foreach(Waypoint waypoint in tdTerrain.waypoints)
        {
            if (waypoint.waypointType == Waypoint.WaypointTypes.enemySpawn)
            {
                spawnWaypoints.Add(waypoint);
            }
        }

        playerValues = new PlayerValues();

        AddHPToPlayer(0);
        AddMoneyToPlayer(0);

        StartCoroutine(MoneyPerSecond());
    }

    public void AddHPToPlayer(int hp)
    {
        playerValues.playerHealthPoints += hp;

        if (playerValues.playerHealthPoints > playerMaxHealth)
        {
            playerValues.playerHealthPoints = playerMaxHealth;
        }

        if (playerValues.playerHealthPoints > playerMaxHealth)
        {
            playerValues.playerHealthPoints = playerMaxHealth;
        }

        uiManagerInstance.SetHPText(playerValues.playerHealthPoints);

        //Debug.Log(hp + " was added to player health, now player health is at " + playerValues.playerHealthPoints);
    }

    // Use this to add money to the player.
    // Can be negative to remove money from the player.
    public void AddMoneyToPlayer(int money)
    {
        playerValues.playerMoney += money;

        if (playerValues.playerMoney > playerMaxMoney)
        {
            playerValues.playerMoney = playerMaxMoney;
        }

        if (playerValues.playerMoney < 0)
        {
            playerValues.playerMoney = 0;
        }

        uiManagerInstance.SetMoneyText(playerValues.playerMoney);

        //Debug.Log(money + " was added to player money bank, now player money bank is at " + playerValues.playerMoney);
    }


    public void StartNewWave()
    {
        if (matchState == MatchState.PreparationPhase)
        {
            
            EntityManager.Entity testEntity = entityManagerInstance.CreateEnemyOnTile(spawnWaypoints[0], GameContent.loadedEnemyEntities[0], spawnWaypoints[0].linkedTile);
            testEntity = entityManagerInstance.CreateEnemyOnTile(spawnWaypoints[0], GameContent.loadedEnemyEntities[0], spawnWaypoints[0].linkedTile);
            testEntity = entityManagerInstance.CreateEnemyOnTile(spawnWaypoints[0], GameContent.loadedEnemyEntities[0], spawnWaypoints[0].linkedTile);

            testEntity = entityManagerInstance.CreateEnemyOnTile(spawnWaypoints[1], GameContent.loadedEnemyEntities[0], spawnWaypoints[1].linkedTile);
            testEntity = entityManagerInstance.CreateEnemyOnTile(spawnWaypoints[1], GameContent.loadedEnemyEntities[0], spawnWaypoints[1].linkedTile);
            testEntity = entityManagerInstance.CreateEnemyOnTile(spawnWaypoints[1], GameContent.loadedEnemyEntities[0], spawnWaypoints[1].linkedTile);

            //not a test
            //Sets the matchstate to wave phase
            matchState = MatchState.WavePhase;

            //Sets the matchstate to wave phase
            startWaveButton.interactable = false;

            playerValues.currentWave += 1;
            uiManagerInstance.SetWaveText(playerValues.currentWave);
        }
    }

    public void StartPreparationPhase()
    {
        if (matchState == MatchState.WavePhase)
        {
            matchState = MatchState.PreparationPhase;
            startWaveButton.interactable = true;
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator MoneyPerSecond()
    {
        while (true)
        {
            yield return new WaitUntil(() => this.matchState == GameManager.MatchState.WavePhase);

            AddMoneyToPlayer(1);

            yield return new WaitForSeconds(0.5f);
        }

        yield break;
    }
}
