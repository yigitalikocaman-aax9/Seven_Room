using System.Collections;
using UnityEngine;

public class PlateInteraction : MonoBehaviour
{
    [Header("Tabak Hareket Ayarları")]
    [SerializeField] private float liftHeight = 0.25f;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Gerekli Referanslar")]
    [SerializeField] private GameObject appleObject;
    [SerializeField] private GameObject noteObject;
    [SerializeField] private Collider tableCollider;

    private Vector3 originalPosition;
    private bool isLifted = false;
    private bool isInteractable = true;
    private Collider plateCollider;

    private void Start()
    {
        // Dünya pozisyonunu kaydet
        originalPosition = transform.position;
        plateCollider = GetComponent<Collider>();

        if (tableCollider != null && plateCollider != null)
        {
            Physics.IgnoreCollision(plateCollider, tableCollider, true);
        }

        if (appleObject != null && appleObject.transform.parent != transform)
        {
            appleObject.transform.SetParent(transform);
        }

        if (noteObject != null)
        {
            Collider noteCol = noteObject.GetComponent<Collider>();
            if (noteCol != null) noteCol.enabled = false;
        }
    }

    private void OnMouseDown()
    {
        if (!isInteractable || isLifted) return;

        isInteractable = false;
        StartCoroutine(LiftPlateRoutine());
    }

    private IEnumerator LiftPlateRoutine()
    {
        Vector3 targetPosition = originalPosition + Vector3.up * liftHeight;

        while (Vector3.Distance(transform.position, targetPosition) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime * moveSpeed);
            yield return null;
        }
        transform.position = targetPosition;
        isLifted = true;

        if (noteObject != null)
        {
            Collider noteCol = noteObject.GetComponent<Collider>();
            if (noteCol != null) noteCol.enabled = true;
        }
    }

    public void LowerPlate()
    {
        if (isLifted)
        {
            StartCoroutine(LowerPlateRoutine());
        }
    }

    private IEnumerator LowerPlateRoutine()
    {
        while (Vector3.Distance(transform.position, originalPosition) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.unscaledDeltaTime * moveSpeed);
            yield return null;
        }
        transform.position = originalPosition;
        isLifted = false;
    }
}