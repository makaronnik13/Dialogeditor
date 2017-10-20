using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FakeGamePanelInitiation : MonoBehaviour
{
    public enum SortType
    {
        None,
        Increase,
        Decrease
    }

    public bool showBought = true;
    public bool showDownloaded = true;
    public SortType sortByPrice;
    public SortType sortByOld;
    public SortType sortByName;
    public SortType sortByPopularity;
    private string findString = "";

    public void SetBought(Toggle toggle)
    {
        showBought = toggle.isOn;
        Show();
    }

    public void SetDownloaded(Toggle toggle)
    {
        showDownloaded = toggle.isOn;
        Show();
    }

    public void SetPriceSort(SortButton value)
    {
        foreach (SortButton sb in FindObjectsOfType<SortButton>())
        {
            if (sb!=value)
            {
                sb.SortType = SortType.None;
            }
        }
        sortByOld = SortType.None;
        sortByName = SortType.None;
        sortByPrice = value.SortType;
        Show();
    }

    public void SetOldSort(SortButton value)
    {
        foreach (SortButton sb in FindObjectsOfType<SortButton>())
        {
            if (sb != value)
            {
                sb.SortType = SortType.None;
            }
        }
        
        sortByOld = value.SortType;
        sortByName = SortType.None;
        sortByPrice = SortType.None;
        Show();
    }

    public void SetNameSort(SortButton value)
    {
        foreach (SortButton sb in FindObjectsOfType<SortButton>())
        {
            if (sb != value)
            {
                sb.SortType = SortType.None;
            }
        }
        sortByOld = SortType.None;
        sortByName = value.SortType;
        sortByPrice = SortType.None;
        Show();
    }

    public void SetPopularitySort(SortButton value)
    {
        foreach (SortButton sb in FindObjectsOfType<SortButton>())
        {
            if (sb != value)
            {
                sb.SortType = SortType.None;
            }
        }
        sortByOld = SortType.None; 
        sortByName = value.SortType;
        sortByPrice = SortType.None;
        Show();
    }

    private void Awake()
    {
        QuestBookLibrary.Instance.onBooksChanged += Show;
    }

    public void Show()
    {
        List<GameInfo> booksInfos = new List<GameInfo>(QuestBookLibrary.Instance.GetBooksList());


        //Debug.Log("___");
        foreach (GameInfo gi in booksInfos)
        {
          //  Debug.Log(gi.bought);
        }

        if (showBought)
        {
            booksInfos.RemoveAll(book=>book.bought==false);
        }
        if (showDownloaded)
        {
            booksInfos.RemoveAll(book => book.downloaded == false);
        }

        switch (sortByPrice)
        {
            case SortType.Decrease:
                booksInfos = booksInfos.OrderByDescending(game => game.price).ToList();
                break;
            case SortType.Increase:
                booksInfos = booksInfos.OrderBy(game => game.price).ToList();
                break;
        }

        switch (sortByName)
        {
            case SortType.Decrease:
                booksInfos = booksInfos.OrderByDescending(game => game.name).ToList();
                break;
            case SortType.Increase:
                booksInfos = booksInfos.OrderBy(game => game.name).ToList();
                break;
        }

        switch (sortByOld)
        {
            case SortType.Decrease:
                booksInfos = booksInfos.OrderByDescending(game => game.old).ToList();
                break;
            case SortType.Increase:
                booksInfos = booksInfos.OrderBy(game => game.old).ToList();
                break;
        }

        if (findString!="")
        {
            booksInfos = booksInfos.FindAll(book=>book.name.Contains(findString));
        }

        GetComponent<BooksView>().ShowBooks(booksInfos);
    }

    public void OnFindStringChanged(InputField s)
    {
        findString = s.text;
        Show();
    }
}
