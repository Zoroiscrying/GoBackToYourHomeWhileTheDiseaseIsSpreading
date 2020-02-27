using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public enum InteractType
{
    Random,
    Click,
    DragToCircle
}

public class BasicItem : InteractableObject
{
    public MouseMode InteractMod;
    
    [SerializeField]
    private int _clickHp = 1;
    private float _dragCircleRadius = 2.5f;
    private BasicMovingObject _movingScript;
    public InteractType InteractionType = InteractType.Click;
    private GameObject _circlePrefab;
    private Vector2 _worldInteractPosition = Vector2.zero;
    private Vector2 _originalPos = Vector2.zero;
    private bool _isInsideCircle = false;

    public float BasicScore = 100;
    public float DragScoreMultiplier = 2;

    private void Start()
    {
        _clickHp = Mathf.RoundToInt(Random.Range(0.5f, 1f) * (Gamemanager.Instance.DifficultyLevel-1) * 4 + 1);
        _clickHp = 1;

        if (this.InteractionType == InteractType.Random)
        {
            bool click = Random.Range(0f, 1f) > 0.3f;
            if (click)
            {
                this.InteractionType = InteractType.Click;
            }
            else
            {
                InteractionType = InteractType.DragToCircle;
            }
        }
        
        switch (InteractionType)
        {
            case InteractType.Click:
                
                break;
            case InteractType.DragToCircle:
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    public override void OnInteract()
    {
        base.OnInteract();
        if (this.InteractionType == InteractType.Click && MouseItemControl.ModeUseful(this.InteractMod))
        {
            this._clickHp--;
            CameraShakeManager.Instance.SmallImpulse();
                if (Math.Abs(BasicScore) > 0.01f)
                {
                    Gamemanager.Instance.AddScore(BasicScore,this.transform.position);
                }
            
            if (_clickHp <= 0)
            {
                this.ItemWork();
            }
        }
        else if (InteractionType == InteractType.DragToCircle && MouseItemControl.ModeUseful(this.InteractMod))
        {
            this.ItemWork();
        }
    }

    public virtual void ItemWork()
    {
        // Debug.Log("Item work.");
        CameraShakeManager.Instance.HighImpulse();
        switch (InteractionType)
        {
            case InteractType.Click:
                if (Math.Abs(BasicScore) > 0.01f)
                {
                    Gamemanager.Instance.AddScore(BasicScore,this.transform.position);
                }
                break;
            case InteractType.DragToCircle:
                Gamemanager.Instance.AddScore(BasicScore * _clickHp + BasicScore * DragScoreMultiplier,this.transform.position);
                break;
        }
        var particle = Instantiate(VFXManager.Instance.DragSolveParticle, this.transform.position, Quaternion.identity);
        particle.transform.position = new Vector3(transform.position.x,transform.position.y,-1);
        Timer.Register(0.15f, () => { Destroy(particle); });
        
        AudioManager.Instance.PlayHitSound();
        
        this.gameObject.SetActive(false);
        
    }

    private void OnMouseDown()
    {
        if (InteractionType == InteractType.Click)
        {
            this.OnInteract();
        }
        else if (InteractionType == InteractType.DragToCircle && MouseItemControl.ModeUseful(this.InteractMod))
        {
            //范围出现
            ShowDragCircle();
            _originalPos = this.transform.position;
        }
    }

    private void ShowDragCircle()
    {
        if (this._worldInteractPosition == Vector2.zero)
        {
            this._worldInteractPosition = Gamemanager.Instance.RandomPosInsideScreen(0.2f, 0.8f, 0.2f, 0.8f);
        }
        //show drag circle
        if (!_circlePrefab)
        {
            _circlePrefab = new GameObject();
            var spriteRenderer = _circlePrefab.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 3;
            spriteRenderer.sprite = VFXManager.Instance.HospitalSprite;
            _circlePrefab.SetActive(false);
        }
        
        _circlePrefab.SetActive(true);
        _circlePrefab.transform.localScale = 0.001f * Vector3.one;
        _circlePrefab.transform.DOScale(1 * Vector3.one, 0.15f);
        var spriteR = _circlePrefab.GetComponent<SpriteRenderer>();
        spriteR.color = new Color(255,255,255,0);
        spriteR.DOColor(new Color(255, 255, 255, 255), 0.15f);
        _circlePrefab.transform.position = _worldInteractPosition;
    }

    private void DisableDragCircle()
    {
        //Disable drag circle.
        _circlePrefab.SetActive(false);
    }

    public void Expire()
    {
        Destroy(_circlePrefab);
        Destroy(this.gameObject);
    }

    private void OnMouseDrag()
    {
        if (InteractionType == InteractType.DragToCircle && MouseItemControl.ModeUseful(this.InteractMod))
        {
            
            if (!_movingScript)
            {
                _movingScript = GetComponent<BasicMovingObject>();
            }

            if (_movingScript)
            {
                _movingScript.StopMoving();
            }

            if (Camera.main)
            {
                this.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y,0);
            }

            if (Vector2.Distance(this.transform.position, _worldInteractPosition) < _dragCircleRadius)
            {
                this._isInsideCircle = true;
            }
            else
            {
                _isInsideCircle = false;
            }
        }
    }

    private void OnMouseUp()
    {
        if (_isInsideCircle)
        {
            this.OnInteract();
        }
        
        if (this.InteractionType == InteractType.DragToCircle && MouseItemControl.ModeUseful(this.InteractMod))
        {
            this.transform.DOMove(this._originalPos, 0.3f).OnComplete(() => {
                this._movingScript.StartMoving();});
        }

        if (_circlePrefab)
        {
            _circlePrefab.SetActive(false);
        }
        
        _isInsideCircle = false;
    }
}
