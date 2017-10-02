using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "RPG/Stat")]
[System.Serializable]
public class Stat : StatWithId
{
    public float MaxValue
    {
        get
        {
            return  StatsManager.Instance.GetValue(maxEvaluator);
        }
    }
    public float MinValue
    {
        get
        {
            return StatsManager.Instance.GetValue(minEvaluator);
        }
    }
    public ModificatorCondition valueEvaluator;
    public ModificatorCondition minEvaluator;
    public ModificatorCondition maxEvaluator;
    public bool EvaluatedValue;
}
