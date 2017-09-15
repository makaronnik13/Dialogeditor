using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine.UI;

public class QuestWindow : EditorWindow
{
    private delegate void StateDel(State state);

    public enum EditorMode
    {
        packs,
        chains
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
	public Chain currentChain;
	Vector2 paramsScrollPosition = Vector2.zero;
	Vector2 chainsScrollPosition = Vector2.zero;
	public EditorMode chainEditorMode = EditorMode.packs;
    Rect makingPathRect = new Rect(Vector2.one*0.12345f, Vector2.one*0.12345f);
    bool makingPath = false;
    Path startPath;
    private State menuState;
    private Path menuPath;
    private Chain menuChain;
    private Param menuParam;
    private StateLink menuStateLink;
    private Texture2D backgroundTexture;
	private Vector2 draggingVector;
	private State draggingState;
	private StateLink draggingStateLink;
	private State debuggingState;
    private UnityEngine.Object copyBuffer = new UnityEngine.Object();

    private float zoom
    {
        get
        {
            if (game)
            {
                return game.zoom;
            }
            return 1;
        }
        set
        {
            if (game)
            {
                game.zoom = value;
            }
        }
    }
	public State DebuggingState
	{
		set
		{
			debuggingState = value;
			if(!currentChain.states.Contains(debuggingState))
			{
				if(!game.chains.SelectMany(g=>g.states).Contains(debuggingState))
				{
					foreach(PathGame pg in FindObjectsOfTypeIncludingAssets(typeof(PathGame)))
					{
						if (pg.chains.SelectMany (g => g.states).Contains (debuggingState)) 
						{
							Init (pg);
							Repaint ();
						}
					}
				}

				foreach(Chain c in game.chains)
				{
					if(c.states.Contains(debuggingState))
					{
						currentChain = c;
						Repaint ();
					}
				}
			}
		}
	}

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
	public static QuestWindow Init(PathGame editedGame = null)
	{
		game = editedGame;
		return Init();
	}

    public void DebugPathGame(State s)
    {
        if (FindObjectOfType<DialogPlayer>())
        {
            FindObjectOfType<DialogPlayer>().onStateIn += DebugState;
        }
        DebugState(s);
    }

	static QuestWindow Init()
	{
        QuestWindow window = (QuestWindow)EditorWindow.GetWindow<QuestWindow>("Dialog creator", true, new Type[3] { typeof(Animator), typeof(Console), typeof(SceneView) });
		window.minSize = new Vector2(600, 400);
        window.ShowAuxWindow();
        return window;
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

        CopyPasteEvents();
        DeleteEvents();

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
			RecalculateWindowsPositions(currentChain.StartState.position.position);
		}

		chainEditorMode = newChainMode;

		if (game)
		{
			EditorUtility.SetDirty (game);
		}

	}

    private void CopyPasteEvents()
    {
        if (Event.current.control && Event.current.isKey)
        {
            if (Event.current.keyCode == KeyCode.C)
            {
                copyBuffer = Selection.activeObject;
            }

            if(chainEditorMode == EditorMode.chains)
            {
                if (copyBuffer.GetType() == typeof(State))
                {
                    if (Event.current.keyCode == KeyCode.V)
                    {
                        lastMousePosition = Event.current.mousePosition;
                        State s = CreateState();
                        EditorUtility.CopySerialized((State)copyBuffer, s);
                        s.position = new Rect(lastMousePosition.x - s.position.width / 2, lastMousePosition.y - s.position.height / 2, s.position.width, s.position.height);
                        Repaint();
                    }
                }

                if (copyBuffer.GetType() == typeof(StateLink))
                {
                    if (Event.current.keyCode == KeyCode.V)
                    {
                        lastMousePosition = Event.current.mousePosition;
                        StateLink s = CreateStateLink();
                        EditorUtility.CopySerialized((StateLink)copyBuffer, s);
                        s.position = new Rect(lastMousePosition.x - s.position.width / 2, lastMousePosition.y - s.position.height / 2, s.position.width, s.position.height);
                        Repaint();
                    }
                }

                if (copyBuffer.GetType() == typeof(Path))
                {
                    if (Event.current.keyCode == KeyCode.V)
                    {
                        Path p = null;
                        if (Selection.activeObject.GetType() == typeof(State))
                        {
                            p = ((State)Selection.activeObject).AddPath();
                        }
                        p = GuidManager.GetStateByPath((Path)Selection.activeObject).AddPath();
                        EditorUtility.CopySerialized((Path)copyBuffer, p);
                        Repaint();
                    }
                }
            }

            if (chainEditorMode == EditorMode.packs)
            {
                if (copyBuffer.GetType() == typeof(Param))
                {
                    if (Event.current.keyCode == KeyCode.V)
                    {
                        PasteParam();
                    }
                }

                if (copyBuffer.GetType() == typeof(Chain))
                {
                    if (Event.current.keyCode == KeyCode.V)
                    {
                        PasteChain();
                    }
                }
            }
        }
    }

