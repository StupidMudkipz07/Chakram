using UnityEngine;

public class ChakramController : MonoBehaviour
{
    [SerializeField] CollisionChecker PlayerCheck;
    [SerializeField] Rigidbody2D _rb;

    [SerializeField]  bool _isDisconnectedFromTarget;


    void Start()
    {

    }

    public void Throw(Vector2 throwDirection)
    {
        _isDisconnectedFromTarget = false;
        _rb.AddForce(throwDirection * 1000);
        Debug.Log("kast");
    }

    void Update()
    {
        //touched player
        if (PlayerCheck.IsTouching)
        {
            _isDisconnectedFromTarget = true;
            Debug.Log("nuddade SPelaren");
        }

        if (_isDisconnectedFromTarget)
        {

        }
        //following player
        else
        {
            
        }
    }
}
