using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameInfo
{
    public bool bought;
    public bool downloaded;
    public string name;
    public string description;
    public int popularity;
    public float old;
    public int price;
    public string author;
    public Sprite image;

	public GameInfo(string name, string description, int popularity, float old, int price, string author, byte[] image)
	{
		Texture2D texture = new Texture2D (512, 512);
		texture.LoadRawTextureData (image);
		this.image = Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), 0.5f*Vector2.one);
		this.name = name;
		this.description = description;
		this.popularity = popularity;
		this.old = old;
		this.price = price;
		this.author = author;
	}
}
