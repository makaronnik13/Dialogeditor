using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ability))]
public class SkillInspector : Editor {

	protected Ability skill;

	public override void OnInspectorGUI()
	{
		skill = (Ability)target;

		BaseInfo ();

		Activation ();

		Upgrade ();
	}

	protected virtual void BaseInfo()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.BeginVertical ();
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("name", GUILayout.Width(200));
		skill.name = GUILayout.TextField (skill.name);
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("max lvl", GUILayout.Width(200));
		skill.maxLevel = EditorGUILayout.IntField (skill.maxLevel);
		GUILayout.EndHorizontal ();
		EditorGUILayout.PropertyField (serializedObject.FindProperty("description"));
		GUILayout.EndVertical ();
		skill.icon = (Sprite)EditorGUILayout.ObjectField(skill.icon, typeof(Sprite), false, GUILayout.Width(80), GUILayout.Height(80));
		GUILayout.EndHorizontal ();
	}

	protected virtual void Activation()
	{
		GUILayout.Space (EditorGUIUtility.singleLineHeight);
		EditorGUILayout.PropertyField (serializedObject.FindProperty("cost"));
	}

	protected virtual void Upgrade()
	{
		GUILayout.Space (EditorGUIUtility.singleLineHeight);
		EditorGUILayout.PropertyField (serializedObject.FindProperty("upgradeCondition"));
	}
}
