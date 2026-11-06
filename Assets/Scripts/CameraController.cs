using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothing;

    void FixedUpdate()
    {
        Vector3 newPosition = Vector3.Lerp(transform.position, _target.position + _offset, _smoothing * Time.deltaTime);
        transform.position = newPosition;
    }
}
