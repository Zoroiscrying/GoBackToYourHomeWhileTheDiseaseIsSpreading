using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Gamemanager : Singleton<Gamemanager>
{
    //0 = 界面 1 = 游戏 2 = 暂停
    private bool m_playMode = false;
    private bool m_gamePaused = false;
    private bool m_gameEnded = false;
    public bool GamePaused => m_gamePaused;
    public bool PlayMode => m_playMode;

    public static string MaxGameScore = "MaxScore";
    private float _score = 0;
    private float _time = 90;

    private float _realTimePassed = 0;
    public float RealTimePassed => _realTimePassed;

    public float GameTime => _time;

    private bool _newHighScore = false;
    private Vector2 _leftDownCornerPos;
    private Vector2 _rightUpCornerPos;
    private Vector2 _centerWorldPos;
    private float _scoreMultiplier = 1.0f;

    private float _worldWidth;
    private float _worldHeight;

    public float DifficultyLevel = 1;

    public float WorldWidth => _worldWidth;
    public float WorldHeight => _worldHeight;

    public Vector2 LeftDownCornerPos => _leftDownCornerPos;
    public Vector2 RightUpCornerPos => _rightUpCornerPos;
    
    public Vector2 LeftUpCornerPos => new Vector2(_leftDownCornerPos.x, _rightUpCornerPos.y);

    public Vector2 RightDownCornerPos => new Vector2(_rightUpCornerPos.x, _leftDownCornerPos.y);

    public Transform VirtualCamera;


    // Start is called before the first frame update
    void Awake()
    {
        Gamemanager.Instance.InitGame();
        
        RecalculateCameraValues();
    }

    private void RecalculateCameraValues()
    {
        if (Camera.main)
        {
            _leftDownCornerPos = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
            _rightUpCornerPos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            _centerWorldPos = (_leftDownCornerPos + _rightUpCornerPos) / 2;
            _worldWidth = _rightUpCornerPos.x - _leftDownCornerPos.x;
            _worldHeight = _rightUpCornerPos.y - _leftDownCornerPos.y;
        }
    }

    public void ChangeScoreMultiplier(float multiplier)
    {
        this._scoreMultiplier = multiplier;
    }

    void InitGame()
    {
        GamePause();
        Time.timeScale = 1.0f;
        m_playMode = false;
        m_gamePaused = false;
        _score = 0;
        _time = 90;
        VirtualCamera.position = new Vector3(0,15,-1);
        _realTimePassed = 0.0f;
        DifficultyLevel = 1;
        UIManager.Instance.GameInit();
        PedestrianGenerator.Instance.GamePause();
        WindowsController.Instance.AllDeActive();    
        ItemGenerator.Instance.GamePaused();
    }

    public void StartGame()
    {
        m_playMode = true;
        m_gamePaused = false;
        VirtualCamera.DOMove(new Vector3(0, -1.2f, -1), .6f).SetEase(Ease.OutQuad).OnComplete(RecalculateCameraValues);
        
        UIManager.Instance.GameStart();
        PedestrianGenerator.Instance.GameStart();
        WindowsController.Instance.RegulateActivatedPeople();    
        ItemGenerator.Instance.GameStart();
        AudioManager.Instance.PlayClickSound();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    

    public void ShowAddScore(Vector2 position, float value)
    {
       var newText = Instantiate(VFXManager.Instance.ScoreAddText);
       var textMesh = newText.GetComponentInChildren<TextMesh>();
       textMesh.text = "+" + Mathf.RoundToInt(value);
       textMesh.color = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f));
       newText.transform.position = position;
       // Debug.Log(newText.transform.position);
       Timer.Register(0.3f, (() => Destroy(newText.gameObject)));
    }

    public void ShowAddTime(Vector2 position)
    {
        var newText = Instantiate(VFXManager.Instance.TimeAddText);
        newText.transform.position = position;
        Timer.Register(0.3f, (() => Destroy(newText.gameObject)));
    }

    public void ShowAddDoubleScoreTime(Vector2 position)
    {
        var newText = Instantiate(VFXManager.Instance.DoubleScoreText);
        newText.transform.position = position;
        Timer.Register(0.3f, (() => Destroy(newText.gameObject)));
    }

    public void GamePause()
    {
        m_gamePaused = true;
        //游戏暂停
        Time.timeScale = 0.0f;
        //UI跳出
        //TODO::Implement this.
        
        PedestrianGenerator.Instance.GamePause();
        WindowsController.Instance.AllDeActive();
        ItemGenerator.Instance.GamePaused();
        UIManager.Instance.GamePause();
    }

    public void RestartGame()
    {
        InitGame();
    }

    public void GameResume()
    {
        m_gamePaused = false;
        Time.timeScale = 1.0f;
        
        PedestrianGenerator.Instance.GameStart();
        WindowsController.Instance.RegulateActivatedPeople();
        ItemGenerator.Instance.GameStart();
        UIManager.Instance.GameResumed();
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_playMode)
        {
            _realTimePassed += Time.fixedDeltaTime;
        }
        
        //刷新游戏UI
        //TODO::Implement this.
        if (PlayMode)
        {
            UpdateUI();
            
            _time -= Time.deltaTime;

            DifficultyLevel += Time.deltaTime / 45f;
        }

        if (Mathf.RoundToInt(_realTimePassed) % 60 == 0 && Mathf.RoundToInt(_realTimePassed) > 1f)
        {
            GameModeManager.Instance.RandomMode();
        }

        if (_time <= 0)
        {
            //TODO::Delete This and Implement
            GameEnd(); 
             // Debug.Log("Game Ended.");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (this.GamePaused)
            {
                this.GameResume();
                
            }
            else
            {
                GamePause();
            }

            if (m_gameEnded)
            {
                RestartGame();
            }
        }
    }

    void GameEnd()
    {
        m_gameEnded = true;
        //游戏暂停
        Time.timeScale = 0.0f;
        //游戏结束，刷新最高分
        int maxScoreLastRound = PlayerPrefs.GetInt(Gamemanager.MaxGameScore);
        if (_score > maxScoreLastRound)
        {
            PlayerPrefs.SetInt(Gamemanager.MaxGameScore, Mathf.RoundToInt(_score));
            _newHighScore = true;
        }
        
        UIManager.Instance.GameEnd();
        
        //UI跳出
        //TODO::Implement this.
        if (_newHighScore)
        {
            //
            
        }

    }

    public void AddScore(float score, Vector2 position)
    {
        this._score += score * _scoreMultiplier;
        UpdateUI();
        ShowAddScore(position, score * _scoreMultiplier);
    }
    

    public void AddTime(float time, Vector2 position)
    {
        this._time += time;
        UpdateUI();
        ShowAddTime(position);
    }

    

    public void UpdateUI()
    {
        //score
        UIManager.Instance.ChangeScore(Mathf.RoundToInt(this._score));
        //time
        UIManager.Instance.ChangeTime(Mathf.RoundToInt(this._time));
    }

    public bool IsOutsideOfScreenFar(Vector3 worldPosition)
    {
        float relativeX = Mathf.Abs(worldPosition.x - _centerWorldPos.x) / _worldWidth;
        float relativeY = Mathf.Abs(worldPosition.y - _centerWorldPos.y) / _worldHeight;

        if (relativeX > 0.7f || relativeY > 0.7f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckIfInsideYRange(float worldYPos, float minYPercent, float maxYPercent)
    {
        float percentage = (worldYPos - _leftDownCornerPos.y) / _worldHeight;
        if (minYPercent < percentage && percentage < maxYPercent)
        {
            return true;
        }
        else return false;
    }

    public float ClampWorldYScreenPercent(float worldYPos, float minYPercent, float maxYPercent)
    {
        return Mathf.Clamp(worldYPos, _leftDownCornerPos.y + minYPercent * _worldHeight,
            _leftDownCornerPos.y + maxYPercent * _worldHeight);
    }

    public Vector2 RandomPositionXPrimary(float minPercent, float maxPercent, bool rightSide)
    {
        float percent = Random.Range(minPercent, maxPercent);
        //right side or left side
        if (rightSide)
        {
            return new Vector2(_centerWorldPos.x - _worldWidth * 0.6f, 
                _leftDownCornerPos.y + percent * _worldHeight);
        }
        else
        {
            return new Vector2(_centerWorldPos.x + _worldWidth * 0.6f, 
                _leftDownCornerPos.y + percent * _worldHeight);
        }
    }

    public Vector2 RandomPosInsideScreen(float xMin, float xMax, float yMin, float yMax)
    {
        float xPercent = Random.Range(xMin, xMax);
        float yPercent = Random.Range(yMin, yMax);
        
        return this._leftDownCornerPos + new Vector2(xPercent * _worldWidth, yPercent * _worldHeight);
    }

    public Vector2 RandomPositionYPrimary(float minPercent, float maxPercent, bool upSide)
    {
        float percent = Random.Range(minPercent, maxPercent);
        //up side or down side
        if (upSide)
        {
            return new Vector2(_leftDownCornerPos.x + percent * _worldWidth, 
                _centerWorldPos.y + 0.6f * _worldHeight);
        }
        else
        {
            return new Vector2(_leftDownCornerPos.x + percent * _worldWidth, 
                _centerWorldPos.y - 0.6f * _worldHeight);
        }
    }
    
}
