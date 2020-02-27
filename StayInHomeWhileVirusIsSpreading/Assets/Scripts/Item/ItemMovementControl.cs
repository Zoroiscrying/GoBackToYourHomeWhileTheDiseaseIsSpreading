using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using Microsoft.Win32;
using MonsterLove.StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;


public class ItemMovementControl : BasicMovingObject
{
    public float MaxMovingSpeed = 10f;
    public float LerpingTime = 0.6f;
    private bool _checkOutsideOfScreen = true;
    public bool RoadItem = false;


    public enum ItemMovingType
    {
        Idle,
        SinMove,
        StraightMove,
        CrossMove,
        Teleport
    }
    
    private StateMachine<ItemMovingType> fsm;

    private void Awake()
    {
        fsm = StateMachine<ItemMovingType>.Initialize(this);
    }


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        fsm.ChangeState(ItemMovingType.Idle);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        
        if (_checkOutsideOfScreen)
        {
            _checkOutsideOfScreen = false;
            if (Gamemanager.Instance.IsOutsideOfScreenFar(this.transform.position))
            {
                Destroy(this.gameObject);
            }
            else
            {
                Timer.Register(6f, (() => { _checkOutsideOfScreen = true; }));
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.fsm.ChangeState(ItemMovingType.Teleport);
        }
    }

    private void SetupPosDirRandomlyCorner()
    {
        bool upCorner = Random.Range(0f, 1f) > 0.5f;
        bool rightCorner = Random.Range(0f, 1f) > 0.5f;
        
        if (upCorner)
        {
            if (rightCorner)
            {
                //up Right
                this.transform.position = Gamemanager.Instance.RightUpCornerPos +
                                          new Vector2(0.1f,0.1f) * new Vector2(Gamemanager.Instance.WorldWidth, 
                                              Gamemanager.Instance.WorldHeight);
                this.ChangeMovingDir(new Vector2(-1f,-1f));
            }
            else
            {
                //up Left
                this.transform.position = Gamemanager.Instance.LeftUpCornerPos +
                                          new Vector2(-0.1f,0.1f) * new Vector2(Gamemanager.Instance.WorldWidth, 
                                              Gamemanager.Instance.WorldHeight);
                this.ChangeMovingDir(new Vector2(1f,-1f));
            }
        }
        else
        {
            if (rightCorner)
            {
                //down Right
                this.transform.position = Gamemanager.Instance.RightDownCornerPos +
                                          new Vector2(0.1f,-0.1f) * new Vector2(Gamemanager.Instance.WorldWidth, 
                                              Gamemanager.Instance.WorldHeight);
                this.ChangeMovingDir(new Vector2(-1f,1f));
            }
            else
            {
                //down left
                this.transform.position = Gamemanager.Instance.LeftDownCornerPos +
                                          new Vector2(-0.1f,-0.1f) * new Vector2(Gamemanager.Instance.WorldWidth, 
                                              Gamemanager.Instance.WorldHeight);
                this.ChangeMovingDir(new Vector2(1f,1f));
            }
        }
    }

    private void SetupPosDirRandomlyFourSided()
    {
        //1.1 0.2 0.8
        bool xPrimary = Random.Range(0f, 1.0f) > 0.5f;
        if (RoadItem)
        {
            xPrimary = true;
        }
        if (xPrimary)
        {
            bool leftSide = Random.Range(0f, 1.0f) > 0.5f;
            this.transform.position = Gamemanager.Instance.RandomPositionXPrimary(
                0.2f, 0.8f, leftSide);
            if (RoadItem)
            {
                this.transform.position = Gamemanager.Instance.RandomPositionXPrimary(0.05f, 0.3f,leftSide);
            }
            if (leftSide)
            {
                this.ChangeMovingDir(new Vector2(1,0));
            }
            else
            {
                this.ChangeMovingDir(new Vector2(-1,0));
            }
        }
        else
        {
            bool upSide = Random.Range(0f, 1.0f) > 0.5f;
            this.transform.position = Gamemanager.Instance.RandomPositionYPrimary(
                0.2f, 0.8f, upSide);
            if (upSide)
            {
                this.ChangeMovingDir(new Vector2(0,-1));
            }
            else
            {
                this.ChangeMovingDir(new Vector2(0,1));
            }
        }
    }
    
    
    #region Idle State

