using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : Singleton<NetManager>
{
    public void DownloadBundle(string bundlePath)
    {
        //StartCoroutine(GetAssetBundle(bundlePath));
    }

    IEnumerator GetAssetBundle(string bookPath)
    {
        string bookName = bookPath.Substring(bookPath.LastIndexOf('/') + 1);

        UnityWebRequest www = UnityWebRequest.GetAssetBundle("https://drive.google.com/open?id=0Bx0fifwbIWEdR1dvNktKSmx4aVU");
        yield return www.Send();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

            byte[] bytes;

            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream memStream = new MemoryStream())
            {
                formatter.Serialize(memStream, bundle);
                bytes = memStream.ToArray();
            }

            FileStream stream = File.Create(bookPath+".quest"); stream.Write(bytes, 0, bytes.Length); stream.Close();
        }
    }

    public List<GameInfo> GetListOfBooks()
    {
        List<GameInfo> booksList = new List<GameInfo>();
        Connect();
        CallOnServer("@GetBooksList");
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

    private void CallOnServer(string comand)
    {
        NetworkStream serverStream = clientSocket.GetStream();
        byte[] outStream = System.Text.Encoding.ASCII.GetBytes(comand + "$");
        serverStream.Write(outStream, 0, outStream.Length);
        serverStream.Flush();
        byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
        serverStream.Read(inStream, 0, inStream.Length);
        string returndata = System.Text.Encoding.ASCII.GetString(inStream);
        msg(returndata);
    }

    public void msg(string mesg)
    {
        Debug.Log(mesg);
        //Disconnect();
        //textBox1.text = mesg;
    }
}