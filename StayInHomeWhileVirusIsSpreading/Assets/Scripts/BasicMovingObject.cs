using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovingObject : MonoBehaviour,IMovable
{
    private float _speedMag = 5.0f;
    private Vector2 _movingDir = Vector2.right;
    private Vector2 _velocity;
    public bool NeverMove = false;
    protected bool _canMove = true;
    private bool _reverseMove = false;
    private bool _movingRight = false;

    public Vector2 MovingDir => _movingDir;
    public float SpeedMag => _speedMag;

    public void ReverseSprite()
    {
        this.transform.Rotate(Vector3.up,180f);
    }

    public void CheckMovingDirRotation()
    {
        if (_movingDir.x > 0 && !_movingRight)
        {
            _movingRight = true;
            ReverseSprite();
        }
        else if (_movingDir.x < 0 && _movingRight)
        {
            _movingRight = false;
            ReverseSprite();
        }
    }
    
    public void ChangeSpeedMag(float speedMag)
    {
        this._speedMag = speedMag;
    }

    public void ChangeMovingDir(Vector2 movingDir)
    {
        this._movingDir = movingDir.normalized;
        CheckMovingDirRotation();
    }
    
    // Start is called before the first frame update
    public virtual void Start()
    {
        StartMoving();
    }

    public void ReverseMovement()
    {
        this._reverseMove = !_reverseMove;
    }

    public void StopMoving()
    {
        this._canMove = false;
    }

    public void StartMoving()
    {
        if (NeverMove)
        {
            this._canMove = false;
            return;
        }
        this._canMove = true;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        CalculateSpeed();
        Move();
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     this.ReverseMovement();
        // }
    }

    public void CalculateSpeed()
    {
        this._velocity = _speedMag * _movingDir.normalized;
    }

    public void Move()
    {
        if (_canMove)
        {
            if (!_reverseMove)
            {
                this.transform.position += new Vector3(_velocity.x, _velocity.y, 0)*Time.deltaTime;
            }
            else
            {
                this.transform.position -= new Vector3(_velocity.x, _velocity.y, 0)*Time.deltaTime;
            }
        }
    }
}
