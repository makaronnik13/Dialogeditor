using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ability))]
public class SkillInspector : Editor
{
	private List<GuiGroups> groups = new List<GuiGroups> ();
    protected Ability skill;

	void OnEnable()
	{
		skill = (Ability)target;
		groups.Clear ();

		groups.Add (new GuiGroups(false, "ability", ()=>{
			DrawBaseInfo();
		}));

		groups.Add (new GuiGroups(false, "upgrade", ()=>{
			DrawUpgrade();
		}));

		groups.Add (new GuiGroups(true, "activation", ()=>{
			DrawActivation();
		}));

        /*
		groups.Add (new GuiGroups(true, "auto activation", ()=>{
			DrawAutoActivation();
		}));
        */

		groups.Add (new GuiGroups(true, "modificator", ()=>{
			DrawModificator();
		}));

		groups.Add (new GuiGroups(true, "aura", ()=>{
			DrawAura();
		}));
			
		groups [0].open = true;
		foreach(GuiGroups gg in groups)
		{
			switch(gg.name)
			{
			case "activation":
				gg.enable = skill.activating;
				break;
			case "modificator":
				gg.enable = skill.withModificator;
				break;
			case "aura":
				gg.enable = skill.withAura;
				break;
			case "auto activation":
				gg.enable = skill.withAura;
				break;
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		foreach(GuiGroups gg in groups)
		{
			gg.DrawGroup ();
			switch(gg.name)
			{
			case "activation":
				skill.activating = gg.enable;
				break;
			case "modificator":
				skill.withModificator = gg.enable;
				break;
			case "aura":
				skill.withAura = gg.enable;
				break;
			case "auto activation":
				skill.withAura = gg.enable;
				break;
			}
		}
	}

	private void DrawBaseInfo()
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("name", GUILayout.Width(200));
        skill.Name = GUILayout.TextField(skill.Name);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("max lvl", GUILayout.Width(200));
        skill.maxLevel = EditorGUILayout.IntField(skill.maxLevel);
        GUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
        GUILayout.EndVertical();
        skill.icon = (Sprite)EditorGUILayout.ObjectField(skill.icon, typeof(Sprite), false, GUILayout.Width(80), GUILayout.Height(80));
        GUILayout.EndHorizontal();
    }

	private void DrawActivation()
    {
		GUILayout.Label("coldown", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldown"), GUIContent.none);

		GUILayout.Label("cost", EditorStyles.boldLabel);

        if (GUILayout.Button("add cost"))
        {
			skill.value.Add(new StatValue());
			serializedObject.Update ();
        }
        
        for (int i = 0; i< serializedObject.FindProperty("value").arraySize; i++)
        {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("value").GetArrayElementAtIndex(i), GUIContent.none);
			GUI.color = Color.red;
			if(GUILayout.Button("",GUILayout.Width(15), GUILayout.Height(15)))
			{
				skill.value.RemoveAt (i);
				serializedObject.Update ();
			}
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal ();
        }



    }

	private void DrawModificator()
	{
		GUILayout.Label("condition", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("modificator").FindPropertyRelative("Condition"), GUIContent.none);

		GUILayout.Label("changers", EditorStyles.boldLabel);

		if (GUILayout.Button("add changer"))
		{
			skill.modificator.Changers.Add(new ModificatorChanger());
			serializedObject.Update ();
		}

		for (int i = 0; i< serializedObject.FindProperty("modificator").FindPropertyRelative("Changers").arraySize; i++)
		{
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("modificator").FindPropertyRelative("Changers").GetArrayElementAtIndex(i), GUIContent.none);
			GUI.color = Color.red;
			if(GUILayout.Button("",GUILayout.Width(15), GUILayout.Height(15)))
			{
				skill.modificator.Changers.RemoveAt (i);
				serializedObject.Update ();
			}
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal ();
		}
	}

	private void DrawAura()
	{
		GUILayout.Label("condition", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("modificator").FindPropertyRelative("Condition"), GUIContent.none);

		GUILayout.Label("changers", EditorStyles.boldLabel);

		if (GUILayout.Button("add changer"))
		{
			skill.modificator.Changers.Add(new ModificatorChanger());
			serializedObject.Update ();
		}

		for (int i = 0; i< serializedObject.FindProperty("modificator").FindPropertyRelative("Changers").arraySize; i++)
		{
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("modificator").FindPropertyRelative("Changers").GetArrayElementAtIndex(i), GUIContent.none);
			GUI.color = Color.red;
			if(GUILayout.Button("",GUILayout.Width(15), GUILayout.Height(15)))
			{
				skill.modificator.Changers.RemoveAt (i);
				serializedObject.Update ();
			}
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal ();
		}

		GUILayout.Label("taggs", EditorStyles.boldLabel);
		if (GUILayout.Button("add tag"))
		{
			skill.auraTags.Add("");
		}
		for(int i = 0; i< skill.auraTags.Count;i++)
		{
			GUILayout.BeginHorizontal ();
			skill.auraTags[i] = EditorGUILayout.DelayedTextField (skill.auraTags[i]);

			GUI.color = Color.red;
			if(GUILayout.Button("",GUILayout.Width(15), GUILayout.Height(15)))
			{
				skill.auraTags.RemoveAt (i);
			}
			GUI.color = Color.white;
			GUILayout.EndHorizontal ();
		}

		GUILayout.Label("field", EditorStyles.boldLabel);
		skill.auraGlobal = GUILayout.Toggle (skill.auraGlobal, "global");
		if(!skill.auraGlobal)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("auraRadius"));
		}
	}

	private void DrawAutoActivation()
	{

	}

	private void DrawUpgrade()
    {
		
		GUILayout.Label("condition", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("upgradeCondition"), GUIContent.none);

		GUILayout.Label("cost", EditorStyles.boldLabel);

		if (GUILayout.Button("add cost"))
		{
			skill.cost.Add(new StatValue());
			serializedObject.Update ();
		}
		for (int i = 0; i< serializedObject.FindProperty("cost").arraySize; i++)
		{
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("cost").GetArrayElementAtIndex(i), GUIContent.none);
			GUI.color = Color.red;
			if(GUILayout.Button("",GUILayout.Width(15), GUILayout.Height(15)))
			{
				skill.cost.RemoveAt (i);
				serializedObject.Update ();
			}
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal ();
		}
    }


}