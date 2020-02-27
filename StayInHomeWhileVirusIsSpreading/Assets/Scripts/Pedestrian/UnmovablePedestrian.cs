using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MonsterLove.StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnmovablePedestrian : BasicMovingObject
{
    public float MaxWaitTime = 5f;
    public float LerpingTime = 0.6f;
    private bool _checkOutsideOfScreen = true;
    private Vector2 _targetPos = Vector2.zero;

    public float MinYPercent = 0.05F;
    public float MaxYPercent = 0.3f;
    


    public enum UnmovablePedMovingType
    {
        Setup,
        Emerging,
        Falling,
        Sliding,
        Teleport
    }
    
    private StateMachine<UnmovablePedMovingType> fsm;

    private void Awake()
    {
        fsm = StateMachine<UnmovablePedMovingType>.Initialize(this);
    }

    private void Expire()
    {
        //消失
        var basicItem = GetComponent<BasicItem>();
        if (basicItem)
        {
            basicItem.Expire();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        fsm.ChangeState(UnmovablePedMovingType.Setup);
        this.StopMoving();
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

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     this.fsm.ChangeState(UnmovablePedMovingType.Sliding);
        // }
    }
    
    
    IEnumerator BlinkSelf(float timeMultiplier, float blinkTime, float firstBlinkInterval)
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
    
    
    #region Setup State

    void Setup_Enter()
    {
        _targetPos = Gamemanager.Instance.RandomPosInsideScreen(0.1f, 0.9f, MinYPercent, MaxYPercent);
        float randomNumber = Random.Range(0, 4);
        switch (randomNumber)
        {
            case 0:
                fsm.ChangeState(UnmovablePedMovingType.Emerging);
                break;
            case 2:
                fsm.ChangeState(UnmovablePedMovingType.Falling);
                break;
            case 1:
                fsm.ChangeState(UnmovablePedMovingType.Sliding);
                break;
            case 3:
                fsm.ChangeState(UnmovablePedMovingType.Sliding);
                break;
            case 4:
                fsm.ChangeState(UnmovablePedMovingType.Teleport);
                break;
            default:
                break;
        }
    }

    #endregion

    #region Emerging

    void Emerging_Enter()
    {
        //从地面出现
        this.transform.position = new Vector3(_targetPos.x,
            Gamemanager.Instance.LeftDownCornerPos.y - Gamemanager.Instance.WorldHeight*0.1f,0);

        this.transform.DOMove(_targetPos, LerpingTime).OnComplete((() =>
        {
            Timer.Register(Mathf.Clamp(MaxWaitTime - Gamemanager.Instance.DifficultyLevel, 2f, MaxWaitTime),
                () => {
                    if (this.gameObject.activeSelf)
                    {
                        StartCoroutine(BlinkSelf(1.0f, 15, 0.15f));    
                    }
                    else
                    {
                        Destroy(this.gameObject);
                    }
                });
        }));
        
        /*
         * () =>
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
            }
         */
    }

    void Emerging_Update()
    {

    }

    #endregion

    #region Falling

    void Falling_Enter()
    {
        //从天空降下
        //从地面出现
        this.transform.position = new Vector3(_targetPos.x,
            Gamemanager.Instance.RightUpCornerPos.y + Gamemanager.Instance.WorldHeight*0.1f,0);

        this.transform.DOMove(_targetPos, LerpingTime).OnComplete((() =>
        {
            Timer.Register(Mathf.Clamp(MaxWaitTime - Gamemanager.Instance.DifficultyLevel, 2f, MaxWaitTime),
                () => {
                    if (this.gameObject.activeSelf)
                    {
                        StartCoroutine(BlinkSelf(1.0f, 12, 0.3f));    
                    }
                    else
                    {
                        Destroy(this.gameObject);
                    }
                });
        }));
    }

    void Falling_Update()
    {
        
    }

    #endregion

    #region Sliding

    void Sliding_Enter()
    {
        bool isAtRightSide = Random.Range(0f, 1f) > 0.5f;
        //从侧面滑出
        if (isAtRightSide)
        {
            this.transform.position = new Vector3(Gamemanager.Instance.RightUpCornerPos.x + Gamemanager.Instance.WorldWidth*0.1f,
                _targetPos.y,0);
        }
        else
        {
            this.transform.position = new Vector3(Gamemanager.Instance.RightUpCornerPos.x - Gamemanager.Instance.WorldWidth*0.1f,
                _targetPos.y,0);
        }

        this.transform.DOMove(_targetPos, LerpingTime).OnComplete((() =>
        {
            Timer.Register(Mathf.Clamp(MaxWaitTime - Gamemanager.Instance.DifficultyLevel, 2f, MaxWaitTime),
                () => {
                    if (this.gameObject.activeSelf)
                    {
                        StartCoroutine(BlinkSelf(1.0f, 12, 0.3f));    
                    }
                    else
                    {
                        Destroy(this.gameObject);
                    }
                });
        }));
    }

    void Sliding_Update()
    {
        
    }

    void Sliding_Exit()
    {
        
    }

    #endregion

    #region Teleport

    void Teleport_Enter()
    {
        //传送出来
        this.transform.position = _targetPos;
        var tempLocalScale = transform.localScale;
        this.transform.localScale = Vector3.one * 0.001f;
        this.transform.DOScale( tempLocalScale, LerpingTime).OnComplete(
            (() =>
            {
                Timer.Register(Mathf.Clamp(MaxWaitTime - Gamemanager.Instance.DifficultyLevel + 1, 2.0f, MaxWaitTime),
                    () => {
                        if (this.gameObject.activeSelf)
                        {
                            StartCoroutine(BlinkSelf(1.0f, 12, 0.3f));    
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
