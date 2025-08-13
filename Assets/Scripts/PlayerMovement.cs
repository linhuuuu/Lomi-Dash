using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Camera camera;

    private Vector3 targetPosition;
    private bool hasTarget = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                Ray ray = camera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    targetPosition = hit.point;

                    targetPosition.y = transform.position.y;

                    hasTarget = true;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (hasTarget)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Vector3 moveStep = direction * moveSpeed * Time.fixedDeltaTime;

            if (Vector3.Distance(transform.position, targetPosition) > moveStep.magnitude)
            {
                rb.MovePosition(rb.position + moveStep);
            }
            else
            {
                rb.MovePosition(targetPosition);
                hasTarget = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        hasTarget = false;
    }
}
