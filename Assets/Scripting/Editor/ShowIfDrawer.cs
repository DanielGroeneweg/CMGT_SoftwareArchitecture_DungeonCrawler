#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return ShouldShow(property)
            ? EditorGUI.GetPropertyHeight(property, label, true)
            : 0f;
    }

    private bool ShouldShow(SerializedProperty property)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;

        // Get relative path (handles nested classes & arrays)
        string fullPath = property.propertyPath;
        string conditionPath = fullPath.Replace(property.name, showIf.fieldName);
        SerializedProperty conditionProp = property.serializedObject.FindProperty(conditionPath);

        if (conditionProp == null)
        {
            Debug.LogWarning($"[ShowIf] Could not find field '{showIf.fieldName}' for {property.name}");
            return true; // default to visible
        }

        switch (conditionProp.propertyType)
        {
            case SerializedPropertyType.Boolean:
                return Compare(conditionProp.boolValue, showIf);
            case SerializedPropertyType.Enum:
                string currentEnum = conditionProp.enumNames[conditionProp.enumValueIndex];
                if (showIf.compareValue is string strValue)
                    return string.Equals(currentEnum, strValue, StringComparison.OrdinalIgnoreCase);
                if (showIf.compareValue is Enum enumValue)
                    return currentEnum.Equals(enumValue.ToString(), StringComparison.OrdinalIgnoreCase);
                return conditionProp.enumValueIndex.Equals(Convert.ToInt32(showIf.compareValue));
            case SerializedPropertyType.Integer:
                return Compare(conditionProp.intValue, showIf);
            case SerializedPropertyType.Float:
                return Compare(conditionProp.floatValue, showIf);
            case SerializedPropertyType.String:
                return Compare(conditionProp.stringValue, showIf);
            default:
                return true;
        }
    }

    private bool Compare<T>(T actual, ShowIfAttribute showIf) where T : IComparable
    {
        object compareObj = showIf.compareValue;
        if (compareObj is Enum) compareObj = Convert.ToInt32(compareObj);

        T expected = (T)Convert.ChangeType(compareObj, typeof(T));
        int cmp = actual.CompareTo(expected);

        switch (showIf.comparison)
        {
            case ComparisonTypes.Equals: return cmp == 0;
            case ComparisonTypes.NotEqual: return cmp != 0;
            case ComparisonTypes.Greater: return cmp > 0;
            case ComparisonTypes.Less: return cmp < 0;
            case ComparisonTypes.GreaterOrEqual: return cmp >= 0;
            case ComparisonTypes.LessOrEqual: return cmp <= 0;
            default: return false;
        }
    }
}
#endif