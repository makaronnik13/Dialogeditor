using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(State))]
public class StateInspector : Editor {

	private ReorderableList list;
	private SerializedObject _serializedObject;
	private State state;

	private void OnEnable() {
		state = (State)target;
        _serializedObject = new SerializedObject (state);
		list = new ReorderableList(_serializedObject, 
			_serializedObject.FindProperty("pathes"), 
			true, true, true, true);

		list.drawElementCallback =  
			(Rect rect, int index, bool isActive, bool isFocused) => {
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += 2;
			EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "l");
		};
	}

	public override void OnInspectorGUI()
	{
		state.description = EditorGUILayout.TextArea (state.description, GUILayout.Height(75));
		state.sound = (AudioClip)EditorGUILayout.ObjectField (state.sound, typeof(AudioClip), false);
		GUILayout.Space (15);
		GUILayout.Label ("Pathes:");
		_serializedObject.Update();
		//list.DoLayoutList();
		_serializedObject.ApplyModifiedProperties();
	}
}
