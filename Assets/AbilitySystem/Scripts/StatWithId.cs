using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatWithId : ScriptableObject
{
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