    void Idle_Enter()
    {
        this.ChangeSpeedMag(0f);

        if (RoadItem)
        {
            fsm.ChangeState(ItemMovingType.StraightMove);
        }
        else
        {
            float randomNumber = Random.Range(0, 4);
            switch (randomNumber)
            {
                case 0:
                    fsm.ChangeState(ItemMovingType.SinMove);
                    break;
                case 2:
                    fsm.ChangeState(ItemMovingType.StraightMove);
                    break;
                case 1:
                    fsm.ChangeState(ItemMovingType.CrossMove);
                    break;
                case 3:
                    fsm.ChangeState(ItemMovingType.Teleport);
                    break;
                case 4:
                    fsm.ChangeState(ItemMovingType.StraightMove);
                    break;
                default:
                    break;
            }
        }


    }

    #endregion

    #region SinMove
    
    private float m_sinWaveValue = 0f;
    private float m_pivotValue = 0f;

    void SinMove_Enter()
    {
        this.ChangeSpeedMag((Gamemanager.Instance.DifficultyLevel *2 + MaxMovingSpeed) * Random.Range(0.6f,1f));
        SetupPosDirRandomlyFourSided();
        // Debug.Log("Sin wave moving dir:" + this.MovingDir);
    }

    void SinMove_Update()
    {
        m_sinWaveValue += Time.deltaTime;
        float difficultyLevel = Gamemanager.Instance.DifficultyLevel;
        
        //mag and frequency
        if (Mathf.Abs(this.MovingDir.x) > 0.5f)
        {
            if (Math.Abs(m_pivotValue) < 0.001f)
            {
                m_pivotValue = transform.position.y;
            }
            this.transform.position = new Vector3(transform.position.x, 
                m_pivotValue + difficultyLevel * 2 * Mathf.Sin(m_sinWaveValue * difficultyLevel * 0.4f),0);
        }

        if (Mathf.Abs(this.MovingDir.y)>0.5f)
        {
            if (Math.Abs(m_pivotValue) < 0.001f)
            {
                m_pivotValue = transform.position.x;
            }
            transform.position = new Vector3(m_pivotValue + difficultyLevel * 2 * Mathf.Sin(m_sinWaveValue * difficultyLevel * 0.4f), 
                  transform.position.y,0);
        }
    }

    void SinMove_Exit()
    {
        
    }

    #endregion

    #region StraightMove

    void StraightMove_Enter()
    {
        this.ChangeSpeedMag((Gamemanager.Instance.DifficultyLevel *2 + MaxMovingSpeed) * Random.Range(0.6f,1f));
        SetupPosDirRandomlyFourSided();
    }

    void StraightMove_Update()
    {
        
    }

    void StraightMove_Exit()
    {
        
    }

    #endregion

    #region CrossMove

    void CrossMove_Enter()
    {
        this.ChangeSpeedMag((Gamemanager.Instance.DifficultyLevel *2 + MaxMovingSpeed) * Random.Range(0.6f,1f));
        SetupPosDirRandomlyCorner();
    }

    void CrossMove_Update()
    {
        
    }

    void CrossMove_Exit()
    {
        
    }

    #endregion

    #region Teleport

    private void Expire()
    {
        Destroy(this.gameObject);
    }

    IEnumerator BlinkItem(float timeMultiplier, float blinkTime, float firstBlinkInterval)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        var color = spriteRenderer.color;

        if (!spriteRenderer) yield break;

        float timePercent = 1.1f;
        
        for (int i = 1; i <= blinkTime; i++)
        {
            color = new Color(color.r, color.g, color.b,0);
            spriteRenderer.color = color;

            yield return new WaitForSeconds(firstBlinkInterval * timePercent * timeMultiplier);
                
            color = new Color(color.r, color.g, color.b,255);
            spriteRenderer.color = color;
            yield return new WaitForSeconds(firstBlinkInterval * timePercent * timeMultiplier);

            timePercent -= 1 / blinkTime;
        }
        
        color = new Color(color.r, color.g, color.b,0);

        this.Expire();

    }

    void Teleport_Enter()
    {
        this.ChangeSpeedMag(0f);
        this.transform.position = Gamemanager.Instance.RandomPosInsideScreen(0.2f, 0.8f, 0.2f, 0.8f);
        var tempLocalScale = transform.localScale;
        this.transform.localScale = Vector3.one * 0.001f;
        this.transform.DOScale( tempLocalScale, LerpingTime).OnComplete(
            (() =>
            {
                Timer.Register(Mathf.Clamp(6.0f - Gamemanager.Instance.DifficultyLevel, 2.0f, 6.0f),
                    () => {
                        if (this.gameObject.activeSelf)
                        {
                            StartCoroutine(BlinkItem(1.0f, 12, 0.3f));    
                        }
                        else
                        {
                            Destroy(this.gameObject);
                        }
                    });
            }));
    }

    void Teleport_Update()
    {
        
    }

    void Teleport_Exit()
    {
        
    }

    #endregion


}
