using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ModificatorCondition))]
public class ModificatorConditionPropertyDrawer : PropertyDrawer
{
    private ModificatorCondition condition;

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

        // Calculate rects
        Rect stringRect = new Rect(position.x, position.y, position.width-20, 15);
        Rect addButtonRect = new Rect(position.x+ position.width - 15, position.y, 15, 15);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels

        EditorGUI.DelayedTextField(stringRect, property.FindPropertyRelative("conditionString"), GUIContent.none);
        GUI.color = Color.green;
        if(GUI.Button(addButtonRect, GUIContent.none))
        {
            if (Resources.FindObjectsOfTypeAll<Stat>().Length>0)
            {
                int index = property.FindPropertyRelative("conditionStats").arraySize;
                property.FindPropertyRelative("conditionStats").InsertArrayElementAtIndex(index);
                property.FindPropertyRelative("conditionStats").GetArrayElementAtIndex(index).objectReferenceValue = Resources.FindObjectsOfTypeAll<Stat>()[0];
            }
        }
        GUI.color = Color.white;

        for (int i = 0; i< property.FindPropertyRelative("conditionStats").arraySize; i++)
        {
            Rect popuprect = new Rect(position.x + 40, position.y + 20 + i*16, position.width-60, 16);
            Rect lableRect = new Rect(position.x, position.y + 20 + i * 16, 45, 16);
            Rect deleteRect = new Rect(position.x  + position.width - 16, position.y + 20 + i * 16, 16, 16);
            EditorGUI.PropertyField(popuprect, property.FindPropertyRelative("conditionStats").GetArrayElementAtIndex(i), GUIContent.none);
            EditorGUI.LabelField(lableRect, "[p"+i+"]");
            GUI.color = Color.red;
            if (GUI.Button(deleteRect, GUIContent.none))
            {
                property.FindPropertyRelative("conditionStats").DeleteArrayElementAtIndex(i);
                property.FindPropertyRelative("conditionStats").DeleteArrayElementAtIndex(i);
            }
            GUI.color = Color.white;
        }
        

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 5+EditorGUIUtility.singleLineHeight *(1+ property.FindPropertyRelative("conditionStats").arraySize);
    }
}