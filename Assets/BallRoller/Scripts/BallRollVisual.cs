using UnityEngine;

[AddComponentMenu("Ball Roller/Ball Roll Visual")]
public class BallRollVisual : MonoBehaviour
{
    public float radius = 0.5f;

    Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector3 delta = currentPosition - lastPosition;
        float distance = delta.magnitude;
        if (distance > 0.0001f)
        {
            Vector3 axis = Vector3.Cross(Vector3.up, delta.normalized);
            float angle = (distance / radius) * Mathf.Rad2Deg;
            transform.Rotate(axis, angle, Space.World);
        }
        lastPosition = currentPosition;
    }
}
