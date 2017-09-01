using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CustomEditor(typeof(PersonDialog))]
public class PersonDialogInspector : Editor
{
	private PersonDialog dialog;
	private SerializedObject dialogObject;
	private SerializedProperty dialogProperty;

	private void OnEnable()
	{
		dialog = (PersonDialog)target;
		dialogObject = new SerializedObject (dialog);
		dialogProperty = dialogObject.FindProperty ("pathEvents");
	}

	private void SetEvents()
	{
		List<Path> newPathes = new List<Path>();
		List<PathEvent> newEvents = new List<PathEvent>();
		foreach (State s in dialog.personChain.states)
		{
			foreach (Path p in s.pathes)
			{
				if (p.withEvent)
				{
					newEvents.Add (new PathEvent());
					newPathes.Add (p);
				}
			}
		}
		dialog.pathes = newPathes.ToArray ();
		dialog.pathEvents = newEvents.ToArray ();

		dialogObject = new SerializedObject (dialog);
		dialogProperty = dialogObject.FindProperty ("pathEvents");
	}

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Dialogs pack:");
		dialog.game = (PathGame)EditorGUILayout.ObjectField(dialog.game, typeof(PathGame), false);
        GUILayout.EndHorizontal();
		if (dialog.game && dialog.game.chains.Count>0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Dialog:");

			if (!dialog.game.chains.Contains(dialog.personChain))
            {
				dialog.personChain = dialog.game.chains[0];
				SetEvents ();
            }

			Chain ch = dialog.game.chains[EditorGUILayout.Popup(dialog.game.chains.IndexOf(dialog.personChain), dialog.game.chains.Select(x => x.dialogName).ToArray())];

			if (dialog.personChain!=ch)
            {
				dialog.personChain = ch;
				SetEvents ();
				Debug.Log (dialogProperty.arraySize+" "+dialog.pathEvents.Count());
            }
            GUILayout.EndHorizontal();
			int i = 0;
			foreach(KeyValuePair<Path, PathEvent> pathEvent in dialog.PathEventsList)
			{			
						string aim = "";
						
						if(pathEvent.Key.aimState!=null)
						{
							aim = pathEvent.Key.aimState.description;
						}
						GUILayout.Label (pathEvent.Key.text+"->"+aim);
						
						Undo.RecordObject(target, "Changed event");
						EditorGUILayout.PropertyField(dialogProperty.GetArrayElementAtIndex(i)); 
						dialogObject.ApplyModifiedProperties ();
				i++;
			}
        }
    }
}