using UnityEngine;

public struct PlayerInput
{
    public Vector2 Move;

    public Vector2 AimDirection;

    public bool Jump;

    public bool Throw;

    public bool Dash;
}


public class InputHandler : MonoBehaviour
{
    PlayerActions _inputActions;

    void Start()
    {
       
    }



    public PlayerInput GetPlayerInput()
    {
        PlayerActions.GamePlayActions input = _inputActions.GamePlay;

        PlayerInput playerInput = new PlayerInput()
        {
            Move = input.Move.ReadValue<Vector2>(),
            Jump = input.Jump.IsPressed(),
            Throw = input.Throw.IsPressed(),
            Dash = input.Dash.IsPressed(),
            AimDirection = input.AimDirection.ReadValue<Vector2>()
        };

        return playerInput;
    }
}
