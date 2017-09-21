using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine.UI;

public class QuestWindow : EditorWindow
{
    public enum EditorMode
    {
        packs,
        chains
    }

	private static GUISkin QuestCreatorSkin
	{
		get
		{
			return Resources.Load ("Skins/QuestEditorSkin") as GUISkin;
		}
	}

    public static PathGame game;
    private static Vector2 lastMousePosition;
	public static Chain currentChain;
    private static Vector2 paramsScrollPosition = Vector2.zero;
    private static Vector2 chainsScrollPosition = Vector2.zero;
    public static EditorMode chainEditorMode = EditorMode.packs;
    private static Rect makingPathRect = new Rect(Vector2.one*0.12345f, Vector2.one*0.12345f);
    private static bool makingPath = false;
    private static Path startPath;
    private static State menuState;
    private static Path menuPath;
    private static Chain menuChain;
    private static Param menuParam;
    private static StateLink menuStateLink;
	private static Vector2 draggingVector;
	private static State draggingState;
	private static StateLink draggingStateLink;
    private static State debuggingState;
    private static UnityEngine.Object copyBuffer = new UnityEngine.Object();
    private static Texture2D backgroundTexture;

    private static float zoom
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
					foreach(PathGame pg in Resources.FindObjectsOfTypeAll<PathGame>())
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

    private static Texture2D BackgroundTexture
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

