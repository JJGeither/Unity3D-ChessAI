using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 5.0f;

    private float _distance;

    private void Start()
    {
        _distance = Vector3.Distance(transform.position, target.position);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.RotateAround(target.position, Vector3.up, -rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);
        }

        transform.position = target.position - transform.forward * _distance;
    }
}