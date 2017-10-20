using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using SimpleJSON;
using System.Text;
using System;

public class NetManager : Singleton<NetManager>
{
    public static string NetworkError = "@ConnectionError"; 
    private string filePath;
    private string booksImagesFolderPath, booksFolderPath;

    public int GetMoney()
    {
        if (Online)
        {
            string r = CallOnServer("@Money," + UserName);
            if (r != NetworkError)
            {
                return int.Parse(r);
            }
            else
            {
                if (PlayerPrefs.HasKey("Money"))
                {
                    return PlayerPrefs.GetInt("Money");
                }
                return 0;
            }
        }
        else
        {
            if (PlayerPrefs.HasKey("Money"))
            {
                return PlayerPrefs.GetInt("Money");
            }

            return 0;
        }
    }

    public bool IsPremium()
    {
        int result = int.Parse(CallOnServer("@IsPremium," + UserName));
        if (result!=1)
        {
            return false;
        }
        return true;
    }

    public bool HasAddBlock()
    {
        int result = int.Parse(CallOnServer("@HasAddBlock," + UserName));
        if (result != 1)
        {
            return false;
        }
        return true;
    }

    private string userName;
    public string UserName
    {
        get
        {
            if (userName == null)
            {
                userName = PlayerPrefs.GetString("Username");
            }
            return userName;
        }
        set
        {
            userName = value;
        }
    }

    private void Awake()
    {
        filePath = System.IO.Path.Combine(Application.persistentDataPath, "BooksInfo.txt");

        booksImagesFolderPath = System.IO.Path.Combine(Application.persistentDataPath, "BooksImages");
        booksFolderPath = System.IO.Path.Combine(Application.persistentDataPath, "Books");

        if (!Directory.Exists(booksImagesFolderPath))
        {
            Directory.CreateDirectory(booksImagesFolderPath);
        }
        if (!Directory.Exists(booksFolderPath))
        {
            Directory.CreateDirectory(booksFolderPath);
        }
        
    }

    public int Login(string name, string pass)
    {
        string s = CallOnServer("@Login," + name + "," + pass);
        if (s==NetworkError)
        {
            return 2;
        }
        if (int.Parse(s)==1)
        {
            return 0;
        }
        return 1;
    }

    public Sprite GetImage(string bookName)
    {
        string imagePath = System.IO.Path.Combine(booksImagesFolderPath, bookName+".png");

        Texture2D texture = new Texture2D(64, 64, TextureFormat.ARGB32, false);

        if (File.Exists(imagePath))
        {
            byte[] data = File.ReadAllBytes(imagePath);
            texture.LoadImage(data, true);
        }
        else
        {
            byte[] data = CallOnServerBytes("@GetImage," + bookName);



            texture.LoadImage(data, true);

            var fs = new FileStream(imagePath, FileMode.Create);
            fs.Write(data, 0 , data.Length);
            fs.Dispose();
        }

        Sprite result = Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), Vector2.one*0.5f);
        return result;
    }

    public AssetBundle GetGame(string bookName)
    {
        string folderPath = System.IO.Path.Combine(booksFolderPath, bookName);
        byte[] data = new byte[0];
        string bundlePath = System.IO.Path.Combine(folderPath, bookName + ".pgb");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (File.Exists(bundlePath))
        {
            data = File.ReadAllBytes(bundlePath);
            return AssetBundle.LoadFromMemory(data);
        }
        else
        {
            data = CallOnServerBytes("@DownloadBook," + bookName);
            var fs = new FileStream(bundlePath, FileMode.Create);
            fs.Write(data, 0, data.Length);
            fs.Dispose();
            return null;
        }    
    }
    public string GetListOfBooks()
    {
        if (Online)
        {
            try
            {
                return GetOnlineBooksInfo();
            }
            catch
            {
                return GetOfflineBooksInfo();
            }
        }
        else
        {
            return GetOfflineBooksInfo();
        }
    }

    public string GetOfflineBooksInfo()
    {
        if (!File.Exists(filePath))
        {
            Debug.Log("no connection, no books");
            return "";
        }
        else
        {
            Debug.Log("no connection");
            return File.ReadAllText(filePath);
        }
    }

    public string GetOnlineBooksInfo()
    {
        string recievedString = CallOnServer("@GetBooksList," + UserName);
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }
        File.WriteAllText(filePath, recievedString);
        return recievedString;
    }

    public bool SignIn(string name, string pass)
    {
        string s = CallOnServer("@SignUp," + name + "," + pass);
        if (int.Parse(s) == 1)
        {
            return true;
        }
        return false;
    }

    public void BuyBonuce(int bonuceId)
    {
        CallOnServer("@BuyBonuce," + UserName+","+bonuceId);
        PlayerStats.Instance.UpdateMoney();
    }

    public bool BuyBook(string bookName)
    {
        int result = int.Parse(CallOnServer("@BuyBook," + UserName + "," + bookName));
        if (result == 1)
        {
            PlayerStats.Instance.UpdateMoney();
            QuestBookLibrary.Instance.onBooksChanged.Invoke();
            return true;
        }
        return false;
    }

    TcpClient clientSocket;
    public bool Online;

    private void Disconnect()
    {
        clientSocket.Close();
        //label1.text = "Server Disconnected ...";
        clientSocket = null;
    }

    private void Connect()
    {
        clientSocket = new TcpClient();
        
        clientSocket.Connect("127.0.0.1", 8888);
        //label1.text = "Server Connected ...";
    }

	private string CallOnServer(string comand)
    {
        return Encoding.ASCII.GetString(CallOnServerBytes(comand));
    }

    private byte[] CallOnServerBytes(string comand)
    {
         try
         {  
            Connect();
            NetworkStream serverStream = clientSocket.GetStream();
            byte[] outStream = Encoding.ASCII.GetBytes(comand + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

        
            byte[] inStream = new byte[clientSocket.ReceiveBufferSize*10];
            MemoryStream ms = new MemoryStream();


            int numBytesRead = inStream.Length;

            ms.Write(inStream, 0, numBytesRead);

            serverStream.Read(inStream, 0, inStream.Length);
            Disconnect();
            return inStream;
       }
        catch
        {
            return Encoding.ASCII.GetBytes(NetworkError);
        }
    }
}