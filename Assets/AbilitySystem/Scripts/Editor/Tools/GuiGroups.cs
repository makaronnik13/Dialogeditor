using UnityEngine;
using System;
using UnityEditor;

public class GuiGroups
{
	public bool open = false;
	public bool withToggle = true;
	public bool enable = false;
	public string name;
	private Action drawing;

	public GuiGroups(bool withToggle, string name, Action drawing)
	{
		this.withToggle = withToggle;
		this.name = name;
		this.drawing = drawing;
	}

	public bool DrawGroup()
	{
		GUI.backgroundColor = Color.gray;
		GUI.depth = 0;
		GUILayout.Label ("");

		Rect buttonRect = new Rect (GUILayoutUtility.GetLastRect().x+16, GUILayoutUtility.GetLastRect().y, GUILayoutUtility.GetLastRect().width-16, GUILayoutUtility.GetLastRect().height);
		Rect toggleRect = new Rect (GUILayoutUtility.GetLastRect().x, GUILayoutUtility.GetLastRect().y, 15, GUILayoutUtility.GetLastRect().height);

		if (GUI.Button (buttonRect, name)) 
		{
			open = !open;
		}

		GUI.backgroundColor = Color.white;

		if(withToggle)
		{
			GUI.depth = 1;
			enable = GUI.Toggle (toggleRect, enable, "");
		}

		GUI.depth = 0;

		if (open) 
		{
			EditorGUI.BeginDisabledGroup (withToggle && !enable);
			drawing.Invoke ();
			EditorGUI.EndDisabledGroup ();
		}
		return enable;
	}
}
