using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ModificatorChanger))]
public class ModificatorChangerProperty : PropertyDrawer
{
	private ModificatorChanger changer;

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
		Rect aimRect = new Rect(position.x, position.y, position.width/2, 15);
		Rect valueRect = new Rect(position.x+ position.width/2, position.y, position.width/2, position.height);

		// Draw fields - passs GUIContent.none to each so they are drawn without labels

		EditorGUI.PropertyField(aimRect, property.FindPropertyRelative("aimStat"), GUIContent.none);
		EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("modificatorStruct"), GUIContent.none);

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 5+EditorGUIUtility.singleLineHeight *(1+ property.FindPropertyRelative("modificatorStruct").FindPropertyRelative("conditionStats").arraySize);
	}
}