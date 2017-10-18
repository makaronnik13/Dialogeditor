using HoloGroup.Threading;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        GetGamesInfosListFromServer();
        onBooksChanged();
        GetComponent<Animator>().SetBool("Open", true);
    }

    public void Exit()
    {
        HideLibrary();
        LoginCanvasControler.Instance.ShowCanvas();
    }

    public void SetSound(float value)
    {
        PlayerPrefs.SetFloat("soundVolume", value);
        foreach (AudioSource source in FindObjectsOfType<AudioSource>())
        {
            source.volume = value;
        }
    }


    public void GetGamesInfosListFromServer()
    {
        //fake
        gold = fakeGold;

        NetManager nm = NetManager.Instance;

        string gameInfosString = "";

        AsyncManager.Instance.MakeAsync(() => {
            gameInfosString = nm.GetListOfBooks();

            ThreadDispatcher.Instance.InvokeFromMainThread(()=>
            {
                if (gameInfosString!="")
                {
                    gameInfos = StringToBooks(gameInfosString);
                }
                else
                {
                    gameInfos = new List<GameInfo>();
                }

                foreach(GameInfo gi in gameInfos)
                {

                    //gi.image = nm.GetImage(gi.name);

                    if (!Directory.Exists(System.IO.Path.Combine(System.IO.Path.Combine(Application.persistentDataPath, "Books"), gi.name)))
                    {
                        gi.downloaded = false;
                    }
                    else
                    {
                        gi.downloaded = true;
                    }
                }

                FindObjectOfType<FakeGamePanelInitiation>().Show();
            });
        }).Start();
    }

    public List<GameInfo> StringToBooks(string recievedString)
    {
        List<GameInfo> booksList = new List<GameInfo>();

        JSONArray books = JSONArray.Parse(recievedString) as JSONArray;
        foreach (JSONNode node in books)
        {
            booksList.Add(
                new GameInfo(
                    node["name"].Value,
                    node["description"].Value,
                    node["popularity"].AsInt,
                    node["old"].AsFloat,
                    node["price"].AsInt,
                    node["author"].Value
                )
            );
        }
        return booksList;
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
        NetManager.Instance.DownloadBundle(dirrectoryPath, gi.name);
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
        GetComponent<Animator>().SetBool("Open", false);
    }

}
