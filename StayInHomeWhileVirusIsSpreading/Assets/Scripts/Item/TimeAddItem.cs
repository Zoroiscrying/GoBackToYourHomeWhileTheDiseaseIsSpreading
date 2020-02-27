using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class TimeAddItem : BasicItem
{

    public int BasicTimeAdd = 5;
    public override void ItemWork()
    {
        base.ItemWork();
        
        Gamemanager.Instance.AddTime(BasicTimeAdd + 4 * Gamemanager.Instance.DifficultyLevel, this.transform.position);
    }
}
