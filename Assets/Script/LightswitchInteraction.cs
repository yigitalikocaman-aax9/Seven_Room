using System.Collections;
using UnityEngine;

public class LightswitchInteraction : MonoBehaviour
{
    [Header("Düğme Hareket Ayarları")]
    [Tooltip("Düğmenin duvardan ne kadar öne kayacağı")]
    [SerializeField] private float moveDistance = 0.25f; 
    
    [Tooltip("Hareket hızı")]
    [SerializeField] private float moveSpeed = 3f;

    [Tooltip("Düğmenin öne kayma yönü (Varsayılan Vector3.forward / Z ekseni)")]
    [SerializeField] private Vector3 moveDirection = Vector3.forward;

    [Header("Gerekli Referanslar")]
    [SerializeField] private GameObject noteObject;     // Düğmenin arkasındaki Not
    [SerializeField] private Collider wallCollider;     // Duvardaki Collider (varsa)

    private Vector3 originalPosition;
    private bool isLifted = false;
    private bool isInteractable = true;
    private Coroutine activeMovement;

    private void Start()
    {
        // Düğmenin duvardaki ilk pozisyonunu kaydet
        originalPosition = transform.position;

        Collider switchCol = GetComponent<Collider>();
        if (wallCollider != null && switchCol != null)
        {
            Physics.IgnoreCollision(switchCol, wallCollider, true);
        }

        // Başlangıçta nota basılamasın (Düğme öne kayana kadar)
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
        MoveForward();
    }

    public void MoveForward()
    {
        if (activeMovement != null) StopCoroutine(activeMovement);
        Vector3 targetPos = originalPosition + transform.TransformDirection(moveDirection.normalized) * moveDistance;
        activeMovement = StartCoroutine(MoveRoutine(targetPos, true));
    }

    public void MoveBack()
    {
        if (activeMovement != null) StopCoroutine(activeMovement);
        activeMovement = StartCoroutine(MoveRoutine(originalPosition, false));
    }

    private IEnumerator MoveRoutine(Vector3 targetPosition, bool isMovingForward)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime * moveSpeed);
            yield return null;
        }

        transform.position = targetPosition;
        isLifted = isMovingForward;

        // Öne kayma tamamlandığında notu aç
        if (isMovingForward && noteObject != null)
        {
            Collider noteCol = noteObject.GetComponent<Collider>();
            if (noteCol != null) noteCol.enabled = true;

            NoteSystem noteSystem = noteObject.GetComponent<NoteSystem>();
            if (noteSystem != null)
            {
                noteSystem.OpenNote();
            }
        }
        // Geri çekilme tamamlandığında collider'ı kapat
        else if (!isMovingForward && noteObject != null)
        {
            Collider noteCol = noteObject.GetComponent<Collider>();
            if (noteCol != null) noteCol.enabled = false;
        }
    }
}