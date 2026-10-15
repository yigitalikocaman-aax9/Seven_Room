using System.Collections;
using UnityEngine;

public class LaptopInteraction : MonoBehaviour
{
    [Header("Laptop Hareket Ayarları")]
    [SerializeField] private float liftHeight = 0.35f; 
    [SerializeField] private float moveSpeed = 3f;      

    [Header("Gerekli Referanslar")]
    [SerializeField] private GameObject noteObject;     
    [SerializeField] private Collider deskCollider;     

    private Vector3 originalPosition;
    private bool isLifted = false;
    private bool isInteractable = true;
    private Coroutine activeMovement;

    private void Start()
    {
        originalPosition = transform.position;

        Collider laptopCol = GetComponent<Collider>();
        if (deskCollider != null && laptopCol != null)
        {
            Physics.IgnoreCollision(laptopCol, deskCollider, true);
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
        LiftLaptop();
    }

    public void LiftLaptop()
    {
        if (activeMovement != null) StopCoroutine(activeMovement);
        activeMovement = StartCoroutine(MoveLaptopRoutine(originalPosition + Vector3.up * liftHeight, true));
    }

    public void LowerLaptop()
    {
        if (activeMovement != null) StopCoroutine(activeMovement);
        activeMovement = StartCoroutine(MoveLaptopRoutine(originalPosition, false));
    }

    private IEnumerator MoveLaptopRoutine(Vector3 targetPosition, bool isLifting)
    {
        
        while (Vector3.Distance(transform.position, targetPosition) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime * moveSpeed);
            yield return null;
        }

        transform.position = targetPosition;
        isLifted = isLifting;

      
        if (isLifting && noteObject != null)
        {
            Collider noteCol = noteObject.GetComponent<Collider>();
            if (noteCol != null) noteCol.enabled = true;

            NoteSystem noteSystem = noteObject.GetComponent<NoteSystem>();
            if (noteSystem != null)
            {
                noteSystem.OpenNote();
            }
        }
       
        else if (!isLifting && noteObject != null)
        {
            Collider noteCol = noteObject.GetComponent<Collider>();
            if (noteCol != null) noteCol.enabled = false;
        }
    }
}