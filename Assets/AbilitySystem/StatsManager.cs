using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Events;

public class StatsManager : Singleton<StatsManager> {

	private Dictionary<Stat, float> stats = new Dictionary<Stat, float> ();

	public UnityEvent onValueChanged;

	public class StatsSaveInfo
	{
		public Dictionary<int, float> paramsDictionary = new Dictionary<int, float>();
	}
	private StatsSaveInfo saveInfo = new StatsSaveInfo();

	[ContextMenu("save")]
	public void SaveDefault()
	{
		Save("saveStats");
	}
	[ContextMenu("load")]
	public void LoadDefault()
	{
		Load("saveStats");
	}
		
	public void Load(string fileName)
	{
		if (!Directory.Exists(Application.dataPath + "/saves"))
		{
			return;
		}
		Stream stream = File.Open(Application.dataPath + "/saves/" + fileName, FileMode.Open);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new PathGameSaveBinder();
		saveInfo = (StatsSaveInfo)bformatter.Deserialize(stream);
		stream.Close();
	}

	public void Save(string fileName)
	{
		if (!Directory.Exists(Application.dataPath + "/saves"))
		{
			Directory.CreateDirectory(Application.dataPath + "/saves");
		}
		if (!File.Exists(Application.dataPath + "/saves/" + fileName))
		{
			File.Create(Application.dataPath + "/saves/" + fileName);
		}
		Stream stream = File.Open(Application.dataPath + "/saves/" + fileName, FileMode.OpenOrCreate);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new PathGameSaveBinder();
		bformatter.Serialize(stream, saveInfo);
		stream.Close();
	}

	public void ChangeParams(Stat stat, float value)
	{
		if (!saveInfo.paramsDictionary.ContainsKey(stat.id))
		{
			saveInfo.paramsDictionary.Add(stat.id, stat.defaultValue);
		}
		saveInfo.paramsDictionary [stat.id] = value;
		onValueChanged.Invoke ();
	}

	public void ChangeParams(StatValue value)
	{
		if (!saveInfo.paramsDictionary.ContainsKey(value.stat.id))
		{
			saveInfo.paramsDictionary.Add(value.stat.id, value.stat.defaultValue);
		}
		Debug.Log (value.value);
		saveInfo.paramsDictionary [value.stat.id] += value.value;
		Debug.Log (value.stat.id);
		Debug.Log (saveInfo.paramsDictionary [value.stat.id]);
		onValueChanged.Invoke ();
	}

	public void ChangeParams(ModificatorChanger changer)
	{
		
		foreach (Stat p in changer.modificatorStruct.stats)
			{
				if (!saveInfo.paramsDictionary.ContainsKey(p.id))
				{
				saveInfo.paramsDictionary.Add(p.id, p.defaultValue);
				}
			}
		if (!saveInfo.paramsDictionary.ContainsKey(changer.aimStat.id))
			{
			saveInfo.paramsDictionary.Add(changer.aimStat.id, changer.aimStat.defaultValue);
			}

			List<float> values = new List<float>();
		foreach (Stat p in changer.modificatorStruct.stats)
			{
				values.Add(saveInfo.paramsDictionary[p.id]);
			}
		saveInfo.paramsDictionary[changer.aimStat.id] = ExpressionSolver.CalculateFloat(changer.modificatorStruct.modificatorString, values);
		onValueChanged.Invoke ();
	}
	public bool CheckCondition(ModificatorCondition condition)
	{
		List<float> playerParameters = new List<float>();

		foreach (Stat s in condition.conditionStats)
		{
			if (!saveInfo.paramsDictionary.ContainsKey(s.id))
			{
				saveInfo.paramsDictionary.Add(s.id, s.defaultValue);
			}
			playerParameters.Add(saveInfo.paramsDictionary[s.id]);
		}
		return ExpressionSolver.CalculateBool(condition.conditionString, playerParameters);
	}

	public float GetValue(Stat s)
	{
		if (!saveInfo.paramsDictionary.ContainsKey(s.id))
		{
			saveInfo.paramsDictionary.Add(s.id, s.defaultValue);
		}
		return saveInfo.paramsDictionary [s.id];
	}

	public float GetValue(ModificatorStruct mStruct)
	{
		List<float> values = new List<float> ();
		foreach(Stat s in mStruct.stats)
		{
			values.Add (GetValue(s));
		}
		return ExpressionSolver.CalculateFloat(mStruct.modificatorString, values);
	}
}
