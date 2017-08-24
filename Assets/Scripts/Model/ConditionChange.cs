using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConditionChange
{
    public Condition condition;
    public List<ParamChanges> changes;

    public ConditionChange(Condition condition)
    {
        this.condition = condition;
        this.changes = new List<ParamChanges>();
    }
}
