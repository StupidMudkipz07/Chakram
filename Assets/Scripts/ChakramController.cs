using System.Collections;
using System.Threading;
using UnityEngine;


public class ChakramController : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] float catchCooldown;
    [SerializeField] float slowdownCooldown;

    [Header("References")]
    [SerializeField] CollisionChecker PlayerCheck;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] Transform FollowTarget;

    public bool IsConnectedToTarget = true;
    bool _isCatchable = true;
    bool _startSlowdown;

    void Start()
    {

    }

    public void Throw(Vector2 throwDirection)
    {
        _rb.simulated = true;
        IsConnectedToTarget = false;
        _rb.linearVelocity = Vector2.zero;
        _rb.gravityScale = 0;
        _isCatchable = false;

        _rb.AddForce(throwDirection);
        Debug.Log("kast");


        StartCoroutine(ReturnAfterDelay());
        StartCoroutine(StartSlowdownCooldown());
    }

    IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(catchCooldown);

        _isCatchable = true;
    }

    IEnumerator StartSlowdownCooldown()
    {
        yield return new WaitForSeconds(slowdownCooldown);

        _startSlowdown = true;
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
            if (_startSlowdown)
            {
                _rb.gravityScale = 1;
            }
        }
        //following player
        else
        {
            FollowPlayer();
        }
    }
}
