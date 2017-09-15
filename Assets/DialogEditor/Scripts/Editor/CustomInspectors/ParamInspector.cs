using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Param))]

public class ParamInspector : Editor 
{
	private Param p;
	private PathGame _game;
	private PathGame game
	{
		get
		{
			if(!_game)
			{
				p = (Param)target;
				_game = AssetDatabase.LoadAssetAtPath<PathGame>(AssetDatabase.GetAssetPath(p));
			}
			return _game;
		}
	}

	public override void OnInspectorGUI()
	{	
		p = (Param)target;

        EditorGUI.BeginChangeCheck();

        string pName = EditorGUILayout.TextArea (p.paramName);
        string pDescription = p.description;
        Sprite pImage = p.image;

        bool pShowing = !GUILayout.Toggle (!p.showing, "hidden");
		if (pShowing) {
			pDescription = EditorGUILayout.TextArea (p.description, GUILayout.Height (45));
			pImage = (Sprite)EditorGUILayout.ObjectField (p.image, typeof(Sprite), false);
		}

        if (EditorGUI.EndChangeCheck())
        {
            p.paramName = pName;
            p.showing = pShowing;
            p.description = pDescription;
            p.image = pImage;
        }

		ConditionChange removingConditionChange = null;
		foreach (ConditionChange conditionChange in p.autoActivatedChangesGUIDS)
		{
			GUILayout.BeginHorizontal();
			//DrawCondition(conditionChange.condition);
			GUI.color = Color.green;
			if (GUILayout.Button((Texture2D)Resources.Load("Icons/cancel") as Texture2D, GUILayout.Width(20), GUILayout.Height(20)))
			{
				conditionChange.changes.Add(new ParamChanges(p));
			}
			GUI.color = Color.red;
			if (GUILayout.Button("", GUILayout.Width(20), GUILayout.Height(20)))
			{
				removingConditionChange = conditionChange;
			}
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
			ParamChanges removingChanger = null;

			foreach (ParamChanges c in conditionChange.changes)
			{
				GUILayout.BeginHorizontal();
				DrawChanges(c);
				GUI.color = Color.red;
				if (GUILayout.Button("", GUILayout.Width(15), GUILayout.Height(15)))
				{
					removingChanger = c;
				}
				GUILayout.EndHorizontal();
			}
			if (removingChanger!=null)
			{
				conditionChange.changes.Remove(removingChanger);
			}
		}

		if (removingConditionChange!=null)
		{
			p.autoActivatedChangesGUIDS.Remove(removingConditionChange);
		}
		GUI.color = Color.white;
	}

	private void DrawChanges(ParamChanges change)
	{
		GUILayout.BeginVertical();
		GUI.color = Color.white;

		EditorGUILayout.BeginHorizontal();

		if (!game.parameters.Contains(change.aimParam))
		{
			if (game.parameters.Count > 0)
			{
				change.aimParam = game.parameters[0];
			}
			else
			{
				EditorGUILayout.EndHorizontal();
				return;
			}
		}
		change.aimParam = game.parameters[EditorGUILayout.Popup(game.parameters.IndexOf(change.aimParam), game.parameters.Select(x => x.paramName).ToArray())];

		GUILayout.Label("=");

		GUI.backgroundColor = Color.white;
		try
		{
			ExpressionSolver.CalculateFloat(change.changeString, new float[change.Parameters.Count].ToList());
		}
		catch
		{
			GUI.color = Color.red;
		}

		change.changeString = EditorGUILayout.TextArea(change.changeString, GUILayout.Width(58));
		GUI.color = Color.yellow;
		if (GUILayout.Button("", GUILayout.Height(15), GUILayout.Width(15)))
		{
			if (game.parameters.Count > 0)
			{
				change.AddParam(game.parameters[0]);
			}
		}
		GUI.color = Color.white;

		Param removingParam = null;
		EditorGUILayout.EndHorizontal();

		if (change.Parameters == null)
		{
			change = new ParamChanges(change.aimParam);
		}

		for (int j = 0; j < change.Parameters.Count; j++)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("[p" + j + "]", GUILayout.Width(35));

			if (!game.parameters.Contains(change.Parameters[j]))
			{
				if (game.parameters.Count > 0)
				{
					change.setParam(game.parameters[0], j);
				}
				else
				{
					removingParam = change.Parameters[j];
					continue;
				}
			}

			int v = EditorGUILayout.Popup(game.parameters.IndexOf(change.Parameters[j]), game.parameters.Select(x => x.paramName).ToArray());
			change.setParam(game.parameters[v], j);
			GUI.color = Color.red;
			if (GUILayout.Button("", GUILayout.Height(15), GUILayout.Width(15)))
			{
				removingParam = change.Parameters[j];
			}
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal();
		}

		if (removingParam != null)
		{
			change.RemoveParam(removingParam);
		}
		GUI.color = Color.white;
		GUILayout.EndVertical();
	}
}