    public string GamePath {
        get
        {
            return AssetDatabase.GetAssetPath(game.GetInstanceID());
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
        paramsScrollPosition = Vector2.zero;
        currentChain = null;
        paramsScrollPosition = Vector2.zero;
        chainsScrollPosition = Vector2.zero;
        chainEditorMode = EditorMode.packs;
        makingPathRect = new Rect(Vector2.one * 0.12345f, Vector2.one * 0.12345f);
        makingPath = false;
        startPath = null;
        menuState = null;
        menuPath = null;
        menuChain = null;
        menuParam = null;
        menuStateLink = null;
        draggingVector = Vector2.zero;
        draggingState = null;
        draggingStateLink = null;
        debuggingState = null;
        copyBuffer = null;
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

        EditorMode newChainMode = (EditorMode)Tabs.DrawTabs(new Rect(0, 0, position.width, 30), new string[] { "Dialogs and parameters", "Node tree" }, (int)chainEditorMode);
        if (newChainMode == EditorMode.chains && newChainMode != chainEditorMode)
        {
            if (currentChain==null && game.chains.Count>0)
            {
                currentChain = game.chains[0];
            }
            if (currentChain!=null)
            {
                RecalculateWindowsPositions(currentChain.StartState.position.position);
            }
        }

        chainEditorMode = newChainMode;


        switch (chainEditorMode) {
		case EditorMode.chains:
			if (game && currentChain)
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

		
		if (game)
		{
			EditorUtility.SetDirty (game);
            if (game.Dirty)
            {
                AssetDatabase.SaveAssets();
                game.Dirty = false;
            }
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
                if (copyBuffer && copyBuffer.GetType() == typeof(State))
                {
                    if (Event.current.keyCode == KeyCode.V)
                    {
                        lastMousePosition = Event.current.mousePosition;
                        State s = CreateState();
                        EditorUtility.CopySerialized((State)copyBuffer, s);

                        List<Path> pathes = new List<Path>(s.pathes);
                        s.pathes.Clear();
                        foreach (Path p in pathes)
                        {
                            Path newPath = s.AddPath();
                            AssetDatabase.AddObjectToAsset(newPath, AssetDatabase.GetAssetPath(game));
                            EditorUtility.CopySerialized(p, newPath);
                            if (!currentChain.states.Contains(p.aimState))
                            {
                                StateLink sl = CreateStateLink();
                                sl.position = new Rect(lastMousePosition.x + sl.position.width / 2, lastMousePosition.y + sl.position.height / 2, sl.position.width, sl.position.height);
                                sl.chain = GuidManager.GetChainByState(p.aimState);
                                sl.state = newPath.aimState;
                            }
                        }
                        s.position = new Rect(lastMousePosition.x - s.position.width / 2, lastMousePosition.y - s.position.height / 2, s.position.width, s.position.height);
                        Repaint();
                    }
                }

                if (copyBuffer && copyBuffer.GetType() == typeof(StateLink))
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

                if (copyBuffer && copyBuffer.GetType() == typeof(Path))
                {
                    if (Event.current.keyCode == KeyCode.V)
                    {
                        Path p = null;
                        if (Selection.activeObject.GetType() == typeof(State))
                        {
                            p = ((State)Selection.activeObject).AddPath();
                            AssetDatabase.AddObjectToAsset(p, AssetDatabase.GetAssetPath(game));
                        }
						p = GuidManager.GetStateByPath((Path)copyBuffer).AddPath();
                        EditorUtility.CopySerialized((Path)copyBuffer, p);
                        if (!currentChain.states.Contains(p.aimState))
                        {
                            StateLink sl = CreateStateLink();
                            sl.position = new Rect(lastMousePosition.x + sl.position.width / 2, lastMousePosition.y + sl.position.height / 2, sl.position.width, sl.position.height);
                            sl.chain = GuidManager.GetChainByState(p.aimState);
                            sl.state = p.aimState;
                        }
                        Repaint();
                    }
                }
            }

            if (chainEditorMode == EditorMode.packs)
            {
                if (copyBuffer && copyBuffer.GetType() == typeof(Param))
                {
                    if (Event.current.keyCode == KeyCode.V)
                    {
                        PasteParam();
                    }
                }

                if (copyBuffer && copyBuffer.GetType() == typeof(Chain))
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

                
                int i = 0;
                c.states.Clear();
                foreach (State s in ((Chain)copyBuffer).states)
                {
                    State newState = c.AddState();
                    AssetDatabase.AddObjectToAsset(newState, AssetDatabase.GetAssetPath(this));
                    PasteStateValues(newState, s, false);
                    i++;
                }

                i = 0;
                c.links.Clear();
                foreach (StateLink s in ((Chain)copyBuffer).links)
                {
                    StateLink newStateLink = c.AddStateLink();
                    AssetDatabase.AddObjectToAsset(newStateLink, AssetDatabase.GetAssetPath(this));
                    PasteStateLinkValues(newStateLink, s, false);
                    i++;
                }



        foreach (State s in c.states)
        {
            foreach (Path p in s.pathes)
            {
                if (((Chain)copyBuffer).states.Contains(p.aimState))
                {
                    p.aimState = c.states[((Chain)copyBuffer).states.IndexOf(p.aimState)];
                }
                else
                {
                    p.aimState = c.links.Find(l=>l.state == p.aimState).state;
                }
            }
        }

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
					menu.AddItem(new GUIContent("Edit"), false, EditChain);
                    menu.AddItem(new GUIContent("Remove"), false, RemoveChain);
                    menu.AddItem(new GUIContent("Copy"), false, CopyChain);
                    menu.AddItem(new GUIContent("Edit"), false, EditChain);
                    if (copyBuffer && copyBuffer.GetType()==typeof(Chain))
                    {
                        menu.AddItem(new GUIContent("Paste values"), false, PasteChainValues);
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
                    if (copyBuffer && copyBuffer.GetType() == typeof(Param))
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

            if (copyBuffer && copyBuffer.GetType() == typeof(Chain))
            {
                menu.AddItem(new GUIContent("Paste chain"), false, PasteChain);
               
            }
            if (copyBuffer && copyBuffer.GetType() == typeof(Param))
            {
                menu.AddItem(new GUIContent("Paste param"), false, PasteParam);
            }
            menu.ShowAsContext();
            return;


        }
    }

    private void EditChain()
    {
        currentChain = menuChain;
        chainEditorMode = EditorMode.chains;
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
        EditorUtility.CopySerialized((Chain)copyBuffer, menuChain);
        int i = 0;
        menuChain.states.Clear();
        foreach (State s in ((Chain)copyBuffer).states)
        {
            State newState = menuChain.AddState();
            AssetDatabase.AddObjectToAsset(newState, AssetDatabase.GetAssetPath(this));
            PasteStateValues(newState, s, false);
            i++;
        }
        i = 0;
        menuChain.links.Clear();
        foreach (StateLink s in ((Chain)copyBuffer).links)
        {
            StateLink newStateLink = menuChain.AddStateLink();
            AssetDatabase.AddObjectToAsset(newStateLink, AssetDatabase.GetAssetPath(this));
            PasteStateLinkValues(newStateLink, s, false);
            i++;
        }
        foreach (State s in menuChain.states)
        {
            foreach (Path p in s.pathes)
            {
                p.aimState = menuChain.states[((Chain)copyBuffer).states.IndexOf(p.aimState)];
            }
        }
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
        if (Selection.activeObject && Selection.activeObject.GetType() == typeof(Chain))
        {
            currentChain = (Chain)Selection.activeObject;
        }

        

        List<string> chainNames = game.chains.Select(x=>x.name).ToList();
        int index = -1;
        if (Selection.activeObject && Selection.activeObject.GetType() == typeof(Chain) && game.chains.Contains((Chain)Selection.activeObject))
        {
            index = game.chains.IndexOf((Chain)Selection.activeObject);
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
                menu.AddItem(new GUIContent("Copy"), false, CopyPath);
                if (copyBuffer && copyBuffer.GetType()==typeof(Path))
                {
                    menu.AddItem(new GUIContent("Paste values"), false, PastePathValues);
                }
                menu.ShowAsContext();
            }
			else if (onState) {
				menuState = onState;
				lastMousePosition = Event.current.mousePosition;
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("Remove state"), false, RemoveState);
				menu.AddItem(new GUIContent("Add path"), false, AddPath);
				menu.AddItem(new GUIContent("Make start"), false, MakeStart);
                menu.AddItem(new GUIContent("Copy"), false, CopyState);
                if (copyBuffer && copyBuffer.GetType() == typeof(Path))
                {
                    menu.AddItem(new GUIContent("Paste path"), false, PastePath);
                }
                if (copyBuffer && copyBuffer.GetType() == typeof(State))
                {
                    menu.AddItem(new GUIContent("Paste values"), false, PasteStateValues);
                }
                menu.ShowAsContext();
			} 
			else if(onStateLink)
			{
				menuStateLink = onStateLink;
				lastMousePosition = Event.current.mousePosition;
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("Remove"), false, RemoveStateLink);
                menu.AddItem(new GUIContent("Copy"), false, CopyStateLink);
                if (copyBuffer && copyBuffer.GetType() == typeof(StateLink))
                {
                    menu.AddItem(new GUIContent("Paste values"), false, PasteStateLinkValues);
                }
                menu.ShowAsContext();
			}

			else
			{
				lastMousePosition = evt.mousePosition;
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("Add state"), false, CreateNewState);
				menu.AddItem(new GUIContent("Add state link"), false, CreateNewStateLink);
                if (copyBuffer && copyBuffer.GetType() == typeof(State))
                {
                    menu.AddItem(new GUIContent("Paste state"), false, PasteState);
                }
                if (copyBuffer && copyBuffer.GetType() == typeof(StateLink))
                {
                    menu.AddItem(new GUIContent("Paste state link"), false, PasteStateLink);
                }
                menu.ShowAsContext();
			}
		}

