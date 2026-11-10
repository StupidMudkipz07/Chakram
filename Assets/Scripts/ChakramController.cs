using System.Collections;
using System.Threading;
using UnityEngine;


public class ChakramController : MonoBehaviour
{
    [SerializeField] CollisionChecker PlayerCheck;
    [SerializeField] Rigidbody2D _rb;

    [SerializeField] Transform FollowTarget;
    [SerializeField] float catchCooldown;

    [SerializeField] public bool IsConnectedToTarget = true;
    [SerializeField] bool _isCatchable = true;


    Coroutine returnCoroutine;

    void Start()
    {

    }

    public void Throw(Vector2 throwDirection)
    {
        _rb.simulated = true;
        IsConnectedToTarget = false;
        //_rb.AddForce(throwDirection);
        Debug.Log("kast");
        _isCatchable = false;

        returnCoroutine = StartCoroutine(ReturnAfterDelay());
    }

    IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(catchCooldown);

        _isCatchable = true;
    }

    void FollowPlayer()
    {
        transform.position = FollowTarget.position;
    }

    void Update()
    {
        //touched player
        if (PlayerCheck.IsTouching && _isCatchable)
        {
            _rb.simulated = false;
            IsConnectedToTarget = true;
            Debug.Log("nuddade SPelaren");
        }
        // slop
        if (!IsConnectedToTarget)
        {

        }
        //following player
        else
        {
            FollowPlayer();
        }
    }
}
