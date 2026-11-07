using System;
using Unity.Mathematics;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float groundedMoveSpeed = 14;
    [SerializeField] float groundedAcceleration = 14;
    [SerializeField] float aerialMoveSpeed = 10;
    [SerializeField] float aerialAcceleration = 10;

    [Header("Jump")]
    [SerializeField] float gravity = 5;
    [SerializeField] float jumpForce = 15;
    [SerializeField] float minimalJumpForce = 1;
    [SerializeField] float jumpBufferInSeconds = 1;
    [SerializeField] float hangTimeInSeconds = 0.1f;
    [SerializeField] float hangTimeGravity = 5;

    [Header("Dash")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;


    [Header("Chakram")]

    [SerializeField] float ChakramThrowSpeed;
    

    PlayerInput _input;
    float _jumpBufferTime;
    float _hangTimer;
    [SerializeField] bool _hasJumped;
    float _dashTimer;
    Vector2 _dashDirection;
    float _dashCooldown;

    //velocity
    Vector2 velocity;

    [Header("References")]
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] CollisionChecker _groundCheck;
    [SerializeField] InputHandler _inputHandler;
    [SerializeField] ChakramController chakram;

    // Capture input in the input update loop; physics may run less often.
    void Update()
    {
        _input = _inputHandler.GetPlayerInput();

        if (_input.JumpPressed)
        {
            _jumpBufferTime = jumpBufferInSeconds;
        }

        if (_input.DashPressed)
        {
            if (_dashCooldown <= 0) ExecuteDash();
        }
    }

    //physics here
    void FixedUpdate()
    {
        UpdateValues();

        if (_dashTimer <= 0)
        {
            HorizontalSpeed();
            VerticalSpeed();
            CheckThrow();
        }
        else
        {
            DuringDash();
        }

        ChangeVelocity();
    }

    #region Throw

    void CheckThrow()
    {
        if (_input.Throw)
        {
            chakram.Throw(_input.AimDirection * ChakramThrowSpeed);
        }
    }

    #endregion

    #region Movement

    void UpdateValues()
    {
        _jumpBufferTime = MathF.Max(_jumpBufferTime - Time.fixedDeltaTime, 0);
        _hangTimer = MathF.Max(_hangTimer - Time.fixedDeltaTime, 0);
        _dashTimer = MathF.Max(_dashTimer - Time.fixedDeltaTime, 0);
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

    void ExecuteDash()
    {
        _dashDirection = _input.Move;
        _dashTimer = dashDuration;
        _dashCooldown = dashCooldown + dashDuration;

        _hasJumped = false;
        _hangTimer = 0;
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
        {   //rising
            if (_rb.linearVelocity.y > 0)
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
            //the player is at the peak of its jump or is falling
            else //targetVelocity.y <= 0
            {
                if (_hasJumped)
                {
                    //start hangtime
                    Debug.Log("hangtime");
                    _hangTimer = hangTimeInSeconds;
                    _hasJumped = false;
                }
            }

            if (_dashTimer <= 0)
            {
                //graviy handling 
                var actualGravity = _hangTimer <= 0 ? gravity : hangTimeGravity;
                velocity.y -= actualGravity * Time.fixedDeltaTime;
            }
        }
    }

    void DuringDash()
    {
        velocity = _dashDirection * dashSpeed;
    }

    void ChangeVelocity()
    {
        _rb.linearVelocity = velocity;
    }

    #endregion
}