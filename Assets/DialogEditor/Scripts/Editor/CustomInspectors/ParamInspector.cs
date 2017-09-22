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
        string pName = EditorGUILayout.TextArea(p.paramName);
        string pDescription = p.description;
        Sprite pImage = p.image;
        bool pShowing = !GUILayout.Toggle(!p.showing, "hidden");
        if (pShowing)
        {
            pDescription = EditorGUILayout.TextArea(p.description, GUILayout.Height(45));
            pImage = (Sprite)EditorGUILayout.ObjectField(p.image, typeof(Sprite), false);
        }
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(p, "param base properties");
            p.paramName = pName;
            p.showing = pShowing;
            p.description = pDescription;
            p.image = pImage;
        }
    }
}
