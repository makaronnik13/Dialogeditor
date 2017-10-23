using HoloGroup.Threading;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class QuestBookLibrary : Singleton<QuestBookLibrary>
{
    private List<GameInfo> gameInfos = new List<GameInfo>();

    public Action onBooksChanged;

    private AssetBundle playingBundle;

    public void Start()
    {
        onBooksChanged();
    }

    public void ShowLibrary()
    {
        if (playingBundle)
        {
            playingBundle.Unload(true);
        }

        int i = PlayerStats.Instance.Money;
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

                foreach (GameInfo gi in gameInfos)
                {
                    gi.image = nm.GetImage(gi.name);
                }
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
				new GameInfo (
					node ["name"].Value,
					node ["description"].Value,
					node ["popularity"].AsInt,
					node ["old"].AsFloat,
					node ["price"].AsInt,
					node ["author"].Value,
					node ["bought"].AsBool || PlayerStats.Instance.IsPremium
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
        NetManager.Instance.BuyBook(gi.name);
        GetGamesInfosListFromServer();
        onBooksChanged();
    }

    public void DownloadBook(GameInfo gi)
    {
        NetManager.Instance.GetGame(gi.name);
        gi.downloaded = true;
        onBooksChanged.Invoke();
    }

    public void PlayBook(string gi)
    {
        HideLibrary();
		playingBundle = NetManager.Instance.GetGame(gi);
        GameCanvasController.Instance.PlayBook(playingBundle.LoadAsset<PathGame>(gi));
    }


    public void HideLibrary()
    {
        GetComponent<Animator>().SetBool("Open", false);
    }

}
