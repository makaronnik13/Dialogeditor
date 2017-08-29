using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Chain: ScriptableObject
{
	public string dialogName
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}
		
	public State StartState;
	[HideInInspector]
    public State inspectedState;
	[HideInInspector]
	public List<State> states = new List<State>();

	public State AddState()
	{
		State newState = CreateInstance<State> ();
		newState.Init ();
		AssetDatabase.AddObjectToAsset (newState, AssetDatabase.GetAssetPath(this));
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		states.Add (newState);
		return newState;
	}

	public void RemoveState(State state)
	{
		if (state == StartState)
		{
			if (states.Count == 1)
			{
				StartState = AddState();
			}
			else
			{
				foreach (State s in states)
				{
					if (s!=state)
					{
						StartState = s;
						break;
					}
				}
			}
		}
		states.Remove (state);
		state.DestroyState ();
		DestroyImmediate (state, true);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
	}

	public void DestroyChain()
	{
		foreach(State s in states)
		{
			s.DestroyState ();
			DestroyImmediate (s, true);
		}
	}

	public void Init(PathGame game)
	{
		dialogName = "New chain";
		name = dialogName;
		AssetDatabase.AddObjectToAsset (this, AssetDatabase.GetAssetPath(game));
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		StartState = AddState();
	}
}
