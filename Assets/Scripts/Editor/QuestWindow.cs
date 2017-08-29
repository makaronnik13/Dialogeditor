using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;

public class QuestWindow : EditorWindow
{
    private delegate void StateDel(State state);

    public enum EditorMode
    {
        packs,
        chains,
        parameters
    }

	private GUISkin QuestCreatorSkin
	{
		get
		{
			return Resources.Load ("Skins/QuestEditorSkin") as GUISkin;
		}
	}

    public Vector2 ParamsScrollPosition { get; private set; }

    public static PathGame game;
	Vector2 lastMousePosition;
	Chain currentChain;
	ReorderableList packList;
	string[] toolbarStrings = new string[] {"Packs", "Chains"};
	int toolbarInt = 0, selectedChain = 0;
	Vector2 packsScrollPosition = Vector2.zero;
	Vector2 chainsScrollPosition = Vector2.zero;
	EditorMode chainEditorMode = EditorMode.packs;
	Rect makingPathRect = new Rect(Vector2.one*0.12345f, Vector2.one*0.12345f);
    bool makingPath = false;
    State pathAimState;
    Path startPath;
    private State menuState;
	private Param deletingParam;
    private Texture2D backgroundTexture;
	private Vector2 draggingVector;
	private State draggingState;

    private Texture2D BackgroundTexture
    {
        get
        {
            if (backgroundTexture == null)
            {
                backgroundTexture = (Texture2D)Resources.Load("Icons/background") as Texture2D;
                BackgroundTexture.wrapMode = TextureWrapMode.Repeat;
            }
            return backgroundTexture;
        }
    }

	static void Init()
	{
		QuestWindow window = (QuestWindow)EditorWindow.GetWindow(typeof(QuestWindow), false,  "Dialog creator", true);
        window.minSize = new Vector2(600, 400);
		window.Show();
	}

    public static void Init(PathGame editedGame = null)
    {
        game = editedGame;
        Init();
    }

    void OnGUI()
	{
        if (Event.current.type == EventType.ValidateCommand)
        {
            switch (Event.current.commandName)
            {
                case "UndoRedoPerformed":
                    Repaint();
                    break;
            }

        }

        switch (chainEditorMode) {
			case EditorMode.chains:
                if (game)
                {
                    DrowChainsWindow();
                }
				break;
			case EditorMode.packs:
                if (game)
                {
                    DrowPacksWindow();
                }
				break;
			case EditorMode.parameters:
                if (game)
                {
                    DrowParamsWindow();
                }
				break;
			}

        EditorMode newChainMode = (EditorMode)Tabs.DrawTabs(new Rect(0, 0, position.width, 30), new string[] { "Dialogs", "Node tree", "Parameters" }, (int)chainEditorMode);
        if (newChainMode == EditorMode.chains && newChainMode!=chainEditorMode)
        {
            RecalculateWindowsPositions();
        }

        chainEditorMode = newChainMode;

        if (game)
		{
			game.SetDirty ();
		}
       
    }

    private void DrowParamsWindow()
    {
        Undo.RecordObject(game, "params");
        Event evt = Event.current;

		if (evt.button == 1 && evt.type == EventType.MouseDown)
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Add param"), false, CreateParam);
			menu.ShowAsContext();

		}

        GUILayout.BeginVertical();
        GUILayout.Space(35);

        ParamsScrollPosition = GUILayout.BeginScrollView(ParamsScrollPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar,GUILayout.Width(position.width-5), GUILayout.Height(position.height-35));
		GUILayout.BeginVertical();
		int i = 0;
		Vector2 rectSize = new Vector2 ((position.width-40)/3, 150);

