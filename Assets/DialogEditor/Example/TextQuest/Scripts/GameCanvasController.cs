using System;
using UnityEngine;

public class GameCanvasController : Singleton<GameCanvasController>
{
    private PathGame playingBook;
    public bool gameFinished = false;

    public void PlayBook(PathGame game)
    {
        gameFinished = false;
        
        playingBook = game;
        GetComponent<Animator>().SetBool("Open", true);
        GetComponent<TextQuesVisualizer>().PlayBook(game);

        PlayerResource.Instance.Load(game.gameName);
    }

    public void GoToLibrary()
    {
        if (!gameFinished)
        {
            PlayerResource.Instance.Save(playingBook.gameName);
        }
        else
        {
            PlayerResource.Instance.DeleteSave(playingBook.gameName);
        }

        //PlayerResource.Instance.SaveDefault();
        GetComponent<Animator>().SetBool("Open", false);
        QuestBookLibrary.Instance.ShowLibrary();
        foreach (StatsPanel sp in FindObjectsOfType<StatsPanel>())
        {
            sp.Clear();
        }
        playingBook = null;
        ChangerEmmiter.Instance.StopEmmit();
    }

    void OnApplicationQuit()
    {
        if (playingBook)
        {
            PlayerResource.Instance.Save(playingBook.gameName);
        }
    }
}