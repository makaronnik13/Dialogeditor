using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(State))]
public class StateInspector : Editor
{
    private ReorderableList list;
    private SerializedObject _serializedObject;
    private State state;

    private void OnEnable()
    {
        state = (State)target;
        _serializedObject = new SerializedObject(state);
        list = new ReorderableList(_serializedObject, _serializedObject.FindProperty("pathes"), true, true, true, true);
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), ((State)target).pathes[index].text);
                if (new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight).Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseDown && Event.current.button == 0)
                {
                    Selection.activeObject = list.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue as Path;
                    Repaint();
                }
            };
        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "pathes");
        };
        list.onReorderCallback = (ReorderableList l) =>
        {
            Undo.RecordObject(state, "state path reorder");
            List<Path> newPathList = new List<Path>();
            for (int i = list.count - 1; i >= 0; i--)
            {
                newPathList.Add(l.serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue as Path);
            }
            state.pathes = newPathList;
            SearchableEditorWindow.GetWindow(typeof(EditorWindow)).Repaint();
        };
        list.onAddCallback = (ReorderableList l) =>
        {
            AssetDatabase.AddObjectToAsset(state.AddPath(), AssetDatabase.GetAssetPath(state));
            Repaint();
            SearchableEditorWindow.GetWindow(typeof(EditorWindow)).Repaint();
        };
        list.onRemoveCallback = (ReorderableList l) =>
        {
            Undo.RecordObject(state, "state path remove");
            state.RemovePath(l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as Path);
            Undo.DestroyObjectImmediate(l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as Path);
            Repaint();
            EditorWindow.GetWindow(typeof(EditorWindow)).Repaint();
        };
    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        string stateDescription = EditorGUILayout.DelayedTextField(state.description, GUILayout.Height(150));
        Sprite stateSprite = (Sprite)EditorGUILayout.ObjectField(state.image, typeof(Sprite), false, GUILayout.Width(80), GUILayout.Height(80));
        EditorGUILayout.EndHorizontal();
        AudioClip stateSound = (AudioClip)EditorGUILayout.ObjectField(state.sound, typeof(AudioClip), false);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(state, "state base properties");
            state.description = stateDescription;
            state.sound = stateSound;
            state.image = stateSprite;
        }
        GUILayout.Space(EditorGUIUtility.singleLineHeight);
        _serializedObject.Update();
        list.DoLayoutList();
        _serializedObject.ApplyModifiedProperties();
    }
}
