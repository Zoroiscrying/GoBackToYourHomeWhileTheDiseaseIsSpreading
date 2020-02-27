using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using Random = UnityEngine.Random;

public class WindowPeople : MonoBehaviour,IInteractable
{
    public enum WindowManState
    {
        Active,
        Ready,
        Idle,
        WaitForControl
    }
    
    public float StateChangeTimeMax = 5f;
    public float BasicScore = 100f;
    public Transform WindowTransform;
    private Animator m_windowAnimator;
    
    private StateMachine<WindowManState> fsm;
    private static readonly int kOpenEnd = Animator.StringToHash("OpenEnd");
    private static readonly int kIdle = Animator.StringToHash("Idle");
    private static readonly int kClick = Animator.StringToHash("Click");
    private static readonly int kShakeOver = Animator.StringToHash("ShakeOver");

    private void Awake()
    {
        fsm = StateMachine<WindowManState>.Initialize(this);
    }

    private void Start()
    {
        fsm.ChangeState(WindowManState.WaitForControl);
        m_windowAnimator = WindowTransform.GetComponent<Animator>();
        if (!m_windowAnimator)
        {
            Debug.Log("No Animator!");
        }
        m_windowAnimator.SetBool(kIdle,true);
    }

    private void OnMouseDown()
    {
        if (MouseItemControl.CurrentMouseMode == MouseMode.QItem)
        {
            this.OnInteract();
        }
    }

    public void Activate()
    {
        this.fsm.ChangeState(WindowManState.Idle);
    }

    public void DeActivate()
    {
        this.fsm.ChangeState(WindowManState.WaitForControl);
    }

    #region IdleState

    void Idle_Enter()
    {
        //关窗
        //TODO::IMPLEMENT
        Timer.Register(StateChangeTimeMax * Random.Range(0.5f, 2f), 
            (() => {this.fsm.ChangeState(WindowManState.Ready); }));
        if (!m_windowAnimator)
        {
            m_windowAnimator = WindowTransform.GetComponent<Animator>();
        }
        m_windowAnimator.SetBool(kIdle,true);
    }

    void Idle_Exit()
    {
        m_windowAnimator.SetBool(kIdle,false);
    }

    #endregion
    
    #region ReadyState

    void Ready_Enter()
    {
        //启动？
        //张望
        //TODO::IMPLEMENT
        bool active = Random.Range(0f, 1f) > 0.3f;
        if (active)
        {
            Timer.Register(StateChangeTimeMax * Random.Range(0.6f, 1f), () =>
            {
                fsm.ChangeState(
                    WindowManState.Active);m_windowAnimator.SetTrigger(kShakeOver);
            });
        }
        else
        {
            Timer.Register(StateChangeTimeMax * Random.Range(0.6f, 1f), () => { fsm.ChangeState(
                WindowManState.Idle);});
        }
    }

    void Ready_Update()
    {
        
    }

    void Ready_Exit()
    {
        
    }

    #endregion

    #region ActiveState

    void Active_Enter()
    {
        //打开窗户！
        //TODO::IMPLEMENT
        Timer.Register(StateChangeTimeMax * Random.Range(0.6f, 1f), () => { fsm.ChangeState(
            WindowManState.Idle);});
    }

    void Active_Update()
    {
        
    }

    void Active_Exit()
    {
        m_windowAnimator.SetTrigger(kOpenEnd);
    }

    #endregion
    
    
    public void OnInteract()
    {
        switch (fsm.State)
        {
            case WindowManState.Active:
                //score and animation
                Gamemanager.Instance.AddScore(BasicScore + (Gamemanager.Instance.DifficultyLevel-1) * 50, this.transform.position);
                var particle = Instantiate(VFXManager.Instance.DragSolveParticle, this.transform.position, Quaternion.identity);
                particle.transform.position = new Vector3(transform.position.x,transform.position.y,-1);
                particle.transform.localScale *= 0.6f;
                Timer.Register(0.15f, () => { Destroy(particle); });
                fsm.ChangeState(WindowManState.Idle);
                AudioManager.Instance.PlayHitSound();
                break;
            case WindowManState.Ready:
                //animation no score
                
                break;
            
            case WindowManState.Idle:
                //nothing little animation
                m_windowAnimator.SetTrigger(kClick);
                break;
        }
    }
}
