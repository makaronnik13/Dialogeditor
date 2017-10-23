﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayerResource : Singleton<PlayerResource>
{
    [Serializable]
    public class SaveInfo
    {
        public Dictionary<int, float> paramsDictionary = new Dictionary<int, float>();
        public Dictionary<GameObject, Chain> personsChains = new Dictionary<GameObject, Chain>();
    }

    public Action<Param> onParamchanged;

    private SaveInfo saveInfo = new SaveInfo();

	public Dictionary<Param, float> GetVisibleParams()
	{
		Dictionary<Param, float> result = new Dictionary<Param, float> ();
		foreach(KeyValuePair<int, float> pair in saveInfo.paramsDictionary)
		{
			Param p = GuidManager.GetItemByGuid (pair.Key);
			if(p.showing)
			{
				result.Add (p, pair.Value);
			}
		}
		return result;
	}

    [ContextMenu("save")]
    public void SaveDefault()
    {
        Save("save");
    }
    [ContextMenu("load")]
    public void LoadDefault()
    {
        Load("save");
    }

    public void Start()
    {
        DialogPlayer.Instance.onPathGo += new DialogPlayer.PathEventHandler(ChangeParams);
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
        saveInfo = (SaveInfo)bformatter.Deserialize(stream);
        foreach (PersonDialog pd in FindObjectsOfType<PersonDialog>())
        {
            pd.PersonChain = saveInfo.personsChains[pd.gameObject];
        }
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
        Debug.Log("Writing Information");
        foreach (PersonDialog pd in FindObjectsOfType<PersonDialog>())
        {
            saveInfo.personsChains.Add(pd.gameObject, pd.PersonChain);
        }
        bformatter.Serialize(stream, saveInfo);
        stream.Close();
    }
    public void ChangeParams(Path path)
    {
        foreach (ParamChanges pch in path.changes)
        {
            InitParam(pch.aimParam);
            foreach (Param p in pch.Parameters)
            {
                InitParam(p);
            }
        }
        
        foreach (ParamChanges pch in path.changes)
        {  
			saveInfo.paramsDictionary[pch.aimParam.paramGUID] = CalcParamAfterChange(pch);
            onParamchanged(pch.aimParam);
        }
    }

	public float CalcParamAfterChange(ParamChanges changer)
	{
		List<float> values = new List<float>();

        InitParam(changer.aimParam);

        foreach (Param p in changer.Parameters)
		{
            InitParam(p);
            values.Add(saveInfo.paramsDictionary[p.paramGUID]);
		}
		return  ExpressionSolver.CalculateFloat(changer.changeString, values);
	}

    public float CalcDifference(ParamChanges changer)
    {
		return  CalcParamAfterChange (changer)-saveInfo.paramsDictionary[changer.aimParam.paramGUID];
    }

    public bool CheckCondition(Condition condition)
    {
        List<float> playerParameters = new List<float>();

        foreach (Param p in condition.Parameters)
        {
            InitParam(p);
            playerParameters.Add(saveInfo.paramsDictionary[p.paramGUID]);
        }
        return ExpressionSolver.CalculateBool(condition.conditionString, playerParameters);
    }

	public Dictionary<Param, float> GetParamsDictionary()
	{
		Dictionary<Param, float> dict = new Dictionary<Param, float> ();
		foreach(KeyValuePair<int, float> kvp in saveInfo.paramsDictionary)
		{
			Param p = GuidManager.GetItemByGuid (kvp.Key);
			dict.Add (p, kvp.Value);
		}
		return dict;
	}

    public float GetValue(Param p)
    {
        InitParam(p);
        return saveInfo.paramsDictionary[p.paramGUID];
    }

    private void InitParam(Param p)
    {
        Debug.Log("init "+p.name);
        if (!saveInfo.paramsDictionary.ContainsKey(p.paramGUID))
        {
            saveInfo.paramsDictionary.Add(p.paramGUID, 0);
            onParamchanged(p);
        }
    }
}
