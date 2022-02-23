using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[CustomEditor(typeof(MapBuilder))]
public class MapBuilderEditor : Editor
{
    static MapBuilder mapBuilder;
    static GameObject prefab;
    static int prefabTotal = 0;
    static Bounds spawnerBounds = new Bounds();
    static bool showArea = false;
    static int selectedLayer = 0;
    static bool snapToGrid = false;

    static List<GameObject> lastPlacedPrefabs = new List<GameObject>();
    static List<GridCell> cellList = new List<GridCell>();

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        mapBuilder = (MapBuilder) target;
        EditorGUILayout.LabelField("Map Builder", EditorStyles.boldLabel);

        if(GUILayout.Button("Build Map"))
        {
            bool decision = EditorUtility.DisplayDialog(
                "Build Map", // title
                "Are you sure want to build this map? This will override any manual changes.", // description
                "Yes", // OK button
                "No" // Cancel button
            );

            if(decision)
            {
                mapBuilder.BuildMap();
            } 
        }
        if(GUILayout.Button("Clear Tiles"))
        {
            mapBuilder.ClearMap();
        }
        if(GUILayout.Button("Refresh Map"))
        {
            mapBuilder.RefreshMapFromData();
        }

        EditorGUILayout.Space(20f);
        EditorGUILayout.LabelField("Object Spawner", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Prefab");
        prefab = (GameObject) EditorGUILayout.ObjectField(prefab, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        selectedLayer = EditorGUILayout.LayerField("Layer for Objects", selectedLayer);
        prefabTotal = EditorGUILayout.IntField("Number of Objects", prefabTotal);
        spawnerBounds = EditorGUILayout.BoundsField("Spawner Bounds", spawnerBounds);
        snapToGrid = EditorGUILayout.Toggle("Snap To Grid", snapToGrid);
        showArea = EditorGUILayout.Toggle("Show Bounds", showArea);

        EditorGUI.BeginDisabledGroup(prefab == null || spawnerBounds.extents == Vector3.zero || selectedLayer == 0 || prefabTotal == 0);
        if(GUILayout.Button("Place Prefabs"))
        {
            bool decision = EditorUtility.DisplayDialog(
                "Place Prefabs", // title
                "Are you sure want to place these prefabs?", // description
                "Yes", // OK button
                "No" // Cancel button
            );

            if(decision)
            {
                cellList.Clear();
                lastPlacedPrefabs.Clear();

                foreach(GridCell gridCell in mapBuilder.mapData.gridData)
                {
                    cellList.Add(gridCell);
                }

                int i = 0;
                int loopCounter = 0;
                while(i < prefabTotal && i < ((loopCounter + 1) * 5) && loopCounter < 1000)
                {
                    loopCounter++;
                    Vector3 point = RandomPointInBounds(spawnerBounds, true);
                    int layerMask = 1 << selectedLayer;
                    RaycastHit hit;
                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(point, -Vector3.up, out hit, Mathf.Infinity))
                    {
                        if(hit.transform.gameObject.layer == selectedLayer)
                        {
                            lastPlacedPrefabs.Add(GameObject.Instantiate(prefab, hit.point, Quaternion.identity));
                            i++;
                        }
                    }
                }
            } 
        }
        EditorGUI.EndDisabledGroup();
        EditorGUI.BeginDisabledGroup(lastPlacedPrefabs.Count == 0);
        if(GUILayout.Button("Undo Last Prefabs"))
        {
            bool decision = EditorUtility.DisplayDialog(
                "Undo Prefabs", // title
                "Are you sure want to remove the last set of prefabs you placed? This action cannot be undone.", // description
                "Yes", // OK button
                "No" // Cancel button
            );

            if(decision)
            {
                foreach(GameObject placedPrefab in lastPlacedPrefabs)
                {
                    GameObject.DestroyImmediate(placedPrefab);
                }
                lastPlacedPrefabs.Clear();
            } 
        }
        EditorGUI.EndDisabledGroup();
    }

    void OnSceneGUI()
    {
        if(showArea)
        {
            // Draw a yellow sphere at the transform's position
            Handles.color = Color.yellow;
            Handles.DrawWireCube(spawnerBounds.center, spawnerBounds.extents * 2);
        }
    }

    Vector3 RandomPointInBounds(Bounds bounds, bool maxY = false) {
        Vector3 randomPoint = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            100,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        if(snapToGrid && cellList.Count > 0) {
            GridCell gridCell = GetClosestGridCell(randomPoint);
            if(gridCell != null)
            {
                Vector3 snapPosition = gridCell.realWorldPosition;
                snapPosition.y = 100;
                return snapPosition;
            }
        }

        return randomPoint;
    }

    public GridCell GetClosestGridCell(Vector3 targetPosition, bool walkable = false)
    {
        if(walkable)
        {
            GridCell closestCell = cellList.Where(gc => gc.walkable).OrderBy(gc => Vector3.Distance(gc.realWorldPosition, targetPosition)).FirstOrDefault();
            return closestCell;
        }
        else
        {
            GridCell closestCell = cellList.OrderBy(gc => Vector3.Distance(gc.realWorldPosition, targetPosition)).FirstOrDefault();
            return closestCell;
        }
    }
}