using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class QuestBookLibrary : Singleton<QuestBookLibrary>
{

    public GameInfo[] fakeGameInfos;
    public int fakeGold;

    private int gold;
    private List<GameInfo> gameInfos = new List<GameInfo>();

    public Action onGoldChanged;
    public Action onBooksChanged;

    private AssetBundle playingBundle;

    public void Start()
    {
        onBooksChanged();
    }

    public int GetGold()
    {
        gold = fakeGold;
        return gold;
    }

    public void ShowLibrary()
    {  
        TryToUnloadeBundle();
        onBooksChanged();
        GetComponent<Animator>().SetBool("Open", false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SetSound(float value)
    {
        PlayerPrefs.SetFloat("soundVolume", value);
        foreach (AudioSource source in FindObjectsOfType<AudioSource>())
        {
            source.volume = value;
        }
    }

    private void OnEnable()
    {
        GetGamesInfosListFromServer();
    }

    public void GetGamesInfosListFromServer()
    {
        //fake
        gold = fakeGold;
        gameInfos = fakeGameInfos.ToList();

        foreach (GameInfo gi in gameInfos)
        {
            if (!Directory.Exists(System.IO.Path.Combine(System.IO.Path.Combine(Application.persistentDataPath, "Books"), gi.name)))
            {
                gi.downloaded = false;
            }
            else
            {
                gi.downloaded = true;
            }
        }
    }

    public List<GameInfo> GetBooksList()
    {
        return gameInfos;
    }

    public void DeleteGameFolder(GameInfo gi)
    {
        Directory.Delete(System.IO.Path.Combine(System.IO.Path.Combine(Application.persistentDataPath, "Books"), gi.name), true);
        gi.downloaded = false;
        onBooksChanged.Invoke();
    }

    public void BuyBook(GameInfo gi)
    {
        //fake
        gi.bought = true;
        onGoldChanged.Invoke();
        onBooksChanged.Invoke();
    }

    public void DownloadBook(GameInfo gi)
    {
        //fake
        string dirrectoryPath = System.IO.Path.Combine(System.IO.Path.Combine(Application.persistentDataPath, "Books"), gi.name);
        Directory.CreateDirectory(dirrectoryPath);
        //create bundle
        NetManager.Instance.DownloadBundle(System.IO.Path.Combine(dirrectoryPath, gi.name));
        gi.downloaded = true;
        onBooksChanged.Invoke();
    }

    public void PlayBook(GameInfo gi)
    {
        HideLibrary();
        TryToUnloadeBundle();
        playingBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(Application.persistentDataPath, "Books"), gi.name), gi.name)+".quest");
        PathGame game = (PathGame)playingBundle.LoadAsset(gi.name, typeof(PathGame));
        GameCanvasController.Instance.PlayBook(game);
    }

    private void TryToUnloadeBundle()
    {
        if (playingBundle != null)
        {
            playingBundle.Unload(true);
            playingBundle = null;
        }
    }

    public void ChangeGold()
    {
        onGoldChanged.Invoke();
    }

    public void HideLibrary()
    {
        GetComponent<Animator>().SetBool("Open", true);
    }

}
