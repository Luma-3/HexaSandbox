using Map.Coordinate;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(HexaCoordinates))]
public class HexCoordinatesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    { 
        HexaCoordinates coordinates = new (
            property.FindPropertyRelative("x").intValue, 
            property.FindPropertyRelative("z").intValue
            );

        position = EditorGUI.PrefixLabel(position, label);
        GUI.Label(position, coordinates.ToString());
    }
}

