using System.Collections;
using UnityEngine;

public class LaptopInteraction : MonoBehaviour
{
    [Header("Laptop Hareket Ayarları")]
    [SerializeField] private float liftHeight = 0.35f; // Laptop'ın kalkacağı yükseklik
    [SerializeField] private float moveSpeed = 3f;      // Yükselme / İnme hızı (Değeri küçülterek daha da yavaşlatabilirsin)

    [Header("Gerekli Referanslar")]
    [SerializeField] private GameObject noteObject;     // Laptopun altındaki Paper (3)
    [SerializeField] private Collider deskCollider;     // Masanın Collider'ı

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
        // Time.unscaledDeltaTime kullanıyoruz ki zaman durmuş olsa (Time.timeScale = 0) bile hareket akıcı devam etsin.
        while (Vector3.Distance(transform.position, targetPosition) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime * moveSpeed);
            yield return null;
        }

        transform.position = targetPosition;
        isLifted = isLifting;

        // Yukarı kalkma hareketi tamamlandığında notu aç
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
        // Aşağı inme tamamlandığında collider'ı kapat
        else if (!isLifting && noteObject != null)
        {
            Collider noteCol = noteObject.GetComponent<Collider>();
            if (noteCol != null) noteCol.enabled = false;
        }
    }
}