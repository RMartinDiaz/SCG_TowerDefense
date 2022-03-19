#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TowerDefenseTerrain))]
[CanEditMultipleObjects]
public class TowerDefenseTerrainEditor : Editor
{
    TowerDefenseTerrain _target;
    SerializedProperty terrainSize;
    SerializedProperty tiles;
    SerializedProperty waypoints;

    bool isChangingWaypoints;

    Tile previouslyPlacedWaypoint;

    void OnEnable()
    {
        terrainSize = serializedObject.FindProperty("terrainSize");
        tiles = serializedObject.FindProperty("tiles");
        waypoints = serializedObject.FindProperty("waypoints");
        _target = (TowerDefenseTerrain)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Terrain Editor Tools");

        EditorGUILayout.Space(20);

        serializedObject.Update();
        EditorGUILayout.PropertyField(terrainSize);
        EditorGUILayout.PropertyField(tiles);
        EditorGUILayout.PropertyField(waypoints);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Tiles quantity : " + _target.tiles.Count);

        EditorGUILayout.Space(20);

        if (GUILayout.Button("Recreate map"))
        {
            _target.waypoints = new List<Waypoint>();

            foreach (Tile tile in _target.tiles)
            {
                DestroyImmediate(tile.tileObject);
            }

            _target.tiles = new List<Tile>();

            _target.CreateTileMap(_target.terrainSize.x, _target.terrainSize.y);
        }

        EditorGUILayout.Space(20);

        if (GUILayout.Button("Change map waypoints"))
        {
            isChangingWaypoints = !isChangingWaypoints;

            if (isChangingWaypoints)
            {
                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                Tools.current = Tool.None;
            }

            else
            {
                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Keyboard);
                Tools.current = Tool.Move;
            }
        }
    }

    void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            if (isChangingWaypoints)
            {
                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                Debug.Log("Hit!");

                Plane hPlane = new Plane(Vector3.up, Vector3.zero);

                float hitDistance;
                if (hPlane.Raycast(ray, out hitDistance))
                {
                    Debug.Log(ray.GetPoint(hitDistance));

                    TowerDefenseTerrain _target = (TowerDefenseTerrain)target;

                    Tile currentlyPlacedWaypoint = _target.GetTileAt((int)Mathf.Round(ray.GetPoint(hitDistance).x), (int)Mathf.Round(ray.GetPoint(hitDistance).z));

                    if (_target.GetWaypointOnTile(currentlyPlacedWaypoint) == null)
                    {
                        _target.CreateWaypointOnTile(currentlyPlacedWaypoint);
                    }

                    if (previouslyPlacedWaypoint != null)
                    {
                        _target.GetWaypointOnTile(previouslyPlacedWaypoint).destinations.Add(_target.GetWaypointOnTile(currentlyPlacedWaypoint));
                    }

                    _target.RecalculateWaypoints();
                    previouslyPlacedWaypoint = currentlyPlacedWaypoint;

                    EditorUtility.SetDirty(target);
                }
            }
        }
    }
}
#endif