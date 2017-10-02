using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Stat))]
public class StatPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        List<Stat> stats = Resources.FindObjectsOfTypeAll<Stat>().ToList();

        if (stats.Count > 0)
        {
            if (!stats.Contains((Stat)property.objectReferenceValue))
            {
                property.objectReferenceValue = stats[0];
            }
            property.objectReferenceValue = stats[EditorGUI.Popup(position, stats.IndexOf((Stat)property.objectReferenceValue), stats.Select(x => x.Name).ToArray())];
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}