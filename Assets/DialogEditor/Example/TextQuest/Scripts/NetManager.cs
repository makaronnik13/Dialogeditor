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
    private string booksImagesFolderPath;
    public string UserName = "user1";

    private void Awake()
    {
        filePath = System.IO.Path.Combine(Application.persistentDataPath, "BooksInfo.txt");
        if (!Directory.Exists(System.IO.Path.Combine(Application.persistentDataPath, "BooksImages")))
        {
            Directory.CreateDirectory(System.IO.Path.Combine(Application.persistentDataPath, "BooksImages"));
        }
        booksImagesFolderPath = System.IO.Path.Combine(Application.persistentDataPath, "BooksImages");
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

    public void DownloadBundle(string bundlePath, string bundleName)
    {
		//System.IO.Path.Combine(dirrectoryPath, gi.name)
		string bundleString = CallOnServer ("@GetBundle"+bundlePath);
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
            //File.Create(imagePath).Dispose();
            //File.WriteAllBytes(imagePath, data);
        }

        Sprite result = new Sprite();//Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), Vector2.one*0.5f);
        return result;
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
            byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
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