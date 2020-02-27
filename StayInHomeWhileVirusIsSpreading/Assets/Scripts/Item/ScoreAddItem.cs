using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreAddItem : BasicItem
{
    
    public float Score = 400;
    public override void ItemWork()
    {
        base.ItemWork();
        
        Gamemanager.Instance.AddScore(Score * Gamemanager.Instance.DifficultyLevel, this.transform.position);
        
        
    }
}
