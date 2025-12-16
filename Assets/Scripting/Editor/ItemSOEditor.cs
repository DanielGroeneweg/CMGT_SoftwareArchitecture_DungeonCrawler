#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Item), true)]
public class ItemSOEditor : Editor
{
    // This completely disables UI Toolkit usage for this inspector
    public override VisualElement CreateInspectorGUI()
    {
        return null; // Return null means "don't use UITK"
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var item = target as Item;
        if (item == null)
            return;

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField("Item ID", item.ItemID ?? "(null)");
        EditorGUI.EndDisabledGroup();
    }
}
#endif