using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SaveDataLoader))]
public class SaveDataLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SaveDataLoader saveDataLoader = (SaveDataLoader) target;
        
        EditorGUILayout.Space(20f);
        EditorGUILayout.LabelField("Save Data Loader", EditorStyles.boldLabel);

        if(GUILayout.Button("Save Default Data"))
        {
            saveDataLoader.SaveDefaultData();
        }
        if(GUILayout.Button("Load Default Data"))
        {
            saveDataLoader.LoadDefaultData();
        }
    }
}