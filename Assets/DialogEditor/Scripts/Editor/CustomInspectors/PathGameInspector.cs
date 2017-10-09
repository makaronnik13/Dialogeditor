using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathGame))]
public class PathGameInspector : Editor
{
    public override void OnInspectorGUI()
    {
        PathGame myTarget = (PathGame)target;
        EditorGUI.BeginChangeCheck();
        GUILayout.Label("Name:");
		string gName = EditorGUILayout.DelayedTextField(myTarget.name);
        GUILayout.Label("Author:");
		string gAuthor = EditorGUILayout.DelayedTextField(myTarget.autor);
        GUILayout.Label("Description:");
		string gDescription = EditorGUILayout.DelayedTextField(myTarget.description, GUILayout.Height(120));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myTarget, "Edit PathGame");
            myTarget.gameName = gName;
            string assetPath = AssetDatabase.GetAssetPath(myTarget.GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, assetPath.Replace(assetPath, gName));
            myTarget.autor = gAuthor;
            myTarget.description = gDescription;
        }
        if (GUILayout.Button("Edit"))
        {
            QuestWindow.Init(myTarget);
        }
    }
}
