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
    [SerializeField] float gravity = 5;

    [Header("Jump")]
    [SerializeField] float jumpForce = 15;
    [SerializeField] float minimalJumpForce = 1;
    [SerializeField] float jumpBufferInSeconds = 1;
    [SerializeField] float hangTimeInSeconds = 1;

    [Header("Dash")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;

    [Header("References")]
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] CollisionChecker _groundCheck;

    [SerializeField] InputHandler inputHandler;

    PlayerInput _input;

    float _jumpBufferTime;
    float _previousYVelocity;
    float _hangTimer;
    [SerializeField] float _dashTimer;
    [SerializeField] Vector2 _dashDirection;


    //velocity sloppet
    Vector2 _jumpVelocity;
    Vector2 _moveVelocity;
    Vector2 _dashVelocity;


    // Capture input in the input update loop; physics may run less often.
    void Update()
    {
        _input = inputHandler.GetPlayerInput();

        if (_input.JumpPressed)
        {
            _jumpBufferTime = jumpBufferInSeconds;
        }

        if (_input.DashPressed)
        {
            _dashDirection = _input.Move;
            _dashTimer = dashTime;
        }
    }

    //physics here
    void FixedUpdate()
    {
        UpdateValues();

        if (_dashTimer > 0)
        {

            DashHandling();
        }
        else
        {
            HandleMovement();
            GravityHandling();
            JumpHandling();
        }

        ChangeVelocity();
    }

    void UpdateValues()
    {
        _jumpBufferTime = MathF.Max(_jumpBufferTime - Time.fixedDeltaTime, 0);
        _hangTimer = MathF.Max(_hangTimer - Time.fixedDeltaTime, 0);
        _dashTimer = MathF.Max(_dashTimer - Time.fixedDeltaTime, 0);
    }

    void HandleMovement()
    {

        float speed = _groundCheck.IsTouching ? groundedMoveSpeed : aerialMoveSpeed;
        _moveVelocity.x = Mathf.MoveTowards(_moveVelocity.x, _input.Move.x * speed, (_groundCheck.IsTouching ? groundedAcceleration : aerialAcceleration) * Time.fixedDeltaTime);
    }

    void JumpHandling()
    {
        //grounded
        if (_groundCheck.IsTouching)
        {
            //jump if there is buffertime left
            if (_jumpBufferTime > 0)
            {
                _jumpBufferTime = 0;
                _jumpVelocity = new(0, jumpForce);
            }
        }
        //airborne
        else
        {
            if (_rb.linearVelocity.y > 0)
            {
                //allows you to cut your jump short if you realease the jumpbutton
                if (!_input.JumpHeld)
                {
                    _jumpVelocity.y = MathF.Min(minimalJumpForce, _jumpVelocity.y);
                }
            }
            //the player is at the peak of its jump or is falling
            else //targetVelocity.y <= 0
            {
                //the previous frame the velocity was postive so hang time should be activated
                if (_previousYVelocity > 0)
                {
                    //    _hangtimer = hangTimeInSeconds;
                    //   Debug.Log("hang time");

                }
            }
        }

        _previousYVelocity = _jumpVelocity.y;

        if (_hangTimer > 0)
        {
            _jumpVelocity.y = 0;
        }
    }

    void GravityHandling()
    {
        //grounded
        if (_groundCheck.IsTouching)
        {
            _jumpVelocity = new(0, 0);
        }
        //airborne
        else
        {
            _jumpVelocity += Vector2.down * gravity * Time.fixedDeltaTime;
        }
    }

    void DashHandling()
    {
        // the player is dashing
        if (_dashTimer > 0)
        {
            _dashVelocity = _dashDirection * dashSpeed;
        }
    }

    void ChangeVelocity()
    {
        if (_dashTimer > 0)
        {
            _rb.linearVelocity = _dashVelocity;
        }
        else
        {
            _rb.linearVelocity = _jumpVelocity + _moveVelocity;
        }
    }
}