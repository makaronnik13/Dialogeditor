using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestEvent))]
public class TestEventInspector : Editor{

	private SerializedProperty prop;
	private SerializedObject so;
	private void OnEnable()
	{
		TestEvent testEvent = (TestEvent)target;
		so = new SerializedObject (testEvent);
		prop = so.FindProperty("ev"); 
	}

	public override void OnInspectorGUI()
	{
		Undo.RecordObject(target, "Changed event");
		 EditorGUILayout.PropertyField(prop);
		so.ApplyModifiedProperties ();
	}
}
