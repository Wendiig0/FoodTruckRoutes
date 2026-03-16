using UnityEngine;
using System.Collections;

public class RoadTile : MonoBehaviour
{
    public float rotationSpeed = 10f;

    private bool isRotating = false;
    private Quaternion targetRotation;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryRotate();
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            TryRotate();
        }
    }

    void TryRotate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == transform)
            {
                if (!isRotating)
                {
                    targetRotation = transform.rotation * Quaternion.Euler(0, 90f, 0);
                    StartCoroutine(RotateTile());
                }
            }
        }
    }

    IEnumerator RotateTile()
    {
        isRotating = true;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        transform.rotation = targetRotation;
        isRotating = false;
    }
}
