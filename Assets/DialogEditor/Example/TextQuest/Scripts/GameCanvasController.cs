using System;
using UnityEngine;

public class GameCanvasController : Singleton<GameCanvasController>
{
    public void PlayBook(PathGame game)
    {
        GetComponent<Animator>().SetBool("Open", true);
        GetComponent<TextQuesVisualizer>().PlayBook(game);
    }

    public void GoToLibrary()
    {
        //PlayerResource.Instance.SaveDefault();
        GetComponent<Animator>().SetBool("Open", false);
        QuestBookLibrary.Instance.ShowLibrary();
    }
}