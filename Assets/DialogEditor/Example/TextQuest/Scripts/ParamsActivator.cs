using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamsActivator : Singleton<ParamsActivator> {

	void Start () {
        PlayerResource.Instance.onParamchanged += CheckParamsActivation;
    }

    private void CheckParamsActivation(Param p)
    {
        if (p.activationType == Param.ActivationType.Auto)
        {  
            if (p.condition.conditionString == "" || PlayerResource.Instance.CheckCondition(p.condition))
            {    
                ActivateParam(p);
            }
        }
    }


    public void ActivateParam(Param p)
    {
        PlayerResource.Instance.ChangeParams(p.changes);
        if (p.activationPath)
        {
            DialogPlayer.Instance.PlayPath(p.activationPath);
        }
    }
}
