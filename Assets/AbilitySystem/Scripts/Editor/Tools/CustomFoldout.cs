using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFoldout
{
	private static GUIStyle openFoldoutStyle; 
	private static GUIStyle closedFoldoutStyle; 
	private static bool initted;

	private static void Init()
	{
		openFoldoutStyle = new GUIStyle(GUI.skin.FindStyle("Button"));
		openFoldoutStyle.fontStyle = (FontStyle)1;
		openFoldoutStyle.stretchHeight = true;
		closedFoldoutStyle = new GUIStyle(openFoldoutStyle);
		openFoldoutStyle.normal = openFoldoutStyle.onNormal;
		openFoldoutStyle.active = openFoldoutStyle.onActive;
		initted = true;
	}

	public static bool Foldout(bool open, ref bool toggled, string text) { return Foldout(open, ref toggled, new GUIContent(text)); }
	public static bool Foldout(bool open, ref bool toggled, GUIContent text)
	{
		if (!initted) Init();
		if (open)
		{
			GUILayout.BeginHorizontal();
			toggled = GUILayout.Toggle(toggled, "", GUILayout.Width(30));
			if (GUILayout.Button(text, openFoldoutStyle, GUILayout.Height(20), GUILayout.ExpandWidth(true)))
			{
				GUI.FocusControl("");
				GUI.changed = false; // force change-checking group to take notice
				GUI.changed = true;
				return false;
			}
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.BeginHorizontal();
			toggled = GUILayout.Toggle(toggled, "", GUILayout.Width(30));
			if (GUILayout.Button(text, closedFoldoutStyle, GUILayout.Height(20)))
			{
				GUI.FocusControl("");
				GUI.changed = false; // force change-checking to take notice
				GUI.changed = true;
				return true;
			}
			GUILayout.EndHorizontal();
		}
		return open;
	}
}
