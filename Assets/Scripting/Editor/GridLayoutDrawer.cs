#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GridLayout))]
public class GridLayoutDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Indent
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Get child properties
        SerializedProperty columnsProp = property.FindPropertyRelative("rows");
        SerializedProperty rowsProp = property.FindPropertyRelative("columns");

        // Calculate rects
        float fullWidth = position.width;
        float fieldWidth = (fullWidth - 20) / 2f; // leave space for 'x'

        Rect columnsRect = new Rect(position.x, position.y, fieldWidth, position.height);
        Rect xLabelRect = new Rect(columnsRect.xMax + 5, position.y, 10, position.height);
        Rect rowsRect = new Rect(xLabelRect.xMax + 5, position.y, fieldWidth, position.height);

        // Draw fields (columns first)
        EditorGUI.PropertyField(columnsRect, columnsProp, GUIContent.none);
        EditorGUI.LabelField(xLabelRect, "x");
        EditorGUI.PropertyField(rowsRect, rowsProp, GUIContent.none);

        EditorGUI.EndProperty();
    }
}
#endif