using System;
using System.Collections;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float groundedMoveSpeed = 10;
    [SerializeField] float groundedAcceleration = 400;
    [SerializeField] float aerialMoveSpeed = 9;
    [SerializeField] float aerialAcceleration = 180;

    [Header("Jump")]
    [SerializeField] float gravity = 50;
    [SerializeField] float jumpForce = 16;
    [SerializeField] float minimalJumpForce = 6;
    [SerializeField] float jumpBufferInSeconds = 0.19f;
    [SerializeField] float hangTimeInSeconds = 0.05f;
    [SerializeField] float hangTimeGravity = -4;
    [SerializeField] float roofSpeedSet = -1;

    [Header("Dash")]
    [SerializeField] float dashSpeed = 27;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 0.56f;


    [Header("Chakram")]
    [SerializeField] float ChakramThrowSpeed;

    [Header("References")]
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] CollisionChecker _groundCheck;
    [SerializeField] CollisionChecker _roofCheck;
    [SerializeField] CollisionChecker _leftWallCheck;
    [SerializeField] CollisionChecker _rightWallCheck;

    [SerializeField] InputHandler _inputHandler;
    [SerializeField] ChakramController chakram;

    [Header("Debug stats")]
    [SerializeField] float _jumpBufferTime;
    [SerializeField] float _hangTimer;
    [SerializeField] bool _hasJumped;
    [SerializeField] Vector2 _dashDirection;
    [SerializeField] float _dashCooldown;
    [SerializeField] PlayerInput _input;
    [SerializeField] bool _isDashing;

    //velocity
    public Vector2 velocity;

    // Capture input in the input update loop, physics may run less often. chatgpt ass comment
    void Update()
    {
        _input = _inputHandler.GetPlayerInput();

        if (_input.JumpPressed)
        {
            _jumpBufferTime = jumpBufferInSeconds;
        }

        if (_input.DashPressed)
        {
            if (_dashCooldown <= 0)
            {
                StartCoroutine(Dash());
            }
        }
        CheckThrow();
    }

    //physics here
    void FixedUpdate()
    {
        UpdateValues();

        if (!_isDashing)
        {//isnt dashing
            HorizontalSpeed();
            VerticalSpeed();
        }

        ChangeVelocity();

        GetWallDirection();
    }

    #region Throw

    void CheckThrow()
    {
        if (_input.Throw && chakram.IsConnectedToTarget)
        {
            chakram.Throw(_input.AimDirection.normalized * ChakramThrowSpeed);
        }
    }

    #endregion

    #region Movement

    void UpdateValues()
    {
        _jumpBufferTime = MathF.Max(_jumpBufferTime - Time.fixedDeltaTime, 0);
        _hangTimer = MathF.Max(_hangTimer - Time.fixedDeltaTime, 0);
        _dashCooldown = MathF.Max(_dashCooldown - Time.fixedDeltaTime, 0);
    }

    void HorizontalSpeed()
    {
        float speed = _groundCheck.IsTouching ? groundedMoveSpeed : aerialMoveSpeed;
        velocity.x = Mathf.MoveTowards(velocity.x, _input.Move.x * speed, (_groundCheck.IsTouching ? groundedAcceleration : aerialAcceleration) * Time.fixedDeltaTime);
    }

    void ExecuteJump()
    {
        _jumpBufferTime = 0;
        velocity.y = jumpForce;
        _hasJumped = true;
    }

    void VerticalSpeed()
    {
        //grounded
        if (_groundCheck.IsTouching)
        {
            velocity.y = 0;

            _hasJumped = false;
            //jump if there is buffertime left
            if (_jumpBufferTime > 0)
            {
                ExecuteJump();
            }
        }
        //airborne
        else
        {
            if (_rb.linearVelocity.y > 0)  //rising
            {
                if (_groundCheck.IsTouching)
                {
                    velocity.y = 0;
                }
                //allows you to cut your jump short if you realease the jumpbutton
                if (!_input.JumpHeld)
                {
                    velocity.y = MathF.Min(minimalJumpForce, velocity.y);
                    //cuts the hangtime so that the player starts falling immedieatly
                    _hasJumped = false;
                }
            }
            else  //the player is at the peak of its jump or is falling
            {
                if (_hasJumped)
                {
                    StartHangTime();
                }
            }

            var actualGravity = _hangTimer <= 0 ? gravity : hangTimeGravity;
            velocity.y -= actualGravity * Time.fixedDeltaTime;
        }

        //touching roof 
        if (_roofCheck.IsTouching)
        {
            velocity.y = roofSpeedSet;
        }
    }

    void StartHangTime()
    {
        //Debug.Log("hangtime");
        _hangTimer = hangTimeInSeconds;
        _hasJumped = false;
    }

    IEnumerator Dash()
    {
        //before dash
        _dashDirection = _input.Move;

        velocity = _dashDirection * dashSpeed;
        _isDashing = true;
        _dashCooldown = dashCooldown + dashDuration;
        _hasJumped = false; //stänger av hangtime, kanske kan vara true

        yield return new WaitForSeconds(dashDuration);

        //after dash
        velocity = Vector2.zero;
        _isDashing = false;

        StartHangTime();
    }

    //this may only be called during fixedUpdate
    void ChangeVelocity()
    {
        _rb.linearVelocity = velocity;
    }

    #region wallCheckSlop

    Vector2 GetWallDirection()
    {
        //maybe fix edge case where to you collide with both colliders at the same time
        if (_leftWallCheck.IsTouching)
        {
            Debug.Log("touching left wall");
        }
        else if (_rightWallCheck.IsTouching)
        {
            Debug.Log("touching right wall");
        }

                                                                
        return Vector2.zero;
    }

    #endregion

    #endregion
}