        foreach (Param param in game.parameters)
        {
			if(i%3==0)
			{
				GUILayout.BeginHorizontal ();
			}
			Rect r = new Rect (new Rect (i % 3 * (rectSize.x + 5), Mathf.FloorToInt (i / 3) * (rectSize.y + 5 / 1.5f), rectSize.x, rectSize.y));
			EditorGUI.DrawRect (r, Color.gray/2);

			DoParamWindow (game.parameters[i], new Rect(r.position.x, r.position.y, r.width+3, r.height+3));
			//GUILayout.EndArea ();
			if(i%3==2 || i== game.parameters.Count-1)
			{
				GUILayout.EndHorizontal();
			}
			i++;
        }
		if(deletingParam!=null)
		{
			game.parameters.Remove (deletingParam);
			DestroyImmediate(deletingParam, true);
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
			deletingParam = null;
		}
		GUILayout.EndVertical ();
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        Undo.FlushUndoRecordObjects();
    }

	void DoParamWindow(Param p, Rect r)
	{
        p.scrollPosition = GUILayout.BeginScrollView (p.scrollPosition, false, false, GUIStyle.none, GUI.skin.label, GUILayout.Width(r.width), GUILayout.Height(r.height-3));
		GUILayout.BeginVertical (GUILayout.Width(r.width-5), GUILayout.Height((r.width-10)/1.5f));
        //GUILayout.Space (5);

		GUILayout.BeginHorizontal ();
		p.name = EditorGUILayout.TextArea (p.name);
		GUI.color = Color.red;
		if(GUILayout.Button("", GUILayout.Width(15), GUILayout.Height(15)))
		{
			deletingParam = p;
		}
		GUI.color = Color.white;
		GUILayout.EndHorizontal();


		p.showing = !GUILayout.Toggle (!p.showing, "hidden");
		if (p.showing) {
			p.description = EditorGUILayout.TextArea (p.description, GUILayout.Height (45), GUILayout.Width(r.width-10));
			p.image = (Sprite)EditorGUILayout.ObjectField (p.image, typeof(Sprite), false);
		}
	

        GUI.color = Color.cyan;
        if (GUILayout.Button("auto change"))
        {
            p.autoActivatedChangesGUIDS.Add(new ConditionChange(new Condition()));
        }
        GUI.color = Color.white;
	


        ConditionChange removingConditionChange = null;
        foreach (ConditionChange conditionChange in p.autoActivatedChangesGUIDS)
        {
            GUILayout.BeginHorizontal();
            DrawCondition(conditionChange.condition);
            GUI.color = Color.green;
            if (GUILayout.Button((Texture2D)Resources.Load("Icons/cancel") as Texture2D, GUILayout.Width(20), GUILayout.Height(20)))
            {
                conditionChange.changes.Add(new ParamChanges(p));
            }
            GUI.color = Color.red;
            if (GUILayout.Button("", GUILayout.Width(20), GUILayout.Height(20)))
            {
                removingConditionChange = conditionChange;
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            ParamChanges removingChanger = null;

            foreach (ParamChanges c in conditionChange.changes)
            {
                GUILayout.BeginHorizontal();
                DrawChanges(c);
                GUI.color = Color.red;
                if (GUILayout.Button("", GUILayout.Width(15), GUILayout.Height(15)))
                {
                    removingChanger = c;
                }
                GUILayout.EndHorizontal();
            }
            if (removingChanger!=null)
            {
                conditionChange.changes.Remove(removingChanger);
            }
        }

        if (removingConditionChange!=null)
        {
            p.autoActivatedChangesGUIDS.Remove(removingConditionChange);
        }
        
        GUI.color = Color.white;
		GUILayout.EndVertical ();
		GUILayout.EndScrollView ();
	}

	void CreateParam()
	{
		Param newParam = CreateInstance<Param> ();
		AssetDatabase.AddObjectToAsset (newParam, AssetDatabase.GetAssetPath(game));
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		game.parameters.Add (newParam);
	}

    void DrowPacksWindow ()
	{  
        GUILayout.BeginVertical();
        GUILayout.Space(30);

        GUILayout.BeginHorizontal();
        Undo.RecordObject(game, "packs and chains");
        DrawChainsList();
        Undo.FlushUndoRecordObjects();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
	}

    private void CreateChain()
    {
		Chain newChain = CreateInstance<Chain> ();
		newChain.Init (game);
		game.chains.Insert(0, newChain);
        Repaint();
    }

    void DrawChainsList()
	{
        Event evt = Event.current;

        if (evt.button == 1 && evt.type == EventType.MouseDown)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add dialog"), false, CreateChain);
            menu.ShowAsContext();
        }

        chainsScrollPosition = GUILayout.BeginScrollView (chainsScrollPosition, false, true, GUIStyle.none, GUI.skin.label,GUILayout.Width(position.width-5), GUILayout.Height(position.height-20));
		GUILayout.BeginVertical();

		Chain deletingChain = null;
         
		foreach(Chain chain in game.chains)
		{
			GUILayout.BeginHorizontal ();
			chain.dialogName = EditorGUILayout.TextArea (chain.dialogName, GUILayout.Height(15));
			if(GUILayout.Button("edit", GUILayout.Width(50), GUILayout.Height(15)))
			{
				currentChain = chain;
				chainEditorMode = EditorMode.chains;
			}
			if(GUILayout.Button("delete", GUILayout.Width(50), GUILayout.Height(15)))
			{
				deletingChain = chain;
			}
            GUILayout.EndHorizontal();
        }

		if(deletingChain!=null)
		{
            game.chains.Remove (deletingChain);
			deletingChain.DestroyChain ();
			DestroyImmediate (deletingChain, true);
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
	    }

		GUILayout.EndVertical ();
		GUILayout.EndScrollView ();
	}


    void DrowChainsWindow ()
	{
        Undo.RecordObject(game, "chains");
        if (currentChain == null)
        {
                foreach (Chain c in game.chains)
                {
                    currentChain = c;        
                }
            if (currentChain == null)
            {
                return;
            }
        }

        Rect fieldRect = new Rect(0, 30, position.width, position.height);
        GUI.DrawTextureWithTexCoords(fieldRect, BackgroundTexture, new Rect(0, 0, fieldRect.width / BackgroundTexture.width, fieldRect.height / BackgroundTexture.height));


        if (Event.current.type == EventType.MouseDown && Event.current.button == 2)
        {
            lastMousePosition = Event.current.mousePosition;
        }

        if (Event.current.type == EventType.MouseDrag && Event.current.button == 2)
        {
            Vector2 delta = Event.current.mousePosition - lastMousePosition;
            foreach (State s in currentChain.states)
            {	
                s.position = new Rect(s.position.position+delta, s.position.size);
            }
            lastMousePosition = Event.current.mousePosition;
            Repaint();
        }

        DrawAditional();

        BeginWindows ();

        if (makingPath && !makingPathRect.Contains(Event.current.mousePosition))
        {
			startPath.aimState = null;
        }

     
		State upperState = null;

        foreach (State state in currentChain.states)
		{
			if (DrawStateBox (state)) 
			{
				upperState = state;
			}
		}
		if(upperState)
		{
			currentChain.states.Remove(upperState);
			currentChain.states.Insert(currentChain.states.Count, upperState);
		}

        EndWindows ();


        Event evt = Event.current;

		if (evt.button == 1 && evt.type == EventType.MouseDown)
		{
			State onState = null;
			foreach(State s in currentChain.states)
			{
				if(s.position.Contains(evt.mousePosition))
				{
					onState = s;
				}
			}

			if (onState) {
				menuState = onState;
				lastMousePosition = Event.current.mousePosition;
				GenericMenu menu = new GenericMenu();
				Undo.RecordObject(game, "chainsed");
				menu.AddItem(new GUIContent("Remove state"), false, RemoveState);
				menu.AddItem(new GUIContent("Add path"), false, AddPath);
				menu.AddItem(new GUIContent("Make start"), false, MakeStart);
				Undo.FlushUndoRecordObjects();
				menu.ShowAsContext();
			} else {
				lastMousePosition = evt.mousePosition;
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("Add state"), false, CreateNewState);
				menu.ShowAsContext();
			}
		}

        if (evt.button == 0 && evt.type == EventType.MouseUp)
        {
            makingPath = false;
        }

        Undo.FlushUndoRecordObjects();
    }

    

    private void RecalculateWindowsPositions()
    {
        if (currentChain!=null)
        {
            Vector2 delta;

            delta = position.center - currentChain.StartState.position.position;
            
            foreach (State s in currentChain.states)
            {
                s.position = new Rect(s.position.position + delta, s.position.size);
            }
        }
    }

    void DrawAditional (){

        if (makingPath)
		{
			Handles.BeginGUI();
			Handles.color = Color.white;
            DrawNodeCurve(makingPathRect, new Rect(Event.current.mousePosition, Vector2.zero), Color.white);

            Handles.EndGUI();
            Repaint();
		}

        foreach (State state in currentChain.states)
        {
            int i = 0;
            foreach (Path path in state.pathes)
            {
                if (path.aimState != null && path.aimStateGuid!=-1)
                {
                    Handles.BeginGUI();
                    Handles.color = Color.red;

					Rect end = path.aimState.position;


					Rect start =new Rect(state.position.x+16 * i, state.position.y + state.position.height, 15, 15);
					DrawNodeCurve(start, end, Color.gray);
                    Handles.EndGUI();
                }
                i++;
            }
        }
    }


	bool DrawStateBox(State state)
	{
		bool moving = false;
        //GUI.Box(_boxPos, state.description);
        string ss = "";
        if (state.description!="")
        {
            ss = state.description.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
            ss = ss.Substring(0, Mathf.Min(20, ss.Length));
        }
       



		if (Event.current.type == EventType.mouseDown && Event.current.button == 0 && state.position.Contains(Event.current.mousePosition)) 
		{
			moving = true;
			Selection.activeObject = state;
			draggingState = state;
			draggingVector = state.position.position - Event.current.mousePosition;
			Repaint ();
		}

		if (Event.current.type == EventType.mouseDrag) {
			if(draggingState==state)
			{
				state.position = new Rect (draggingVector+Event.current.mousePosition, state.position.size);
				Repaint ();
			}
		}
			
		if(Event.current.type == EventType.mouseUp && Event.current.button == 0 )
		{
			draggingState = null;
		}

		if (currentChain.StartState == state) {
			GUI.backgroundColor = Color.green * 0.8f;
		} else 
		{
			GUI.backgroundColor = Color.white * 0.8f;
		}

		if(Selection.activeObject == state)
		{
			GUI.backgroundColor = GUI.backgroundColor * 1.3f;
		}

		if (state.position.Contains(Event.current.mousePosition) && makingPath == true)
		{
			GUI.backgroundColor = Color.yellow;
			if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
			{
				startPath.aimState = state;
				makingPath = false;
				Repaint();
			}
		}

		GUI.Box (state.position, ss);

        //state.position = GUILayout.Window(currentChain.states.IndexOf(state), state.position, DoStateWindow, ss, GUILayout.Width(180), GUILayout.Height(30));

        int i = 0;

        Event c = Event.current;

        foreach (Path path in state.pathes)
        {
			Rect r = new Rect(state.position.x+16 * i, state.position.y + state.position.height, 15, 15);
            GUI.backgroundColor = Color.white;
			if (Selection.activeObject == path)
            {
                GUI.backgroundColor = Color.gray;
            }
            GUI.Box(r, new GUIContent((Texture2D)Resources.Load("Icons/play-button") as Texture2D));

            if (r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
            {
                makingPath = true;
                makingPathRect = r;
                startPath = path;
            }

            if (r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
            {
                if (makingPath)
                {
					Selection.activeObject = path;
                }
                makingPath = false;
            }
            i++;
        }

        GUI.backgroundColor = Color.white;
		return moving;
    }
		

    private void MakeStart()
    {
        currentChain.StartState = menuState;
    }

    private void RemoveState()
    {
        foreach (State s in currentChain.states)
        {
            List<Path> removigPathes = new List<Path>();
            int i = 0;
            foreach (Path p in s.pathes)
            {
                if (p.aimState == menuState)
                {
					p.aimState = null;
                }
            }    
        }
			
		currentChain.RemoveState(menuState);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
    }

    private void AddPath()
    {
		menuState.AddPath();
    }

    private void CreateNewState()
    {
        CreateState();
    }



   

    State CreateState()
	{
		State newState = currentChain.AddState ();
		newState.position = new Rect (lastMousePosition.x - newState.position.width/2, lastMousePosition.y - newState.position.height/2, newState.position.width, newState.position.height);
        
		return newState;
	}

	private void OnDestroy()
	{
		if (game)
		{
			AssetDatabase.SaveAssets ();
		}
	}
		
    private void DrawCondition(Condition condition)
    {
        GUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = Color.white;
        try
        {
            ExpressionSolver.CalculateBool(condition.conditionString, condition.Parameters);
        }
        catch
        {
            GUI.color = Color.red;
        }

        condition.conditionString = EditorGUILayout.TextArea(condition.conditionString, GUILayout.Width((position.width - 40) / 3 - 80));

        GUI.color = Color.yellow;
        if (GUILayout.Button((Texture2D)Resources.Load("Icons/add") as Texture2D, GUILayout.Width(20), GUILayout.Height(20)))
        {
            if (game.parameters.Count > 0)
            {
                condition.AddParam(game.parameters[0]);
            }
        }

        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();

        Param removingParam = null;

        for (int i = 0; i < condition.Parameters.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("[p" + i + "]", GUILayout.Width(35));

            if (!game.parameters.Contains(condition.Parameters[i]))
            {
                if (game.parameters.Count > 0)
                {
                    condition.Parameters[i] = game.parameters[0];
                }
                else
                {
                    removingParam = condition.Parameters[i];
                    Repaint();
                    continue;
                }
            }
            condition.setParam(i, game.parameters[EditorGUILayout.Popup(game.parameters.IndexOf(condition.Parameters[i]), game.parameters.Select(x => x.name).ToArray())]);


            GUI.color = Color.red;
            if (GUILayout.Button("", GUILayout.Height(15), GUILayout.Width(15)))
            {
                removingParam = condition.Parameters[i];
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        if (removingParam != null)
        {
            condition.RemoveParam(removingParam);
        }
        GUILayout.EndVertical();
    }

    private void DrawChanges(ParamChanges change)
    {
        GUILayout.BeginVertical();
        GUI.color = Color.white;

            EditorGUILayout.BeginHorizontal();

            if (!game.parameters.Contains(change.aimParam))
            {
                if (game.parameters.Count > 0)
                {
                    change.aimParam = game.parameters[0];
                }
                else
                {
                    //removingChanger = path.changes[i];
                    EditorGUILayout.EndHorizontal();
                    return;
                }
            }
            change.aimParam = game.parameters[EditorGUILayout.Popup(game.parameters.IndexOf(change.aimParam), game.parameters.Select(x => x.name).ToArray())];

            GUILayout.Label("=");

            GUI.backgroundColor = Color.white;
            try
            {
                ExpressionSolver.CalculateFloat(change.changeString, change.Parameters);
            }
            catch
            {
                GUI.color = Color.red;
            }

            change.changeString = EditorGUILayout.TextArea(change.changeString, GUILayout.Width(58));
            GUI.color = Color.yellow;
            if (GUILayout.Button("", GUILayout.Height(15), GUILayout.Width(15)))
            {
                if (game.parameters.Count > 0)
                {
                    change.AddParam(game.parameters[0]);
                }
            }
            GUI.color = Color.white;

            Param removingParam = null;
            EditorGUILayout.EndHorizontal();

            if (change.Parameters == null)
            {
                change = new ParamChanges(change.aimParam);
            }

        for (int j = 0; j < change.Parameters.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("[p" + j + "]", GUILayout.Width(35));

                if (!game.parameters.Contains(change.Parameters[j]))
                {
                    if (game.parameters.Count > 0)
                    {
                        change.setParam(game.parameters[0], j);
                    }
                    else
                    {
                        removingParam = change.Parameters[j];
                        continue;
                    }
                }

                int v = EditorGUILayout.Popup(game.parameters.IndexOf(change.Parameters[j]), game.parameters.Select(x => x.name).ToArray());
                change.setParam(game.parameters[v], j);
                GUI.color = Color.red;
                if (GUILayout.Button("", GUILayout.Height(15), GUILayout.Width(15)))
                {
                    removingParam = change.Parameters[j];
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }

            if (removingParam != null)
            {
                change.RemoveParam(removingParam);
            }
            GUI.color = Color.white;
        GUILayout.EndVertical();
    }

    void DrawNodeCurve(Rect start, Rect end, Color c)
    {
		Vector3 startPos = new Vector3(start.x+start.width/2, start.y + start.height / 2, 0);
		Vector3 endPos = new Vector3(end.x+end.width/2, end.y + end.height / 2, 0);
        Vector3 startTan = startPos;
        Vector3 endTan = endPos;
        Color shadowCol = new Color(0, 0, 0, 0.06f);
        for (int i = 0; i < 2; i++) // Draw a shadow
        Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 7);
        Handles.DrawBezier(startPos, endPos, startTan, endTan, c, null, 3);
    }
}