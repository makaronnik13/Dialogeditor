using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (PathGame))]
public class PathGameInspector : Editor {

    public override void OnInspectorGUI()
    {
        PathGame myTarget = (PathGame)target;
        GUILayout.Label("Name:");
        myTarget.name = EditorGUILayout.TextField(myTarget.name);
        GUILayout.Label("Author:");
        myTarget.autor = EditorGUILayout.TextField(myTarget.autor);
        GUILayout.Label("Description:");
        myTarget.description = EditorGUILayout.TextArea(myTarget.description, GUILayout.Height(120));
        if (GUILayout.Button("Edit"))
        {
            QuestWindow.Init(myTarget);
        }
    }
}
