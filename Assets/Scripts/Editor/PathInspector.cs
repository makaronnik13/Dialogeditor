using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Path))]

public class PathInspector : Editor {

	private Path p;
	private PathGame game;

	public void OnEnable()
	{
		p = (Path)target;
		game = AssetDatabase.LoadAssetAtPath<PathGame>(AssetDatabase.GetAssetPath(p));
	}

	public override void OnInspectorGUI()
	{
		GUILayout.BeginHorizontal ();
		p.text = EditorGUILayout.TextArea(p.text, GUILayout.Height(30));
		GUI.color = Color.white;
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		p.auto = GUILayout.Toggle(p.auto, "auto", GUILayout.Width(60));
		p.withEvent = GUILayout.Toggle(p.withEvent, "action", GUILayout.Width(60));
		EditorGUI.BeginDisabledGroup (game.parameters.Count == 0);
		if (GUILayout.Button("add condition param"))
		{
				p.condition.AddParam(game.parameters[0]);
		}
		if (GUILayout.Button("add param changer"))
		{
				p.changes.Add(new ParamChanges(game.parameters[0]));
		}
		EditorGUI.EndDisabledGroup ();
		GUILayout.EndHorizontal ();
		GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
		//inspectedState.pathes[inspectedPath].waitInput = GUILayout.Toggle(inspectedState.pathes[inspectedPath].waitInput, "wait input", GUILayout.Width(80));
		DrawCondition (p);
		DrawChanges (p);
		GUI.color = Color.white;
	}

	private void DrawCondition(Path path)
	{
		EditorGUILayout.LabelField ("condition:");
		EditorGUILayout.BeginHorizontal ();

		GUI.backgroundColor = Color.white;
		try
		{
			ExpressionSolver.CalculateBool(path.condition.conditionString, path.condition.Parameters);
		}
		catch
		{
			GUI.color = Color.red;
		}

		path.condition.conditionString = EditorGUILayout.TextArea (path.condition.conditionString);



		GUI.color = Color.white;
		EditorGUILayout.EndHorizontal ();

		Param removingParam = null;

		for(int i = 0;i<path.condition.Parameters.Count;i++)
		{
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("[p"+i+"]", GUILayout.Width(35));

			if(!game.parameters.Contains(path.condition.Parameters[i]))
			{
				if(game.parameters.Count>0){
					path.condition.Parameters [i] = game.parameters [0];
				}
				else{
					removingParam = path.condition.Parameters[i];
					continue;
				}
			}
			path.condition.setParam(i, game.parameters[EditorGUILayout.Popup (game.parameters.IndexOf(path.condition.Parameters[i]), game.parameters.Select (x => x.name).ToArray())]); 


			GUI.color = Color.red;
			if(GUILayout.Button("", GUILayout.Height(15), GUILayout.Width(15)))
			{
				removingParam = path.condition.Parameters[i];
			}
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal ();
		}

		if(removingParam!=null)
		{
			path.condition.RemoveParam (removingParam);
		}

	}

	private void DrawChanges(Path path)
	{
		ParamChanges removingChanger = null;
		for (int i = 0; i< path.changes.Count; i++) {
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("delete")) 
			{
				removingChanger = path.changes[i];
			}

			if (GUILayout.Button ("add param")) 
			{
				path.changes[i].AddParam (game.parameters [0]);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();

			if(!game.parameters.Contains(path.changes[i].aimParam))
			{
				if (game.parameters.Count > 0) {
					path.changes [i].aimParam = game.parameters [0];	
				} else {
					removingChanger = path.changes [i];
					continue;
				}
			}
			path.changes [i].aimParam = game.parameters [EditorGUILayout.Popup (game.parameters.IndexOf (path.changes[i].aimParam), game.parameters.Select (x => x.name).ToArray (), GUILayout.Width(100))]; 

			GUILayout.Label ("=", GUILayout.Width(15));

            GUI.backgroundColor = Color.white;
            try
            {
                ExpressionSolver.CalculateFloat(path.changes[i].changeString, path.changes[i].Parameters);
            }
            catch
            {
                GUI.color = Color.red;
            }

            path.changes [i].changeString = EditorGUILayout.TextArea(path.changes [i].changeString);

			GUI.color = Color.white;

			Param removingParam = null;
			EditorGUILayout.EndHorizontal ();


			for (int j = 0; j < path.changes[i].Parameters.Count; j++) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("[p" + j + "]", GUILayout.Width (35));

				if (!game.parameters.Contains (path.changes[i].Parameters [j])) {
					if (game.parameters.Count > 0) {
						path.changes[i].setParam(game.parameters [0], j);
					} else {
						removingParam = path.changes[i].Parameters [j];
						continue;
					}
				}

				int v = EditorGUILayout.Popup (game.parameters.IndexOf (path.changes[i].Parameters [j]), game.parameters.Select (x => x.name).ToArray ());


                path.changes[i].setParam(game.parameters[v], j);
                

                GUI.color = Color.red;
				if (GUILayout.Button ("", GUILayout.Height (15), GUILayout.Width (15))) {
					removingParam = path.changes[i].Parameters [j];
				}
				GUI.color = Color.white;
				EditorGUILayout.EndHorizontal ();
			}

			if (removingParam != null) {
				path.changes[i].RemoveParam (removingParam);
			}

			GUI.color = Color.white;

		}
		if(removingChanger!=null)
		{
			path.changes.Remove (removingChanger);
		}
	}

}
