using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraXY : MonoBehaviour
{
    public Transform cameraTransform;
    public float followDistance = 10f;

    void LateUpdate()
    {
        if (!cameraTransform) return;
        transform.position = new Vector3(
            cameraTransform.position.x,
            cameraTransform.position.y,
            cameraTransform.position.z + followDistance
        );
    }
}
