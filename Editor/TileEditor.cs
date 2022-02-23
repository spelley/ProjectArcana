using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Tile tile = (Tile) target;
        
        EditorGUILayout.Space(20f);
        EditorGUILayout.LabelField("Tile Modification", EditorStyles.boldLabel);

        MapBuilder mapBuilder = GameObject.Find("Map Builder").GetComponent<MapBuilder>();

        if(mapBuilder != null && GUILayout.Button("Update Tile"))
        {
            mapBuilder.BuildNeighbours();
            mapBuilder.AssignMapData();
            mapBuilder.ClearMap();
            mapBuilder.RenderGrid();
        }
    }
}