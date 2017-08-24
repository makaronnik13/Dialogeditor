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
            GUIDManager.SetInspectedGame(myTarget.game);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Dialog:");
            if (!myTarget.game.chains.Select(x => x.ChainGuid).Contains(myTarget.personChainId))
            {
                myTarget.personChainId = myTarget.game.chains[0].ChainGuid;
            }
            myTarget.personChainId = myTarget.game.chains[EditorGUILayout.Popup(myTarget.game.chains.IndexOf(GUIDManager.GetChainByGuid(myTarget.personChainId)), myTarget.game.chains.Select(x => x.name).ToArray())].ChainGuid;
            GUILayout.EndHorizontal();
        }
    }
}