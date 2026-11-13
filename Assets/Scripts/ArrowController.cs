using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


public class ArrowController : MonoBehaviour
{
    PlayerInput _input;

    [Header("References")]

    [SerializeField] InputHandler _inputHandler;

    void Update()
    {
        _input = _inputHandler.GetPlayerInput();
    }

    void FixedUpdate()
    {
        // vector2 to degrees
        float angle = Mathf.Atan2(_input.AimDirection.y, _input.AimDirection.x) * Mathf.Rad2Deg;

        // make the angle into quaternion
        var rotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = rotation;
    }
}