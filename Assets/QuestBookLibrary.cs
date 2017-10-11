using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestBookLibrary : Singleton<QuestBookLibrary>
{

    public GameInfo[] fakeGameInfos;
    public int fakeGold;

    private int gold;
    private List<GameInfo> gameInfos = new List<GameInfo>();

    public Action onGoldChanged;
    public Action onBooksChanged;

    public int GetGold()
    {
        gold = fakeGold;
        return gold;
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
            if (!Directory.Exists(System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath, "Books"), gi.name)))
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
        Directory.Delete(System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath, "Books"), gi.name));
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
        Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath, "Books"), gi.name));
        gi.downloaded = true;
        onBooksChanged.Invoke();
    }

    public void PlayBook(GameInfo gi)
    {
        PlayerPrefs.SetString("currentGame", gi.name);
        SceneManager.LoadSceneAsync(2);
    }

    public void ChangeGold()
    {
        onGoldChanged.Invoke();
    }
}
