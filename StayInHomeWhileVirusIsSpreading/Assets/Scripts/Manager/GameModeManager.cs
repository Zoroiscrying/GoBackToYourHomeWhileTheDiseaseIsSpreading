using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameModeManager : Singleton<GameModeManager>
{
    private bool m_gamePausedLastFrame = false;
    
    private float m_normalModeTimer = 0.0f;
    private bool m_normalMode = false;
    //正常模式
    public void AddNormalMode(float time)
    {
        m_normalModeTimer += time;
    }

    private float m_theWorldModeTimer = 0.0f;
    private bool m_theWorldMode = false;
    //The world 模式
    public void AddTheWorldMode(float time)
    {
        m_theWorldModeTimer += time;
    }

    private float m_doubleScoreModeTimer = 0.0f;

    private bool m_doubleScoreMode = false;

    public bool MDoubleScoreMode => m_doubleScoreMode;

    //双倍积分模式
    public void AddDoubleScoreMode(float time)
    {
        m_doubleScoreModeTimer += time;
    }

    private float m_massivePedModeTimer = 0.0f;

    private bool m_massivPedMode = false;
    //大量行人模式
    public void AddMassivePedestrian(float time)
    {
        m_massivePedModeTimer += time;
    }

    private float m_massiveWindowModeTimer = 0.0f;

    private bool m_massiveWindowMode = false;
    //窗户狂魔模式
    public void AddMassiveWindowPeople(float time)
    {
        m_massiveWindowModeTimer += time;
    }
    
    //

    private void CheckingAllMode()
    {
        if (Gamemanager.Instance.GamePaused && !m_gamePausedLastFrame)
        {
            Time.timeScale = 0.0f;
        }
        else if (m_gamePausedLastFrame && !Gamemanager.Instance.GamePaused)
        {
            Time.timeScale = 1.0f;
        }
        m_gamePausedLastFrame = Gamemanager.Instance.GamePaused;

        m_normalModeTimer -= Time.fixedDeltaTime;
        m_doubleScoreModeTimer -= Time.fixedDeltaTime;
        m_massivePedModeTimer -= Time.fixedDeltaTime;
        m_massiveWindowModeTimer -= Time.fixedDeltaTime;
        m_theWorldModeTimer -= Time.fixedDeltaTime;

        if (m_normalModeTimer <= 0.0f)
        {
            m_normalModeTimer = 0.0f;
        }

        if (m_doubleScoreModeTimer <= 0.0f)
        {
            m_doubleScoreModeTimer = 0.0f;
        }

        if (m_massivePedModeTimer <= 0.0f)
        {
            m_massivePedModeTimer = 0.0f;
        }

        if (m_massiveWindowModeTimer <= 0.0f)
        {
            m_massiveWindowModeTimer = 0.0f;
        }

        if (m_theWorldModeTimer <= 0.0f)
        {
            m_theWorldModeTimer = 0.0f;
        }
        
        //massive window
        if (m_massiveWindowMode && m_massiveWindowModeTimer <= 0.0f)
        {
            //time over
            WindowsController.Instance.RegulateActivatedPeople();
            m_massiveWindowMode = false;
        }
        else if(!m_massiveWindowMode && m_massiveWindowModeTimer > 0.0f)
        {
            //massive window time begin
            WindowsController.Instance.AllActive();
            m_massiveWindowMode = true;
        }

        //normal mode
        if (m_normalMode && m_normalModeTimer <= 0.0f)
        {
            //over
            m_normalMode = false;
        }
        else if(!m_normalMode && m_normalModeTimer > 0.0f)
        {
            //begin
            m_normalMode = true;
        }
        
        //massive ped mode
        if (m_massivPedMode && m_massivePedModeTimer <= 0.0f)
        {
            //over
            m_massivPedMode = false;
        }
        else if (!m_massivPedMode && m_massivePedModeTimer > 0.0f)
        {
            //begin
            StartCoroutine(GeneratePedestrians(10 + Mathf.RoundToInt(Gamemanager.Instance.DifficultyLevel * 5)));
            m_massivPedMode = true;
        }
        
        //doubleScore
        if (m_doubleScoreMode && m_doubleScoreModeTimer <= 0.0f)
        {
            //over
            Gamemanager.Instance.ChangeScoreMultiplier(1.0f);
            m_doubleScoreMode = false;
        }
        else if (!m_doubleScoreMode && m_doubleScoreModeTimer > 0.0f)
        {
            //begin
            Gamemanager.Instance.ChangeScoreMultiplier(2.0f);
            m_doubleScoreMode = true;
            Debug.Log("DOUBLE SCORE TIME!!");
        }
        
        //theWorldMode
        if (m_theWorldMode && m_theWorldModeTimer <= 0.0f)
        {
            //over
            Time.timeScale = 0.0f;
            m_theWorldMode = false;
        }
        else if (!m_theWorldMode && m_theWorldModeTimer > 0.0f)
        {
            //begin
            Time.timeScale = 1.0f;
            m_theWorldMode = true;
        }
        
        
    }

    IEnumerator GeneratePedestrians(int num)
    {
        for (int i = 0; i < num; i++)
        {
            PedestrianGenerator.Instance.GeneratePedestrianRandomly(1);
            yield return new WaitForSeconds(0.33f);
        }
    }

    private void FixedUpdate()
    {
        CheckingAllMode();
    }

    public void RandomMode()
    {
        var value = Random.Range(0f, 1f);
        if (value > 0.5f)
        {
            m_massivePedModeTimer += 3.0f;
        }
        else
        {
            m_massiveWindowModeTimer += 10f;
        }
    }
}
