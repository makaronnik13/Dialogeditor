using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PersonDialog))]
public class PersonDialogInspector : Editor
{
    public override void OnInspectorGUI()
    {
        PersonDialog myTarget = (PersonDialog)target;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Dialogs pack:");
        myTarget.game = (PathGame)EditorGUILayout.ObjectField(myTarget.game, typeof(PathGame), false);
        GUILayout.EndHorizontal();
        if (myTarget.game && myTarget.game.chains.Count>0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Dialog:");
            if (!myTarget.game.chains.Contains(myTarget.personChain))
            {
                myTarget.personChain = myTarget.game.chains[0];
            }
            myTarget.personChain = myTarget.game.chains[EditorGUILayout.Popup(myTarget.game.chains.IndexOf(myTarget.personChain), myTarget.game.chains.Select(x => x.dialogName).ToArray())];
            GUILayout.EndHorizontal();

			foreach(State s in myTarget.personChain.states)
			{
				foreach(Path p in s.pathes)
				{
					if(p.withEvent)
					{
						string aim = "";
						if(p.aimState!=null)
						{
							aim = p.aimState.description;
						}
						GUILayout.Label (s.description.Substring(0,Mathf.Min(s.description.Length, 8))+" -> "+aim.Substring(0,Mathf.Min(aim.Length, 8)));
						SerializedObject serializedGame = new SerializedObject(p); 
						SerializedProperty onPath = serializedGame.FindProperty("pathEvent"); 
						EditorGUILayout.PropertyField(onPath); 
					}
				}
			}
        }
    }
}