    private void DeleteEvents()
    {
        if (Event.current.isKey && (Event.current.keyCode == KeyCode.Delete) || (Event.current.command && Event.current.keyCode == KeyCode.Backspace) && Selection.activeObject)
        {
            if (!Selection.activeObject)
            {
                return;
            }
            if (Selection.activeObject.GetType() == typeof(State))
            {
                menuState = (State)Selection.activeObject;
                RemoveState();
                return;
            }
            if (Selection.activeObject.GetType() == typeof(StateLink))
            {
                menuStateLink = (StateLink)Selection.activeObject;
                RemoveStateLink();
                return;
            }
            if (Selection.activeObject.GetType() == typeof(Path))
            {
                RemovePath((Path)Selection.activeObject);
                return;
            }
            if (Selection.activeObject.GetType() == typeof(Param))
            {
                RemoveParam((Param)Selection.activeObject);
                return;
            }
            if (Selection.activeObject.GetType() == typeof(Chain))
            {
                RemoveChain((Chain)Selection.activeObject);
            }
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
		
	void DebugState(State s)
	{
        currentChain = GuidManager.GetChainByState(s);
        chainEditorMode = EditorMode.chains;
        Repaint();
		DebuggingState = s;
	}



    void DrowPacksWindow ()
	{  
        GUILayout.BeginVertical();
        GUILayout.Space(30);

        GUILayout.BeginHorizontal();
        ChainLisClickEvent();
        DrawChainsList();
		DrawParamsList ();  
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
	}

    private void PasteChain()
    {
                Chain c = CreateChain();
                EditorUtility.CopySerialized((Chain)copyBuffer, c);
                Repaint();
    }

    void DrawParamsList()
	{
		paramsScrollPosition = GUILayout.BeginScrollView (paramsScrollPosition, false, true, GUILayout.Width(position.width/2-5), GUILayout.Height(position.height-20));
        GUILayout.BeginVertical();
        Param selectedParam = null;
        if (Selection.activeObject && Selection.activeObject.GetType() == typeof(Param))
        {
            selectedParam = (Param)Selection.activeObject;
        }

        List<string> paramsNames = game.parameters.Select(x => x.name).ToList();
        int index = -1;
        if (game.parameters.Contains(selectedParam))
        {
            index = game.parameters.IndexOf(selectedParam);
        }

        GUILayout.Space(30 * game.parameters.Count);
        int paramIndex = Tabs.DrawTabs(new Rect(0, 0, position.width / 2 - 20, 30 * game.parameters.Count), paramsNames.ToArray(), index, true);
        if (paramIndex >= 0)
        {
            Selection.activeObject = game.parameters[paramIndex];
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    private void ChainLisClickEvent()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            GenericMenu menu = new GenericMenu();

            for (int i = 0;i<game.chains.Count;i++)
            {
                if (new Rect(5, 30*(i+1), -30+position.width / 2, (30 * game.chains.Count) / game.chains.Count).Contains(Event.current.mousePosition))
                {
                    menuChain = game.chains[i];
                    menu.AddItem(new GUIContent("Add chain"), false, CreateNewChain);
                    menu.AddItem(new GUIContent("Remove"), false, RemoveChain);
                    menu.AddItem(new GUIContent("Copy"), false, CopyChain);
                    if (copyBuffer.GetType()==typeof(Chain))
                    {
                        menu.AddItem(new GUIContent("Paste values"), false, PasteChainValues); //!
                        menu.AddItem(new GUIContent("Paste chain"), false, PasteChain);
                    }
                    menu.ShowAsContext();
                    return;
                }
            }

            for (int i = 0; i < game.parameters.Count; i++)
            {
                if (new Rect(position.width / 2, 30*(1 + i), -5+position.width / 2 - 20, (30 * game.parameters.Count)/game.parameters.Count).Contains(Event.current.mousePosition))
                {
                    menuParam = game.parameters[i];
                    menu.AddItem(new GUIContent("Add param"), false, CreateNewParam);
                    menu.AddItem(new GUIContent("Remove"), false, RemoveParam);
                    menu.AddItem(new GUIContent("Copy"), false, CopyParam);
                    if (copyBuffer.GetType() == typeof(Param))
                    {
                        menu.AddItem(new GUIContent("Paste values"), false, PasteParamValues); //!
                        menu.AddItem(new GUIContent("Paste param"), false, PasteParam);
                    }
                    menu.ShowAsContext();
                   
                    return;
                }
            }

            menu.AddItem(new GUIContent("Add chain"), false, CreateNewChain);
            menu.AddItem(new GUIContent("Add param"), false, CreateNewParam);

            if (copyBuffer.GetType() == typeof(Chain))
            {
                menu.AddItem(new GUIContent("Paste chain"), false, PasteChain);
               
            }
            if (copyBuffer.GetType() == typeof(Param))
            {
                menu.AddItem(new GUIContent("Paste param"), false, PasteParam);
            }
            menu.ShowAsContext();
            return;


        }
    }

    private void CopyChain()
    {
        copyBuffer = menuChain;
    }

    private void RemoveChain()
    {
        RemoveChain(menuChain);
    }

    private void PasteChainValues()
    {
        EditorUtility.CopySerialized((Param)copyBuffer, menuChain);
    }

    private void PasteParam()
    { 
                Param p = CreateParam();
                EditorUtility.CopySerialized((Param)copyBuffer, p);
                Repaint(); 
    }

    private void PasteParamValues()
    {
        EditorUtility.CopySerialized((Param)copyBuffer, menuParam);
    }

    private void CopyParam()
    {
        copyBuffer = menuParam;
    }

    private void RemoveParam()
    {
        RemoveParam(menuParam);
    }


    void DrawChainsList()
	{
        chainsScrollPosition = GUILayout.BeginScrollView (chainsScrollPosition, false, true, GUILayout.Width(position.width/2-5), GUILayout.Height(position.height-20));
		GUILayout.BeginVertical();
        Chain selectedChain = null;
        if (Selection.activeObject && Selection.activeObject.GetType() == typeof(Chain))
        {
            selectedChain = (Chain)Selection.activeObject;
        }

        List<string> chainNames = game.chains.Select(x=>x.name).ToList();
        int index = -1;
        if (game.chains.Contains(selectedChain))
        {
            index = game.chains.IndexOf(selectedChain);
        }

        GUILayout.Space(30 * game.chains.Count);
        int chainIndex = Tabs.DrawTabs(new Rect(0, 0, position.width / 2 - 20, 30*game.chains.Count), chainNames.ToArray(), index, true);
        if (chainIndex>=0)
        {
            Selection.activeObject = game.chains[chainIndex];
        }
		GUILayout.EndVertical ();
		GUILayout.EndScrollView ();
	}


    void DrowChainsWindow ()
	{
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

        if (Event.current.type == EventType.ScrollWheel)
        {
			if(zoom>0.2f && zoom<1.5f)
			{
				RecalculateWindowsZoomPositions (Event.current.delta.y*0.01f);
			}
            zoom += Event.current.delta.y*0.01f;
            zoom = Mathf.Clamp(zoom, 0.2f, 1.5f);
            Repaint();
        }

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
			foreach (StateLink s in currentChain.links)
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
		StateLink upperLink = null;

        foreach (State state in currentChain.states)
		{
			if (DrawStateBox (state)) 
			{
				upperState = state;
			}
		}

		foreach(StateLink link in currentChain.links)
		{
			if(DrawStateLinkBox(link))
			{
				upperLink = link;
			}
		}

		if(upperState)
		{
			currentChain.states.Remove(upperState);
			currentChain.states.Insert(currentChain.states.Count, upperState);
		}

		if(upperLink)
		{
			currentChain.links.Remove(upperLink);
			currentChain.links.Insert(currentChain.links.Count, upperLink);
		}

        EndWindows ();


        Event evt = Event.current;

		if (evt.button == 1 && evt.type == EventType.MouseDown)
		{
			State onState = null;
            Path onPath = null;
			StateLink onStateLink = null;
			foreach(State s in currentChain.states)
			{
                int i = 0;
                foreach (Path p in s.pathes)
                {
                    if (new Rect(s.position.x + 16 * zoom * i, s.position.y + s.position.height, 15 * zoom, 15 * zoom).Contains(evt.mousePosition))
                    {
                        onPath = p;
                    }
                    i++;
                }

                if (s.position.Contains(evt.mousePosition))
				{
					onState = s;
				}
			}

			foreach(StateLink s in currentChain.links)
			{
				if(s.position.Contains(evt.mousePosition))
				{
					onStateLink = s;
				}
			}
            if (onPath)
            {
                menuPath = onPath;
                lastMousePosition = Event.current.mousePosition;
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Remove"), false, RemoveCurrentPath);
                menu.ShowAsContext();
            }
			else if (onState) {
				menuState = onState;
				lastMousePosition = Event.current.mousePosition;
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("Remove state"), false, RemoveState);
				menu.AddItem(new GUIContent("Add path"), false, AddPath);
				menu.AddItem(new GUIContent("Make start"), false, MakeStart);
				menu.ShowAsContext();
			} 
			else if(onStateLink)
			{
				menuStateLink = onStateLink;
				lastMousePosition = Event.current.mousePosition;
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("Remove"), false, RemoveStateLink);
				menu.ShowAsContext();
			}

			else
			{
				lastMousePosition = evt.mousePosition;
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("Add state"), false, CreateNewState);
				menu.AddItem(new GUIContent("Add state link"), false, CreateNewStateLink);
				menu.ShowAsContext();
			}
		}

