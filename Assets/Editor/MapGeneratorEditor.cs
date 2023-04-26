using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        DrawDefaultInspector();


        if (GUILayout.Button("Generate"))
        {

            mapGen.GenerateMap();
        }
        else if (GUILayout.Button("Update"))
        {
            mapGen.Generate2nd();
        }
    }
}