        if (evt.button == 0 && evt.type == EventType.MouseUp)
        {
            makingPath = false;
        }
    }

    private void PasteStateLink()
    {
        StateLink s = CreateStateLink();
        EditorUtility.CopySerialized((StateLink)copyBuffer, s);
        s.position = new Rect(lastMousePosition.x - s.position.width / 2, lastMousePosition.y - s.position.height / 2, s.position.width, s.position.height);
    }

    private void PasteState()
    {  
        State s = CreateState();
        EditorUtility.CopySerialized((State)copyBuffer, s);

        List<Path> pathes = new List<Path>(s.pathes);
        s.pathes.Clear();

        foreach (Path p in pathes)
        {
            Path newPath = s.AddPath();
            AssetDatabase.AddObjectToAsset(newPath, AssetDatabase.GetAssetPath(game));
            EditorUtility.CopySerialized(p, newPath);
            if (!currentChain.states.Contains(p.aimState))
            {
                StateLink sl = CreateStateLink();
                sl.position = new Rect(lastMousePosition.x + sl.position.width / 2, lastMousePosition.y + sl.position.height / 2, sl.position.width, sl.position.height);
                sl.chain = GuidManager.GetChainByState(p.aimState);
                sl.state = newPath.aimState;
            }
        }

        s.position = new Rect(lastMousePosition.x - s.position.width / 2, lastMousePosition.y - s.position.height / 2, s.position.width, s.position.height);
    }

    private void CopyPath()
    {
        copyBuffer = menuPath;
    }

    private void PasteStateLinkValues()
    {
        PasteStateLinkValues(menuStateLink, (StateLink)copyBuffer);
    }

    private void PasteStateLinkValues(StateLink s, StateLink buffer, bool pastePosition = false)
    {
        Rect p = s.position;
        EditorUtility.CopySerialized(buffer, s);
        if (pastePosition)
        {
            s.position = p;
        }
        Selection.activeObject = s;
    }

    private void CopyStateLink()
    {
        copyBuffer = menuStateLink;
    }

    private void CopyState()
    {
        copyBuffer = menuState;
    }

    private void PastePath()
    {
        Path pastedPath = menuState.AddPath();
        AssetDatabase.AddObjectToAsset(pastedPath, AssetDatabase.GetAssetPath(this));
        EditorUtility.CopySerialized((Path)copyBuffer, pastedPath);
        if (!currentChain.states.Contains(pastedPath.aimState))
        {
            StateLink sl = CreateStateLink();
            sl.position = new Rect(lastMousePosition.x + sl.position.width / 2, lastMousePosition.y + sl.position.height / 2, sl.position.width, sl.position.height);
            sl.chain = GuidManager.GetChainByState(pastedPath.aimState);
            sl.state = pastedPath.aimState;
        }

        Selection.activeObject = pastedPath;
    }

    private void PasteStateValues()
    {
        PasteStateValues(menuState, (State)copyBuffer);
    }

    private void PasteStateValues(State s, State copy, bool pastePosition = false)
    {
        Rect position = s.position;
        EditorUtility.CopySerialized(copy, s);

        List<Path> pathes = new List<Path>(s.pathes);
        s.pathes.Clear();

        foreach (Path p in pathes)
        {
            Path newPath = s.AddPath();
            EditorUtility.CopySerialized(p, newPath);
        }
        if (pastePosition)
        {
            s.position = position;
        }

        if (GuidManager.GetChainByState(copy).StartState == copy)
        {
            GuidManager.GetChainByState(s).StartState = s;
        }
        Selection.activeObject = menuState;
    }

    private void PastePathValues()
    {
        EditorUtility.CopySerialized((Path)copyBuffer, menuPath);

        if (!currentChain.states.Contains(menuPath.aimState))
        {
            StateLink sl = CreateStateLink();
            sl.position = new Rect(lastMousePosition.x + sl.position.width / 2, lastMousePosition.y + sl.position.height / 2, sl.position.width, sl.position.height);
            sl.chain = GuidManager.GetChainByState(menuPath.aimState);
            sl.state = menuPath.aimState;
        }
        Selection.activeObject = menuPath;
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
        if (copyBuffer == deletingParam)
        {
            copyBuffer = null;
        }
		game.parameters.Remove (deletingParam);
		DestroyImmediate (deletingParam, true);
        game.Dirty = true;
        Repaint ();
	}

	public void RemoveChain(Chain deletingChain)
	{
        if (copyBuffer == deletingChain)
        {
            copyBuffer = null;
        }
        game.chains.Remove (deletingChain);
		deletingChain.DestroyChain ();
		DestroyImmediate (deletingChain, true);
        game.Dirty = true;
        Repaint ();
	}

	private void RemovePath(Path p)
	{
        if (copyBuffer == p)
        {
            copyBuffer = null;
        }
        foreach (State s in currentChain.states)
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
        if (copyBuffer == menuState)
        {
            copyBuffer = null;
        }
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

        if (menuState == currentChain.StartState)
        {
            if (currentChain.states.Count == 1)
            {
                currentChain.StartState = currentChain.AddState();
                AssetDatabase.AddObjectToAsset(currentChain.StartState, AssetDatabase.GetAssetPath(this));
            }
            else
            {
                foreach (State s in currentChain.states)
                {
                    if (s != menuState)
                    {
                        currentChain.StartState = s;
                        break;
                    }
                }
            }
        }
        currentChain.RemoveState(menuState);
        game.Dirty = true;
        Repaint ();
	}

	private void RemoveStateLink()
	{
        if (copyBuffer == menuStateLink)
        {
            copyBuffer = null;
        }

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
        game.Dirty = true;
        Repaint ();
	}

	private void AddPath()
	{
		Selection.activeObject = menuState.AddPath();
        AssetDatabase.AddObjectToAsset(Selection.activeObject, AssetDatabase.GetAssetPath(game));
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
		StateLink newStateLink = currentChain.AddStateLink();
        AssetDatabase.AddObjectToAsset(newStateLink, AssetDatabase.GetAssetPath(this));
        newStateLink.position = new Rect (lastMousePosition.x - newStateLink.position.width/2, lastMousePosition.y - newStateLink.position.height/2, newStateLink.position.width, newStateLink.position.height);
        Selection.activeObject = newStateLink;
        return newStateLink;
	}

	State CreateState()
	{
		State newState = currentChain.AddState ();
        AssetDatabase.AddObjectToAsset(newState, AssetDatabase.GetAssetPath(this));
        newState.position = new Rect (lastMousePosition.x - newState.position.width/2, lastMousePosition.y - newState.position.height/2, newState.position.width, newState.position.height);
        Selection.activeObject = newState;
		return newState;
	}

    void CreateNewParam()
    {
        CreateParam();
    }

	Param CreateParam()
	{
		Undo.RecordObject (game, "param creattion");
		Param newParam = CreateInstance<Param> ();
		newParam.paramName = "new param";
		Undo.RegisterCreatedObjectUndo (newParam, "param creation");
		newParam.id = GuidManager.GetItemGUID ();
		AssetDatabase.AddObjectToAsset (newParam, AssetDatabase.GetAssetPath(game));
        game.Dirty = true;
        game.parameters.Add (newParam);
        Selection.activeObject = newParam;
        return newParam;
	}

    private void CreateNewChain()
    {
        CreateChain();
    }

	private Chain CreateChain()
	{
		Undo.RecordObject (game, "chain creattion");
		Chain newChain = CreateInstance<Chain> ();
        AssetDatabase.AddObjectToAsset(newChain, AssetDatabase.GetAssetPath(game));
        newChain.StartState = newChain.AddState();
        AssetDatabase.AddObjectToAsset(newChain.StartState, AssetDatabase.GetAssetPath(this));
        newChain.Init (game);   
        Selection.activeObject = newChain;
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