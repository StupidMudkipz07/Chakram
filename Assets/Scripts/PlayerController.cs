using System;
using Unity.Mathematics;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float groundedMoveSpeed = 14;
    [SerializeField] float aerialMoveSpeed = 10;
    [SerializeField] float gravity = 5;
    [SerializeField] float jumpForce = 15;
    [SerializeField] float minimalJumpForce = 1;
    [SerializeField] float jumpBufferInSeconds = 1;


    [Header("References")]
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] CollisionChecker _groundCheck;

    [SerializeField] InputHandler inputHandler;

    PlayerInput _input;
    float _jumpBufferTime;
    float _previousYVelocity;


    // Capture input in the input update loop; physics may run less often.
    void Update()
    {
        _input = inputHandler.GetPlayerInput();

        if (_input.JumpPressed)
        {
            _jumpBufferTime = jumpBufferInSeconds;
        }
    }

    //physics here
    void FixedUpdate()
    {
        UpdateValues();

        ChangeVelocity();
    }

    void UpdateValues()
    {
        _jumpBufferTime = MathF.Max(_jumpBufferTime - Time.fixedDeltaTime, 0);
    }

    void ChangeVelocity()
    {

        Vector2 targetVelocity = Vector2.zero;

        // is grounded
        if (_groundCheck.IsTouching)
        {
            targetVelocity = new Vector2(_input.Move.x * groundedMoveSpeed, 0f);

            //jump if there is buffertime left
            if (_jumpBufferTime > 0)
            {
                _jumpBufferTime = 0;
                targetVelocity.y = jumpForce;
            }
        }

        // is airborne
        else
        {
            targetVelocity = new Vector2(_input.Move.x * aerialMoveSpeed, _rb.linearVelocity.y - gravity);


            //the player is rising
            if (targetVelocity.y > 0)
            {
                //allows you to cut your jump short if you realease the jump
                if (!_input.JumpHeld)
                {
                    targetVelocity.y = MathF.Min(minimalJumpForce, targetVelocity.y);
                }
            }
            //the player is at the peak of its jump or is falling
            else //targetVelocity.y <= 0
            {
                //the previous frame the velocity was postive so hang time should be activated
                if (_previousYVelocity > 0)
                {
                    
                }
            }

        }

        //targetVelocity *= Time.fixedDeltaTime;
        _rb.linearVelocity = targetVelocity;

        _previousYVelocity = targetVelocity.y;
    }
}