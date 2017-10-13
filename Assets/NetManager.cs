using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : Singleton<NetManager>
{
    public void DownloadBundle(string bundlePath)
    {
        StartCoroutine(GetAssetBundle(bundlePath));
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
}