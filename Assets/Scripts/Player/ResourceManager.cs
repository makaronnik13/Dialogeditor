using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HoloToolkit.Unity;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;

public class ResourceManager : Singleton<ResourceManager> {

    public delegate void ParamChanging(Param p);
    public event ParamChanging OnParamChanging;

    public PathGame pathGame;
    private List<Param> parameters = new List<Param>();


	public class ParametersList
	{
		 public List<Param> parameters;
	}

    public void SetParam(string name, float value)
    {
        parameters.First(x => x.name == name).PValue = value;
		if (OnParamChanging!=null) 
		{
			OnParamChanging.Invoke(parameters.First(x => x.name == name));
		}
    }

    public void CheckParam(string name)
    {
		if(OnParamChanging!=null)
		{
			OnParamChanging.Invoke(parameters.First(x => x.name == name));
		}
    }

    public void Init(PathGame pathGame)
    {
		//load case
        this.pathGame = pathGame;
            parameters = pathGame.parameters;
            foreach (Param p in parameters)
            {
                p.pValue = 0;  
				if(OnParamChanging!=null)
				{
					OnParamChanging.Invoke(p);
				}
            }
    }

	public void ApplyChanger(List<ParamChanges> changers)
	{
		foreach(ParamChanges pch in changers)
		{
			SetParam(pch.aimParam.name, pch.changeString, pch.Parameters);
		}
	}

    public void SetParam(string name, string evaluationString, List<Param> evaluationParameters = null)
    {

        float value = 0;
        value = ExpressionSolver.CalculateFloat(evaluationString, evaluationParameters);

        parameters.First(x => x.name == name).PValue = value;
		if (OnParamChanging!=null) 
		{
			OnParamChanging.Invoke(parameters.First(x => x.name == name));
		}
    }

    public float GetParam(string name)
    {
        return parameters.First(x => x.name == name).PValue;
    }

	public void Load()
	{
		if(!File.Exists(Application.dataPath+"/Saves/Save1" + ".dgs"))
		{
			Save ();
		}
		BinaryFormatter bformatter = new BinaryFormatter ();
		byte[] data = File.ReadAllBytes(Application.dataPath+"/Saves/Save1" + ".dgs");
		MemoryStream stream = new MemoryStream(data);
		bformatter.Binder = new ClassBinder();
		parameters = ((ParametersList)bformatter.Deserialize(stream)).parameters;
		stream.Close();
	}

	public void Save()
	{
		BinaryFormatter bformatter = new BinaryFormatter ();
		ParametersList pl = new ParametersList ();
		pl.parameters = new List<Param> (parameters);
		if(!Directory.Exists(Application.dataPath+"/Saves"))
		{
			Directory.CreateDirectory (Application.dataPath+"/Saves");
		}
		Stream stream = File.Open(Application.dataPath+"/Saves/Save1" + ".dgs", FileMode.Create);//Открываем поток
		Debug.Log (new ClassBinder());
		bformatter.Binder = new ClassBinder();//Обучаем сериализатор работать с нашим классом         
		bformatter.Serialize(stream, pl);//Cериализуем
		stream.Close();//Закрываем поток 
	}

	void Start()
	{
		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
		Load ();
	}

	void OnApplicationQuit()
	{
		Save ();
	}

	public sealed class ClassBinder : SerializationBinder //
	{
		public override Type BindToType(string assemblyName, string typeName)
		{
			if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
			{
				Type typeToDeserialize = null;
				assemblyName = Assembly.GetExecutingAssembly().FullName;
				typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
				return typeToDeserialize;
			}
			return null;
		}
	}
}
