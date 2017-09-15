using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (PathGame))]
public class PathGameInspector : Editor {

    public override void OnInspectorGUI()
    {
        PathGame myTarget = (PathGame)target;

        EditorGUI.BeginChangeCheck();
        GUILayout.Label("Name:");
        string gName = EditorGUILayout.TextField(myTarget.name);
        GUILayout.Label("Author:");
        string gAuthor = EditorGUILayout.TextField(myTarget.autor);
        GUILayout.Label("Description:");
        string gDescription = EditorGUILayout.TextArea(myTarget.description, GUILayout.Height(120));

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myTarget, "Edit PathGame");
            myTarget.gameName = gName;
            myTarget.autor = gAuthor;
            myTarget.description = gDescription;
        }
        
        if (GUILayout.Button("Edit"))
        {
            QuestWindow.Init(myTarget);
        }
    }
}
