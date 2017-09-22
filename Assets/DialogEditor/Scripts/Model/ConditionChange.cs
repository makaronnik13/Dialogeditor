﻿using System.Collections.Generic;

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
