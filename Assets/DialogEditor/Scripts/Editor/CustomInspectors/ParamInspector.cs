using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Param))]

public class ParamInspector : Editor
{
    private Param p;
    private PathGame _game;
    private PathGame game
    {
        get
        {
            if (!_game)
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
		EditorGUILayout.LabelField ("name:");
		EditorGUILayout.BeginHorizontal ();
		string pName = EditorGUILayout.DelayedTextField(p.paramName);
		bool pShowing = !GUILayout.Toggle(!p.showing, "hidden", GUILayout.Width(60));
		EditorGUILayout.EndHorizontal ();
        string pDescription = p.description;
        Sprite pImage = p.image;
        if (pShowing)
        {
			EditorGUILayout.LabelField ("description:");
			EditorGUILayout.BeginHorizontal ();
			pDescription = EditorGUILayout.DelayedTextField(p.description, GUILayout.Height(60));
			pImage = (Sprite)EditorGUILayout.ObjectField(p.image, typeof(Sprite), false, GUILayout.Width(60), GUILayout.Height(60));
			EditorGUILayout.EndHorizontal ();
        }

		EditorGUILayout.LabelField ("tags:");
		string pTags = EditorGUILayout.DelayedTextField(p.tags);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(p, "param base properties");
            p.paramName = pName;
            p.showing = pShowing;
            p.description = pDescription;
            p.image = pImage;
			p.tags = pTags;
        }
    }
}
