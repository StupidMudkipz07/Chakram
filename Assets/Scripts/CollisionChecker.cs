using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    [HideInInspector] public bool IsTouching;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] Collider2D collider;

    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((groundLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            IsTouching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((groundLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            IsTouching = false;
        }
    }


}