        if (evt.button == 0 && evt.type == EventType.MouseUp)
        {
            makingPath = false;
        }
    }

    private void RemoveCurrentPath()
    {
        RemovePath(menuPath);
    }

    private void RecalculateWindowsPositions(Vector2 center)
    {
        if (currentChain!=null)
        {
            Vector2 delta;

			delta = position.center- center;
            
            foreach (State s in currentChain.states)
            {
				s.position = new Rect( s.position.position + delta, s.position.size);
            }
			foreach (StateLink s in currentChain.links)
			{
				s.position = new Rect(s.position.position + delta, s.position.size);
			}
        }
    }

	private void RecalculateWindowsZoomPositions(float force)
	{
		if (currentChain!=null)
		{
			foreach (State s in currentChain.states)
			{
				s.position = new Rect( (s.position.position - Event.current.mousePosition)*(1+force)+Event.current.mousePosition, new Vector2(208, 30) *zoom);
			}
			foreach (StateLink s in currentChain.links)
			{
				s.position = new Rect( (s.position.position - Event.current.mousePosition)*(1+force)+Event.current.mousePosition, new Vector2(100, 30) * zoom);
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
					if(currentChain.links.Select(x=>x.state).Contains(path.aimState))
					{
						end = currentChain.links.Find (x => x.state == path.aimState).position;
					}

					Rect start =new Rect(state.position.x+16*zoom * i, state.position.y + state.position.height, 15*zoom, 15*zoom);
					DrawNodeCurve(start, end, Color.gray);
                    Handles.EndGUI();
                }
                i++;
            }
        }
    }


	bool DrawStateLinkBox(StateLink state)
	{
		bool moving = false;
        //GUI.Box(_boxPos, state.description);
        string ss = "";
		if (state.state!=null)
        {
			ss = "=>";
			if(state.state.description!="")
			{
				ss += state.state.description.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
			}
            ss = ss.Substring(0, Mathf.Min(20, ss.Length));
        }

		if (Event.current.type == EventType.mouseDown && Event.current.button == 0 && state.position.Contains(Event.current.mousePosition)) 
		{
			moving = true;
			Selection.activeObject = state;
			draggingStateLink = state;
			draggingVector = state.position.position - Event.current.mousePosition;
			Repaint ();
		}

		if (Event.current.type == EventType.mouseDrag) {
			if(draggingStateLink==state)
			{
				state.position = new Rect (draggingVector + Event.current.mousePosition, state.position.size);
				Repaint ();
			}
		}
			
		if(Event.current.type == EventType.mouseUp && Event.current.button == 0 )
		{
			draggingStateLink = null;
		}


		GUI.backgroundColor = Color.cyan * 0.8f;
	

		if(Selection.activeObject == state)
		{
			GUI.backgroundColor = GUI.backgroundColor * 1.3f;
		}

		if (state.position.Contains(Event.current.mousePosition) && makingPath == true)
		{
			GUI.backgroundColor = Color.yellow;
			if (Event.current.button == 0 && Event.current.type == EventType.MouseUp && state.state)
			{
				startPath.aimState = state.state;
				makingPath = false;
				Repaint();
			}
		}

		GUIStyle s = new GUIStyle(GUI.skin.box);
		s.fontSize = Mathf.FloorToInt(15 * zoom);
		GUI.Box (state.position, ss, s);

        Event c = Event.current;

        GUI.backgroundColor = Color.white;
		return moving;
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
				state.position = new Rect (draggingVector + Event.current.mousePosition,  state.position.size);
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

		if(debuggingState == state)
		{
			GUI.backgroundColor = Color.red*0.8f;
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
        GUIStyle s = new GUIStyle(GUI.skin.box);
        s.fontSize = Mathf.FloorToInt(15 * zoom);
		GUI.Box (state.position, ss, s);

		//state.position = GUILayout.Window(currentChain.states.IndexOf(state), state.position, DoStateWindow, ss, GUILayout.Width(180), GUILayout.Height(30));

		int i = 0;

		Event c = Event.current;

		foreach (Path path in state.pathes)
		{
			Rect r = new Rect(state.position.x+16*zoom * i, state.position.y + state.position.height, 15*zoom, 15*zoom);
			GUI.backgroundColor = Color.white*0.5f;
			if (Selection.activeObject == path)
			{
				GUI.backgroundColor = Color.white;
			}
			GUI.Box(r, new GUIContent((Texture2D)Resources.Load("Icons/play-button") as Texture2D));

			if (r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
			{
				Selection.activeObject = path;
				Repaint ();
			}

			if (r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDrag)
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

	private void RemoveParam(Param deletingParam)
	{
		game.parameters.Remove (deletingParam);
		DestroyImmediate (deletingParam, true);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		Repaint ();
	}

	public void RemoveChain(Chain deletingChain)
	{
		game.chains.Remove (deletingChain);
		deletingChain.DestroyChain ();
		DestroyImmediate (deletingChain, true);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		Repaint ();
	}

	private void RemovePath(Path p)
	{
		foreach(State s in currentChain.states)
		{
			foreach(Path path in s.pathes)
			{
				if(path == p)
				{
					s.RemovePath(p);
					Repaint();
					return;
				}
			}
		}
	}

	private void RemoveState()
	{
		foreach (State s in currentChain.states)
		{
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
		Repaint ();
	}

	private void RemoveStateLink()
	{
		foreach (State s in currentChain.states)
		{
			foreach (Path p in s.pathes)
			{
				if (p.aimState == menuStateLink.state)
				{
					p.aimState = null;
				}
			}    
		}

		currentChain.RemoveStateLink(menuStateLink);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		Repaint ();
	}

	private void AddPath()
	{
		menuState.AddPath();
	}

	private void CreateNewState()
	{
		CreateState();
	}

	private void CreateNewStateLink()
	{
		CreateStateLink();
	}

	StateLink CreateStateLink()
	{
		StateLink newState = currentChain.AddStateLink();
		newState.position = new Rect (lastMousePosition.x - newState.position.width/2, lastMousePosition.y - newState.position.height/2, newState.position.width, newState.position.height);
		return newState;
	}

	State CreateState()
	{
		State newState = currentChain.AddState ();
        newState.position = new Rect (lastMousePosition.x - newState.position.width/2, lastMousePosition.y - newState.position.height/2, newState.position.width, newState.position.height);

		return newState;
	}

    void CreateNewParam()
    {
        CreateParam();
    }

	Param CreateParam()
	{
		Param newParam = CreateInstance<Param> ();
		newParam.id = GuidManager.GetItemGUID ();
		AssetDatabase.AddObjectToAsset (newParam, AssetDatabase.GetAssetPath(game));
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		game.parameters.Add (newParam);
        return newParam;
	}

    private void CreateNewChain()
    {
        CreateChain();
    }

	private Chain CreateChain()
	{
		Chain newChain = CreateInstance<Chain> ();
		newChain.Init (game);
		Repaint();
        return newChain;
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