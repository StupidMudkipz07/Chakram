using UnityEngine;

public struct PlayerInput
{
    public Vector2 Move;

    public Vector2 AimDirection;

    public bool JumpPressed;

    public bool JumpHeld;

    public bool JumpReleased;

    public bool Throw;

    public bool DashPressed;
}

public class InputHandler : MonoBehaviour
{
    PlayerActions _inputActions;

    [SerializeField] float moveVectorDeadZone = 0.4f;

    [SerializeField] float aimVectorDeadZone = 0.5f;

    void Start()
    {
        _inputActions = new PlayerActions();
        _inputActions.Enable();
    }

    void OnDestroy()
    {
        _inputActions.Dispose();
    }

    public PlayerInput GetPlayerInput()
    {
        PlayerActions.GamePlayActions input = _inputActions.GamePlay;

        PlayerInput playerInput = new PlayerInput()
        {
            Move = input.Move.ReadValue<Vector2>(),

            JumpPressed = input.Jump.WasPressedThisFrame(),
            JumpHeld = input.Jump.IsPressed(),
            JumpReleased = input.Jump.WasReleasedThisFrame(),

            Throw = input.Throw.WasPressedThisFrame(),
            DashPressed = input.Dash.WasPressedThisFrame(),
            AimDirection = input.AimDirection.ReadValue<Vector2>()
        };

        //dont accept too small vectors
        if (playerInput.Move.magnitude < moveVectorDeadZone)
        {
            playerInput.Move = Vector2.zero;
        }

        //dont accept too small vectors
        if (playerInput.AimDirection.magnitude < aimVectorDeadZone)
        {
            playerInput.AimDirection = Vector2.zero;
        }

        return playerInput;
    }
}
