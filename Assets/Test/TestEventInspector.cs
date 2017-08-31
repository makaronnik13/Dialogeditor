using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestEvent))]
public class TestEventInspector : Editor{

	private SerializedProperty prop;

	private void OnEnable()
	{
		TestEvent testEvent = (TestEvent)target;
		SerializedObject so = new SerializedObject (testEvent);
		prop = so.FindProperty("ev"); 
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(prop);
	}
}
