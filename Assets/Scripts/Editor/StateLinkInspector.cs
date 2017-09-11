using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(StateLink))]
public class StateLinkInspector : Editor {

	private StateLink link;
	private PathGame game;

	private void OnEnable()
	{
		link = (StateLink)target;
		game = AssetDatabase.LoadAssetAtPath<PathGame> (AssetDatabase.GetAssetPath(link)) as PathGame;
			Debug.Log (game);
		if(link.chain == null)
		{
			link.chain = game.chains [0];
			link.state = link.chain.StartState;
		}
	}

	public override void OnInspectorGUI()
	{

		if(!link.chain)
		{
			return;
		}

		link.chain = game.chains[EditorGUILayout.Popup(game.chains.IndexOf(link.chain), game.chains.Select(x => x.name).ToArray())];
		if(link.chain)
		{
			link.state = link.chain.states [EditorGUILayout.Popup (link.chain.states.IndexOf (link.state), link.chain.states.Select (x => x.description).ToArray ())];
		}
	}
}
