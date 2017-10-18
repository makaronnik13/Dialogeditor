using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Text;

public class NetManager : Singleton<NetManager>
{
	public void DownloadBundle(string bundlePath, string bundleName)
    {
		System.IO.Path.Combine(dirrectoryPath, gi.name)
		string bundleString = CallOnServer ("@GetBundle"+bundlePath);
    }

    public List<GameInfo> GetListOfBooks()
    {
		string filePath = System.IO.Path.Combine(Application.persistentDataPath, "BooksInfo.txt");

		try
		{
	        Connect();
			string recievedString = CallOnServer ("@GetBooksList");
			if(!File.Exists(filePath))
			{
				File.Create (filePath);
			}
			File.WriteAllText (filePath, recievedString);

				return StringToBooks (recievedString);
			}
		catch
		{
			if (!File.Exists (filePath)) 
			{
				Debug.Log ("no connection, no books");
				return new List<GameInfo> ();
			} else 
			{
				Debug.Log ("тщ сщттусешщт");
				return StringToBooks (File.ReadAllText(filePath));	
			}
		}
    }

	public List<GameInfo> StringToBooks(string recievedString)
	{
		List<GameInfo> booksList = new List<GameInfo>();

		JSONArray books = JSONArray.Parse(recievedString) as JSONArray;
		foreach(JSONNode node in books)
		{
			booksList.Add (
				new GameInfo(
					node["name"].ToString(), 
					node["description"].ToString(), 
					node["popularity"].AsInt, 
					node["old"].AsFloat, 
					node["price"].AsInt, 
					node["author"].ToString(), 
					Encoding.ASCII.GetBytes(node["image"].ToString())
				)
			);
		}
		return booksList;
	}

    TcpClient clientSocket;

    private void Disconnect()
    {
        clientSocket.Close();
        //label1.text = "Server Disconnected ...";
        clientSocket = null;
    }

    private void Connect()
    {
        clientSocket = new System.Net.Sockets.TcpClient();
        clientSocket.Connect("127.0.0.1", 8888);
        //label1.text = "Server Connected ...";
    }

	private string CallOnServer(string comand)
    {
        NetworkStream serverStream = clientSocket.GetStream();
        byte[] outStream = System.Text.Encoding.ASCII.GetBytes(comand + "$");
        serverStream.Write(outStream, 0, outStream.Length);
        serverStream.Flush();
        byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
        serverStream.Read(inStream, 0, inStream.Length);
		return System.Text.Encoding.ASCII.GetString(inStream);
    }
}