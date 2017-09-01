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
	Vector2 paramsScrollPosition = Vector2.zero;
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

	#region LifeCycle
	public static void Init(PathGame editedGame = null)
	{
		game = editedGame;
		Init();
	}

	static void Init()
	{
		QuestWindow window = (QuestWindow)EditorWindow.GetWindow(typeof(QuestWindow), false,  "Dialog creator", true);
		window.minSize = new Vector2(600, 400);
		window.Show();
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
		}

		EditorMode newChainMode = (EditorMode)Tabs.DrawTabs(new Rect(0, 0, position.width, 30), new string[] { "Dialogs and parameters", "Node tree"}, (int)chainEditorMode);
		if (newChainMode == EditorMode.chains && newChainMode!=chainEditorMode)
		{
			RecalculateWindowsPositions();
		}

		chainEditorMode = newChainMode;

		if (game)
		{
			EditorUtility.SetDirty (game);
		}

	}

	private void OnDestroy()
	{
		if (game)
		{
			AssetDatabase.SaveAssets ();
		}
	}
	#endregion
		
    void DrowPacksWindow ()
	{  
        GUILayout.BeginVertical();
        GUILayout.Space(30);

        GUILayout.BeginHorizontal();
        Undo.RecordObject(game, "packs and chains");

		Event evt = Event.current;
		if (evt.button == 1 && evt.type == EventType.MouseDown)
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Add dialog"), false, CreateChain);
			menu.AddItem(new GUIContent("Add parameter"), false, CreateParam);
			menu.ShowAsContext();
		}

        DrawChainsList();
		DrawParamsList ();
        Undo.FlushUndoRecordObjects();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
	}

	void DrawParamsList()
	{
		chainsScrollPosition = GUILayout.BeginScrollView (paramsScrollPosition, false, true, GUILayout.Width(position.width/2-5), GUILayout.Height(position.height-20));
		GUILayout.BeginVertical();

		Param deletingParam = null;

		foreach(Param parameter in game.parameters)
		{
			GUILayout.BeginHorizontal ();
			parameter.paramName = EditorGUILayout.TextArea (parameter.paramName);
			if(GUILayout.Button("edit", GUILayout.Width(50), GUILayout.Height(15)))
			{
				Selection.activeObject = parameter;
			}
			if(GUILayout.Button("delete", GUILayout.Width(50), GUILayout.Height(15)))
			{
				deletingParam = parameter;
			}
			GUILayout.EndHorizontal();
		}

		if(deletingParam!=null)
		{
			game.parameters.Remove (deletingParam);
			DestroyImmediate (deletingParam, true);
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
		}

		GUILayout.EndVertical ();
		GUILayout.EndScrollView ();
	}

    void DrawChainsList()
	{
        chainsScrollPosition = GUILayout.BeginScrollView (chainsScrollPosition, false, true, GUILayout.Width(position.width/2-5), GUILayout.Height(position.height-20));
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

	#region menuDropdownEvents
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

	void CreateParam()
	{
		Param newParam = CreateInstance<Param> ();
		AssetDatabase.AddObjectToAsset (newParam, AssetDatabase.GetAssetPath(game));
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		game.parameters.Add (newParam);
	}

	private void CreateChain()
	{
		Chain newChain = CreateInstance<Chain> ();
		newChain.Init (game);
		game.chains.Insert(0, newChain);
		Repaint();
	}
	#endregion

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