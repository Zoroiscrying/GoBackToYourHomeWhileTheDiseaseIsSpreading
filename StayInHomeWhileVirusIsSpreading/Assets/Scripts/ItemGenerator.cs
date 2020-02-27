using System;
using System.Collections;
using System.Collections.Generic;
using MonsterLove.StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemGenerator : Singleton<ItemGenerator>
{
    public int ItemNumberMax = 10;
    private int m_currentGeneratedNum = 0;
    private int m_initItemNumMax;
    public float GenerateIntervalMax = 5f;

    public List<GameObject> Items = new List<GameObject>();
    public List<GameObject> RoadItems = new List<GameObject>();
    
    public enum ItemGenerationStates
    {
        Idle,
        Normal
    }
    
    private StateMachine<ItemGenerationStates> fsm;

    private void Awake()
    {
        fsm = StateMachine<ItemGenerationStates>.Initialize(this);
    }
    
    public void Start()
    {
        m_initItemNumMax = this.ItemNumberMax;
        fsm.ChangeState(ItemGenerationStates.Idle);
    }

    public void GamePaused()
    {
        fsm.ChangeState(ItemGenerationStates.Idle);
    }

    public void GameStart()
    {
        fsm.ChangeState(ItemGenerationStates.Normal);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            this.GenerateItemRandomly(1);
        }
    }

    public void ItemExpired()
    {
        m_currentGeneratedNum--;
    }
    
    public void GenerateItemRandomly(int num)
    {
        for (int i = 0; i < num; i++)
        {
            int randomInd = Random.Range(0, 100) % Items.Count;
            Instantiate(Items[randomInd]);
        }
    }
    
    public void GenerateRoadItemRandomly(int num)
    {
        for (int i = 0; i < num; i++)
        {
            int randomInd = Random.Range(0, 100) % Items.Count;
            Instantiate(RoadItems[randomInd]);
        }
    }

    #region Idle

    

    #endregion

    #region Normal

    private float _generationClock = 0.0f;
    

    void Normal_Enter()
    {
        //normally generating people
    }
    
    void Normal_Update()
    {
        _generationClock += Time.deltaTime;

        //产生的条件--时间到并且当前生成的个体数量要少于最多数量
        if (_generationClock >= GenerateIntervalMax && m_currentGeneratedNum < ItemNumberMax)
        {
            this.ItemNumberMax = m_initItemNumMax + Mathf.RoundToInt(Gamemanager.Instance.DifficultyLevel * 4);
            GenerateItemRandomly(1);
            m_currentGeneratedNum++;
            _generationClock = 0.0f;
            if (Random.Range(0f,1f) > 0.4f)
            {
                GenerateRoadItemRandomly(1);
            }
        }

    }
    
    void Normal_Exit()
    {
        
    }

    #endregion
    
    
    
}
