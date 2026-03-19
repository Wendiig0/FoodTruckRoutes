using UnityEngine;
using System.Collections;

public class RoadTile : MonoBehaviour
{
    public float rotationSpeed = 90f; // degrees per second
    private bool isRotating = false;
    private Quaternion targetRotation;
    private Camera mainCamera;

    void Start()
    {
        // Cache Camera.main to avoid repeated FindObjectWithTag calls
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Mouse input (PC)
        if (Input.GetMouseButtonDown(0))
        {
            TryRotate(Input.mousePosition);
        }

        // Touch input (Mobile) - uses correct touch position
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            TryRotate(Input.GetTouch(0).position);
        }
    }

    void TryRotate(Vector2 screenPos)
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == transform && !isRotating)
            {
                targetRotation = transform.rotation * Quaternion.Euler(0, 90f, 0);
                StartCoroutine(RotateTile());
            }
        }
    }

    IEnumerator RotateTile()
    {
        isRotating = true;

        // RotateTowards gives constant speed instead of easing out
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Snap to exact target to avoid floating point drift
        transform.rotation = targetRotation;
        isRotating = false;
    }
}