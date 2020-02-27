using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using Random = UnityEngine.Random;

public class SimplePedestrian : BasicMovingObject
{
    public float MaxMovingSpeed = 3f;
    public float StateChangeTimeMax = 5f;
    public float MaxYPercentage = 0.05f;
    public float MinYPercentage = 0.3f;
    private bool _checkOutsideOfScreen = true;
    private Animator _animator;
    
    public enum PedestrianState
    {
        Setup,
        Move,
        Idle
    }
    
    private StateMachine<PedestrianState> fsm;

    private void Awake()
    {
        fsm = StateMachine<PedestrianState>.Initialize(this);
    }

    // Start is called before the first frame update

    public override void Start()
    {
        base.Start();
        fsm.ChangeState(PedestrianState.Setup);
        _animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (_canMove)
        {
            this.transform.position =
                new Vector3(this.transform.position.x,
                    Gamemanager.Instance.ClampWorldYScreenPercent(this.transform.position.y, MinYPercentage,
                        MaxYPercentage), 0);
        }
        
        if (_checkOutsideOfScreen)
        {
            _checkOutsideOfScreen = false;
            if (Gamemanager.Instance.IsOutsideOfScreenFar(this.transform.position))
            {
                Expire();
            }
            else
            {
                Timer.Register(6f, (() => { _checkOutsideOfScreen = true; }));
            }
        }

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     this.ChooseStartPosDirRandomly();
        // }
        
        // CheckMovingDirRotation();
    }

    void Solved()
    {
        //加分
        //动画
        //destroy延迟
    }

    void Expire()
    {
        PedestrianGenerator.Instance.PedestrianExpired();
    }

    void RandomChangeToMoveOrIdleState()
    {
        var toMoveState = Random.Range(0f, 1f) > 0.5f;
        if (toMoveState)
        {
            if (fsm.State == PedestrianState.Move)
            {
                Timer.Register(this.StateChangeTimeMax * Random.Range(0.6f, 1f),
                    RandomChangeToMoveOrIdleState);
            }
            else
            {
                Timer.Register(this.StateChangeTimeMax * Random.Range(0.2f, 0.5f),
                    () => { fsm.ChangeState(PedestrianState.Move); });
            }
        }
        else
        {
            if (fsm.State == PedestrianState.Idle)
            {
                Timer.Register(this.StateChangeTimeMax * Random.Range(0.2f, 0.5f),
                    RandomChangeToMoveOrIdleState);
            }
            else
            {
                Timer.Register(this.StateChangeTimeMax * Random.Range(0.6f, 1f),
                    () => {this.fsm.ChangeState(PedestrianState.Idle); });
            }
        }
    }

    void RandomChangeMovingDir()
    {
        var reverseXDir = Random.Range(0f, 1f) > 0.85f;
        if (reverseXDir)
        {
            this.ChangeMovingDir(new Vector2(-this.MovingDir.x,this.MovingDir.y));
        }
        
        var ySpeed = Random.Range(0f, 1.2f);
        
        if (ySpeed < 0.4f)
        {
            this.ChangeMovingDir(new Vector2(this.MovingDir.x,-this.MovingDir.x * 0.2f));
        }
        else if (ySpeed < 0.8f)
        {
            this.ChangeMovingDir(new Vector2(this.MovingDir.x, 0f));
        }
        else
        {
            this.ChangeMovingDir(new Vector2(this.MovingDir.x,this.MovingDir.x * 0.2f));
        }

    }

    void ChooseStartPosDirRandomly()
    {
        bool movingRight = Random.Range(0f, 1f) > 0.5f;
        this.transform.position = Gamemanager.Instance.RandomPositionXPrimary(MinYPercentage, MaxYPercentage, movingRight);
        if (movingRight)
        {
            this.ChangeMovingDir(new Vector2(1,0));
            // this.ReverseSprite();
        }
        else
        {
            this.ChangeMovingDir(new Vector2(-1,0));
        }
        // Debug.Log("Randomized Position:" + this.transform.position);
    }
    
    #region Move State

    private float m_movingClock = 0.0f;
    private float m_checkMoveDirTime = 5.0f;
    
    void Move_Enter()
    {
        // Debug.Log("Start Moving.");
        this.ChangeSpeedMag(this.MaxMovingSpeed * Random.Range(0.6f,1f) +  Gamemanager.Instance.DifficultyLevel);
        RandomChangeToMoveOrIdleState();
        // RandomChangeMovingDir();
        m_checkMoveDirTime = Mathf.Clamp(5.0f * Random.Range(0.8f, 1.2f) - Gamemanager.Instance.DifficultyLevel,
            2.0f, 6f);
        if (!_animator)
        {
            _animator = GetComponent<Animator>();
        }

        if (_animator)
        {
            _animator.SetBool("Moving",true);
        }
    }

    void Move_Update()
    {
        m_movingClock += Time.deltaTime;

        if (m_movingClock > m_checkMoveDirTime)
        {
            RandomChangeMovingDir();
            m_movingClock = 0.0f;
        }
        
    }

    void Move_Exit()
    {
        
    }
        
    #endregion

    #region Idle State

    void Idle_Enter()
    {
        // Debug.Log("Stop Moving.");
        this.ChangeSpeedMag(0f);
        RandomChangeToMoveOrIdleState();
        RandomChangeMovingDir();
        //动画
        //TODO::IMPLEMENT
        _animator.SetBool("Moving",false);
    }

    void Idle_Update()
    {
        
    }

    #endregion

    #region Setup

    void Setup_Enter()
    {
        _animator = this.GetComponent<Animator>();
        ChooseStartPosDirRandomly();
        // Debug.Log("Setup Complete.");
        fsm.ChangeState(PedestrianState.Move);
    }

    #endregion
}
