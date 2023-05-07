using Map.Coordinate;
using Script.Map;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(HexaCoordinates))]
public class HexCoordinatesDrawer : PropertyDrawer
{
    public override void OnGUI(
        Rect position, SerializedProperty property, GUIContent label
    )
    {
        HexaCoordinates coordinates = new (
            property.FindPropertyRelative("x").intValue,
            property.FindPropertyRelative("z").intValue
        );

        position = EditorGUI.PrefixLabel(position, label);
        GUI.Label(position, coordinates.ToString());
    }
}
