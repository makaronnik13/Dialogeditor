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

    public void SetParam(StatWithId stat, float value)
    {
        float minValue = 0;
        if (stat.GetType() == typeof(Stat))
        {
            minValue = ((Stat)stat).MinValue;
            if (((Stat)stat).EvaluatedValue)
            {
                Debug.LogWarning("you are trying to midify evaluated stat " + stat.Name);
                return;
            }
        }

		if (!saveInfo.paramsDictionary.ContainsKey(stat.Id))
		{
			saveInfo.paramsDictionary.Add(stat.Id, minValue);
		}

		saveInfo.paramsDictionary [stat.Id] = value;
		onValueChanged.Invoke ();
	}

	public void ChangeParam(StatValue value)
	{
        if (value.stat.EvaluatedValue)
        {
            Debug.LogWarning("you are trying to midify evaluated stat " + value.stat.Name);
            return;
        }
        SetParam(value.stat, GetValue(value.stat)+value.value);
	}

	public void ChangeParam(ModificatorChanger changer)
	{
        if (changer.aimStat.GetType() == typeof(Stat) && ((Stat)changer.aimStat).EvaluatedValue)
        {
            Debug.LogWarning("you are trying to midify evaluated stat " + changer.aimStat.Name);
            return;
        }

        float minValueAim = 0;
        if (changer.aimStat.GetType() == typeof(Stat))
        {
            minValueAim = ((Stat)changer.aimStat).MinValue;
        }

        foreach (StatWithId stat in changer.modificatorStruct.conditionStats)
        {
            float minValue = 0;
            if (changer.aimStat.GetType() == typeof(Stat))
            {
                minValue = ((Stat)changer.aimStat).MinValue;
            }

            if (!saveInfo.paramsDictionary.ContainsKey(stat.Id))
            {
                SetParam(stat, minValue);
            }
        }

		if (!saveInfo.paramsDictionary.ContainsKey(changer.aimStat.Id))
		{
            SetParam(changer.aimStat, minValueAim);
        }

		List<float> values = new List<float>();
		foreach (StatWithId p in changer.modificatorStruct.conditionStats)
		{
            values.Add(GetValue(p));
		}

        SetParam(changer.aimStat, ExpressionSolver.CalculateFloat(changer.modificatorStruct.conditionString, values));
	}

	public bool CheckCondition(ModificatorCondition condition)
	{
		List<float> playerParameters = new List<float>();

		foreach (Stat s in condition.conditionStats)
		{
            playerParameters.Add(GetValue(s));
		}
		return ExpressionSolver.CalculateBool(condition.conditionString, playerParameters);
	}

	public float GetValue(StatWithId s)
	{
        if (s.GetType() == typeof(Stat))
        {
            if (((Stat)s).EvaluatedValue)
            {
                return GetValue(((Stat)s).valueEvaluator);
            }         
        }

        if (!saveInfo.paramsDictionary.ContainsKey(s.Id))
        {
            saveInfo.paramsDictionary.Add(s.Id, ((Stat)s).MinValue);
        }

        return saveInfo.paramsDictionary [s.Id];
	}

	public float GetValue(ModificatorCondition mStruct)
	{
		List<float> values = new List<float> ();
		foreach(Stat s in mStruct.conditionStats)
		{
			values.Add (GetValue(s));
		}
		return ExpressionSolver.CalculateFloat(mStruct.conditionString, values);
	}
}
