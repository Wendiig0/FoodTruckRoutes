using UnityEngine;
using System.Collections;

public class RoadTile : MonoBehaviour
{
    [Header("Tile Settings")]
    public float rotationSpeed = 180f;

    private bool isRotating = false;
    private Quaternion targetRotation;

    public bool IsRotating => isRotating;

    public void Rotate90()
    {
        if (isRotating) return;

        targetRotation = transform.rotation * Quaternion.Euler(0f, 90f, 0f);
        StartCoroutine(RotateTileRoutine());
    }

    private IEnumerator RotateTileRoutine()
    {
        isRotating = true;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.rotation = targetRotation;
        isRotating = false;
    }
}