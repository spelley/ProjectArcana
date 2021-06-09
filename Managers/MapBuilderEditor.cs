using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapBuilder))]
public class MapBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapBuilder mapBuilder = (MapBuilder) target;
        
        EditorGUILayout.Space(20f);
        EditorGUILayout.LabelField("Map Builder", EditorStyles.boldLabel);

        if(GUILayout.Button("Build Map"))
        {
            mapBuilder.BuildMap();
        }
        if(GUILayout.Button("Clear Tiles"))
        {
            mapBuilder.ClearMap();
        }
        if(GUILayout.Button("Refresh Map"))
        {
            mapBuilder.RefreshMapFromData();
        }
    }
}