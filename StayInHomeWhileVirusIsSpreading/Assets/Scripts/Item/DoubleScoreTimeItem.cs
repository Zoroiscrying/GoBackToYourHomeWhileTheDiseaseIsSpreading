using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleScoreTimeItem : BasicItem
{

    public float DoubleScoreTime = 8;

    public override void ItemWork()
    {
        base.ItemWork();
        
        GameModeManager.Instance.AddDoubleScoreMode(DoubleScoreTime);
    }
}
