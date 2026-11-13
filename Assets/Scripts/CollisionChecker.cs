using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    [HideInInspector] public bool IsTouching;

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Collider2D collider;

    bool IsGroundLayer(int layer)
    {
        // Bitwise check: is the object's layer part of the ground mask?5
        return (targetLayer.value & (1 << layer)) != 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check whether the colliding object is on a layer included in the ground mask.
        if (IsGroundLayer(other.gameObject.layer))
        {
            IsTouching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Stop considering the ground touched once the object leaves the ground layer.
        if (IsGroundLayer(other.gameObject.layer))
        {
            IsTouching = false;
        }
    }


}