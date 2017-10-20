using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour {

    private BookInfoPanel infoPanel;
    private GameInfo gameInfo;

	public void Init(GameInfo gameInfo, BookInfoPanel infoPanel)
    {
        this.gameInfo = gameInfo;
        this.infoPanel = infoPanel;
        transform.GetChild(0).GetComponent<Text>().text = gameInfo.name;
        Button readButton = transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Button>();
        Button BuyButton = transform.GetChild(1).GetChild(0).GetChild(4).GetComponent<Button>();
        Button DownloadButton = transform.GetChild(1).GetChild(0).GetChild(3).GetComponent<Button>();
        Button DeleteButton = transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Button>();
        Button AboutButton = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Button>();

        AboutButton.onClick.AddListener(ShowInfo);
        readButton.onClick.AddListener(Read);
        BuyButton.onClick.AddListener(Buy);
        DownloadButton.onClick.AddListener(Download);
        DeleteButton.onClick.AddListener(Delete);

        BuyButton.GetComponentInChildren<Text>().text = gameInfo.price+"";

        if (!gameInfo.downloaded)
        {
            DownloadButton.gameObject.SetActive(true);
            readButton.gameObject.SetActive(false);
            DeleteButton.gameObject.SetActive(false);
        }
        else
        {
            DownloadButton.gameObject.SetActive(false);
            DeleteButton.gameObject.SetActive(true);
        }

        if (!gameInfo.bought)
        {
            BuyButton.gameObject.SetActive(true);
            DownloadButton.gameObject.SetActive(false);
            readButton.gameObject.SetActive(false);
        }
        else
        {
            BuyButton.gameObject.SetActive(false);
        }


        BuyButton.interactable = gameInfo.price <= PlayerStats.Instance.Money;
        DownloadButton.interactable = NetManager.Instance.Online;
    }

    private void Delete()
    {
        QuestBookLibrary.Instance.DeleteGameFolder(gameInfo);
    }

    private void Download()
    {
        QuestBookLibrary.Instance.DownloadBook(gameInfo);
    }

    private void Buy()
    {
        QuestBookLibrary.Instance.BuyBook(gameInfo);
    }

    private void Read()
    {
        QuestBookLibrary.Instance.PlayBook(gameInfo);
    }

    private void ShowInfo()
    {
        infoPanel.Open(gameInfo);
    }
}
