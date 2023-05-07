using Map.Coordinate;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(ChunkCoordinates))]
public class ChunkCoordEditor : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property,GUIContent label)
    {
        var coordinates = new ChunkCoordinates(
            property.FindPropertyRelative("_x").intValue,
            property.FindPropertyRelative("_z").intValue
            );

        position = EditorGUI.PrefixLabel(position,label);
        GUI.Label(position, coordinates.ToString());
    }
}
