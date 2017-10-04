using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StatValue))]
public class StatValuePropertyField : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, GUIContent.none, property);

		// Draw label
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		Rect statRect = new Rect (position.x, position.y, position.width/2, EditorGUIUtility.singleLineHeight);
		Rect valueRect = new Rect (position.x+position.width/2, position.y, position.width/2, position.height);

		EditorGUI.PropertyField (statRect, property.FindPropertyRelative("stat"), GUIContent.none);
		EditorGUI.PropertyField (valueRect, property.FindPropertyRelative("value"), GUIContent.none);
		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 3+EditorGUIUtility.singleLineHeight *(1+ property.FindPropertyRelative("value").FindPropertyRelative("conditionStats").arraySize);
	}
}