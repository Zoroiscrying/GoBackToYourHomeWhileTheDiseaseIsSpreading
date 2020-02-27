using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public Text TimeUI;
    public Text ScoreUI;
    public Text PauseScore;
    public Text EndScore;

    public Text DoubleScoreText;

    public GameObject StartBtn;
    public GameObject GamePauseBg;
    public GameObject GamePausePanel;
    public GameObject GameEndedPanel;

    public GameObject GameInstructionPanel;

    public GameObject PauseBtn;
    public GameObject QuitBtn;

    public Slider TimeSlider;
    private int m_maxTime;
    private int m_currentTime;

    public void ChangeTime(int time)
    {
        this.TimeUI.text = "TIME :" + time;
        if (time > m_maxTime)
        {
            m_maxTime = time;
            TimeSlider.maxValue = m_maxTime;
        }

        TimeSlider.value = time;
    }

    public void ChangeScore(int score)
    {
        this.ScoreUI.text = "SCORE:" + score;
        
        
        
        EndScore.text = "SCORE\n"+score;
        PauseScore.text = "SCORE\n"+score;
    }

    public void ShowInstruction()
    {
        GamePauseBg.SetActive(true);
        GameInstructionPanel.SetActive(true);
    }

    public void CloseInstruction()
    {
        GamePauseBg.SetActive(false);
        GameInstructionPanel.SetActive(false);
    }

    public void GameInit()
    {
        ScoreUI.gameObject.SetActive(false);
        TimeSlider.gameObject.SetActive(false);
        StartBtn.SetActive(true);
        QuitBtn.SetActive(true);
        GamePauseBg.SetActive(false);
        GamePausePanel.SetActive(false);
        TimeSlider.gameObject.SetActive(false);
        EndScore.gameObject.SetActive(false);
        GameEndedPanel.SetActive(false);
    }

    public void GamePause()
    {
        ScoreUI.gameObject.SetActive(false);
        GamePausePanel.SetActive(true);
        GamePauseBg.SetActive(true);
        TimeSlider.gameObject.SetActive(false);
        EndScore.gameObject.SetActive(true);
        PauseBtn.SetActive(false);
        QuitBtn.SetActive(false);
    }

    public void GameResumed()
    {
        ScoreUI.gameObject.SetActive(true);
        GamePauseBg.SetActive(false);
        GamePausePanel.SetActive(false);
        TimeSlider.gameObject.SetActive(true);
        EndScore.gameObject.SetActive(false);
        PauseBtn.SetActive(true);
    }

    public void GameEnd()
    {
        ScoreUI.gameObject.SetActive(false);
        GamePauseBg.SetActive(true);
        GameEndedPanel.SetActive(true);
        TimeSlider.gameObject.SetActive(false);
        EndScore.gameObject.SetActive(true);
        PauseBtn.SetActive(false);
        QuitBtn.SetActive(false);
    }

    public void GameStart()
    {
        ScoreUI.gameObject.SetActive(true);
        StartBtn.SetActive(false);
        TimeSlider.gameObject.SetActive(true);
        m_maxTime = Mathf.RoundToInt(Gamemanager.Instance.GameTime);
        PauseBtn.SetActive(true);
        QuitBtn.SetActive(false);
    }

    private void Update()
    {
        if (GameModeManager.Instance.MDoubleScoreMode)
        {
            DoubleScoreText.gameObject.SetActive(true);
            float r = Mathf.PerlinNoise(Gamemanager.Instance.RealTimePassed, 0);
            float g = Mathf.PerlinNoise(0,Gamemanager.Instance.RealTimePassed);
            float b = Mathf.PerlinNoise(Gamemanager.Instance.RealTimePassed, Gamemanager.Instance.RealTimePassed);
            DoubleScoreText.color = new Color(r,g,b);
            float sinValue = Mathf.Sin(Gamemanager.Instance.RealTimePassed);
            float sinValue2 = Mathf.Sin(Gamemanager.Instance.RealTimePassed * 2);
            DoubleScoreText.transform.localScale = (Mathf.Abs(sinValue)+0.5f) * Vector2.one;
            DoubleScoreText.transform.rotation = Quaternion.Euler(new Vector3(0,0,sinValue2*45f));
        }
        
        if (!GameModeManager.Instance.MDoubleScoreMode)
        {
            DoubleScoreText.gameObject.SetActive(false);
        }
    }
}
