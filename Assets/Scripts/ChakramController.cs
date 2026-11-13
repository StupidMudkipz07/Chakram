using System;
using System.Collections;
using System.Threading;
using UnityEngine;


public class ChakramController : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] float catchCooldown;
    [SerializeField] float slowdownCooldown;
    [SerializeField] float slowdownAmount = 5;
    [SerializeField] float setGravity = 1;

    [Header("References")]
    [SerializeField] CollisionChecker PlayerCheck;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] Transform FollowTarget;

    public bool IsConnectedToTarget = true;
    bool _isCatchable = true;
    bool _startSlowdown;

    Coroutine _slowdownCoroutine;

    void Start()
    {

    }

    public void Throw(Vector2 throwDirection)
    {
        _rb.simulated = true;
        _rb.linearVelocity = Vector2.zero;
        _rb.gravityScale = 0;
        _isCatchable = false;
        _startSlowdown = false;

        IsConnectedToTarget = false;

        _rb.AddForce(throwDirection);
        Debug.Log("kast");

        //start countdowns
        StartCoroutine(MakeCatchableAfterDelay());

        //stoppa rutinen om den fortfarande är igång
        if (_slowdownCoroutine != null) StopCoroutine(_slowdownCoroutine);

        _slowdownCoroutine = StartCoroutine(StartSlowdownCooldown());
    }

    IEnumerator MakeCatchableAfterDelay()
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

    void FixedUpdate()
    {
        //touched player
        if (PlayerCheck.IsTouching && _isCatchable)
        {
            _rb.simulated = false;
            IsConnectedToTarget = true;
            Debug.Log("nuddade SPelaren");
        }
        //flying freely
        if (!IsConnectedToTarget)
        {
            if (_startSlowdown)
            {
                _rb.gravityScale = setGravity;
                _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, Vector2.zero, slowdownAmount * Time.fixedDeltaTime);
            }
        }
        //following player
        else
        {
            FollowPlayer();
        }
    }
}
