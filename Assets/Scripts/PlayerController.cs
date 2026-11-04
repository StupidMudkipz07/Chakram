using UnityEngine;



public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float groundedMoveSpeed = 14;
    [SerializeField] float aerialMoveSpeed = 10;
    [SerializeField] float gravity = 5;
    [SerializeField] float jumpForce = 15;


    [Header("References")]
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] CollisionChecker _groundCheck;


    PlayerActions _inputActions;
    PlayerActions.GamePlayActions _currrentInput;




    void Start()
    {
        _inputActions = new PlayerActions();
        _inputActions.Enable();
        _currrentInput = _inputActions.GamePlay;
    }

    void OnDestroy()
    {
        _inputActions.Dispose();
    }

    //visuals here only
    void Update() { }

    //physics here
    void FixedUpdate()
    {


        ChangeVelocity();
    }

    void ChangeVelocity()
    {

        var targetVelocity = new Vector2(0, 0f);


        // is grounded
        if (_groundCheck.IsTouching)
        {
            targetVelocity = new Vector2(_currrentInput.Move.ReadValue<Vector2>().x * groundedMoveSpeed, 0f);
        }
        // is airborne
        else
        {
            targetVelocity = new Vector2(_currrentInput.Move.ReadValue<Vector2>().x * groundedMoveSpeed, _rb.linearVelocity.y - gravity);
        }

        if (_currrentInput.Jump.IsPressed())
        {
            targetVelocity.y = jumpForce;
        }

        _rb.linearVelocity = targetVelocity;
    }

}
