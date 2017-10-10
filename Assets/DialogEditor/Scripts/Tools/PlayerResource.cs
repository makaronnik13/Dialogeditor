using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayerResource : Singleton<PlayerResource>
{
    public class SaveInfo
    {
        public Dictionary<int, float> paramsDictionary = new Dictionary<int, float>();
        public Dictionary<GameObject, Chain> personsChains = new Dictionary<GameObject, Chain>();
    }
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
            pd.personChain = saveInfo.personsChains[pd.gameObject];
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
            saveInfo.personsChains.Add(pd.gameObject, pd.personChain);
        }
        bformatter.Serialize(stream, saveInfo);
        stream.Close();
    }
    public void ChangeParams(Path path)
    {
        foreach (ParamChanges pch in path.changes)
        {
            foreach (Param p in pch.Parameters)
            {
                if (!saveInfo.paramsDictionary.ContainsKey(p.id))
                {
                    saveInfo.paramsDictionary.Add(p.id, 0);
                }
            }
            if (!saveInfo.paramsDictionary.ContainsKey(pch.aimParam.id))
            {

                saveInfo.paramsDictionary.Add(pch.aimParam.id, 0);
            }
        }
        foreach (ParamChanges pch in path.changes)
        {
            List<float> values = new List<float>();
            foreach (Param p in pch.Parameters)
            {
                values.Add(saveInfo.paramsDictionary[p.id]);
            }
            saveInfo.paramsDictionary[pch.aimParam.id] = ExpressionSolver.CalculateFloat(pch.changeString, values);
        }
    }
    public bool CheckCondition(Condition condition)
    {
        List<float> playerParameters = new List<float>();

        foreach (Param p in condition.Parameters)
        {
            if (!saveInfo.paramsDictionary.ContainsKey(p.id))
            {
                saveInfo.paramsDictionary.Add(p.id, 0);
            }
            playerParameters.Add(saveInfo.paramsDictionary[p.id]);
        }
        return ExpressionSolver.CalculateBool(condition.conditionString, playerParameters);
    }
}
