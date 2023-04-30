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

        if (DrawDefaultInspector() && mapGen.autoUpdate)
        {
            if (mapGen.drawMode == MapGenerator.DrawMode.Mesh)
            {
                mapGen.UpdateMap();
            }
            else
            {
                mapGen.GenerateMap();
            }
        }


        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
        if (GUILayout.Button("Destroy"))
        {
            mapGen.DestroyMap();
        }
    }
}
