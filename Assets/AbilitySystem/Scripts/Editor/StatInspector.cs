using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Stat))]
public class StatInspector : Editor
{
    private SerializedObject sTarget;

    public override void OnInspectorGUI()
    {
        Stat stat = (Stat)target;
        if (sTarget == null)
        {
            sTarget = new SerializedObject(target);
        }


        EditorGUILayout.BeginVertical();

        stat.Name =  EditorGUILayout.DelayedTextField(stat.Name);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("description", GUILayout.Width(65));
        EditorGUILayout.PropertyField(sTarget.FindProperty("description"), GUIContent.none);
        EditorGUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("min", GUILayout.Width(65));
        EditorGUILayout.PropertyField(sTarget.FindProperty("minEvaluator"), GUIContent.none);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("max", GUILayout.Width(65));
        EditorGUILayout.PropertyField(sTarget.FindProperty("maxEvaluator"), GUIContent.none);
        GUILayout.EndHorizontal();

        stat.EvaluatedValue = GUILayout.Toggle(stat.EvaluatedValue, "evaluated");
        if (stat.EvaluatedValue)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("value", GUILayout.Width(65));
            EditorGUILayout.PropertyField(sTarget.FindProperty("valueEvaluator"), GUIContent.none);
            GUILayout.EndHorizontal();
        }


        EditorGUILayout.EndVertical();
    }
}