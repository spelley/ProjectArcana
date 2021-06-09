using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	Vector3 offset;
    public Transform target;
    public float smoothTime = 0.3F;
    Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
		offset = transform.position - target.position;
	}

    void Update()
    {
        // Define a target position with offset
        Vector3 targetPosition = target.position + offset;

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}