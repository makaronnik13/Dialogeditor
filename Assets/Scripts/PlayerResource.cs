using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayerResource : Singleton<PlayerResource> {


	private Dictionary<int, float> paramsDictionary = new Dictionary<int, float>();

	[ContextMenu("save")]
	public void SaveDefault()
	{
		Save ("save");
	}

	[ContextMenu("load")]
	public void LoadDefault()
	{
		Load ("save");
	}


	public void Load(string fileName)
	{
		if(!Directory.Exists(Application.dataPath+"/saves"))
		{    
			return;
		}
		Stream stream = File.Open(Application.dataPath+"/saves/"+fileName, FileMode.Open);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new PathGameSaveBinder(); 
		paramsDictionary = (Dictionary<int, float>)bformatter.Deserialize(stream);
		stream.Close();
	}

	public void Save(string fileName)
	{
		if(!Directory.Exists(Application.dataPath+"/saves"))
		{    
			//if it doesn't, create it
			Directory.CreateDirectory(Application.dataPath+"/saves");

		}

		if(!File.Exists(Application.dataPath+"/saves/"+fileName))
		{
			File.Create (Application.dataPath+"/saves/"+fileName);
		}
		Stream stream = File.Open(Application.dataPath+"/saves/"+fileName, FileMode.OpenOrCreate);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new PathGameSaveBinder(); 
		Debug.Log ("Writing Information");
		bformatter.Serialize(stream, paramsDictionary);
		stream.Close();
	}

	public void Start()
	{
		DialogPlayer.Instance.onPathGo += new DialogPlayer.PathEventHandler(ChangeParams);
	}

	public void ChangeParams(Path path)
	{
		foreach(ParamChanges pch in path.changes)
		{
			foreach(Param p in pch.Parameters)
			{
				if(!paramsDictionary.ContainsKey(p.id))
				{
					paramsDictionary.Add (p.id, 0);
				}	
			}

			if (!paramsDictionary.ContainsKey (pch.aimParam.id)) {

				paramsDictionary.Add (pch.aimParam.id, 0);
			} 
		}

		foreach(ParamChanges pch in path.changes)
		{
			List<float> values = new List<float> ();
			foreach(Param p in pch.Parameters)
			{
				values.Add (paramsDictionary[p.id]);
			}
			paramsDictionary[pch.aimParam.id] = ExpressionSolver.CalculateFloat (pch.changeString, values);
			Debug.Log (paramsDictionary[pch.aimParam.id]);
		}
	}

	public bool CheckCondition(Condition condition)
	{
		List<float> playerParameters = new List<float> ();

		foreach(Param p in condition.Parameters)
		{
			if(!paramsDictionary.ContainsKey(p.id))
			{
				paramsDictionary.Add (p.id, 0);
			}	
			playerParameters.Add (paramsDictionary[p.id]);
		}

		return ExpressionSolver.CalculateBool (condition.conditionString, playerParameters);
	}
}
