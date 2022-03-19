using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum PlayerState
    {
        selection,
        building,
        removeBuilding
    }

    public PlayerState playerState;

    // Defines if the player can build on this tile.
    bool canBuild;
    public Tile tileUnderMouse;
    public GameObject buildingPreviewGO;

    public int currentBuildingToBuild;

    private void Awake()
    {
        SettingPlayerState(PlayerState.building);
    }

    public void SettingPlayerState(int newPlayerState)
    {
        playerState = (PlayerState)newPlayerState;

        if ((PlayerState)newPlayerState == PlayerState.building)
        {
            buildingPreviewGO.SetActive(true);
        }

        else
        {
            buildingPreviewGO.SetActive(false);
        }
    }

    public void SettingPlayerState(PlayerState newPlayerState)
    {
        playerState = newPlayerState;

        if (newPlayerState == PlayerState.building)
        {
            buildingPreviewGO.SetActive(true);
        }

        else 
        { 
            buildingPreviewGO.SetActive(false); 
        }
    }

    public void SettingBuildingType(int buildingID)
    {
        currentBuildingToBuild = buildingID;
    }

    private void Update()
    {
        switch (playerState)
        {
            case PlayerState.building :

                if (Input.GetMouseButtonDown(0))
                {
                    if (canBuild)
                    {
                        BuildingDef currentBuildingSelected = GameContent.loadedBuildings[currentBuildingToBuild];

                        if (GameManager.gameManagerInstance.playerValues.playerMoney >= currentBuildingSelected.buildingCost)
                        {
                            GameManager.gameManagerInstance.buildingManagerInstance.AddBuildingOnTile(tileUnderMouse, currentBuildingSelected);
                            GameManager.gameManagerInstance.AddMoneyToPlayer(-currentBuildingSelected.buildingCost);
                        }
                    }
                }

                //else if (Input.GetMouseButtonDown(1))
                //{
                //    SettingPlayerState(PlayerState.selection);
                //}

                break;
        }
    }

    private void FixedUpdate()
    {
        switch (playerState)
        {
            case PlayerState.building :

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                Plane hPlane = new Plane(Vector3.up, Vector3.zero);

                float hitDistance;
                if (hPlane.Raycast(ray, out hitDistance))
                {
                    tileUnderMouse = GameManager.gameManagerInstance.tdTerrain.GetTileAt(Mathf.RoundToInt(ray.GetPoint(hitDistance).x), Mathf.RoundToInt(ray.GetPoint(hitDistance).z));

                    if (tileUnderMouse != null)
                    {
                        buildingPreviewGO.transform.position = GameManager.gameManagerInstance.tdTerrain.GetTilePositionInScene(tileUnderMouse);

                        if (tileUnderMouse.tileType == Tile.TileType.roadborder && GameManager.gameManagerInstance.buildingManagerInstance.GetBuildingOnTile(tileUnderMouse) == null)
                        {
                            buildingPreviewGO.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                            canBuild = true;
                        }

                        else
                        {
                            buildingPreviewGO.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                            canBuild = false;
                        }
                    }
                }

                break;
        }
    }
}
