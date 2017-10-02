using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StatWithId : ScriptableObject
{
	public string Name{
		get
		{
			return name;
		}
		set
		{
			name = value;
			#if UNITY_EDITOR
			string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
			AssetDatabase.RenameAsset(assetPath, assetPath.Replace(assetPath, value));
			#endif
		}
	}

    public int id = -1;
    public int Id
    {
        get
        {
            if (id == -1)
            {
                id = GuidManager.GetStatGuid();
            }
            return id;
        }
    }

    public ModificatorCondition description;
}
