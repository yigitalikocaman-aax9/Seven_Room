using UnityEngine;

public class PillowNoteInteraction : MonoBehaviour
{
    [Header("Yastık Ayarları")]
    public float liftAmountY = 0.5f; // Ne kadar yukarı kalkacağı
    public float moveSpeed = 8f;     // Hareket hızı

    [Header("Görsel Model (İsteğe Bağlı)")]
    public Transform visualModel;    // Yastığın mesh modeli

    [Header("Not & UI Referansları")]
    public GameObject noteObject;    // Paper objesi
    public NoteManager noteManager;  // NoteManager scripti
    [TextArea] public string noteMessage = "Yastığın altındaki gizli not!";

    [Header("Açılacak Kapı Referansı")]
    public DoorController targetDoor; // 3. Kapının DoorController scripti

    [Header("Mesafe ve Katman Ayarları")]
    public float interactDistance = 10f;
    public LayerMask interactableLayer;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isPillowLifted = false;
    private bool isCompleted = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (visualModel == null)
        {
            visualModel = transform;
        }

        startPos = visualModel.position;
        targetPos = startPos;

        // Başlangıçta kağıdın tıklanmasını engelle
        if (noteObject != null)
        {
            Collider noteCollider = noteObject.GetComponent<Collider>();
            if (noteCollider != null) noteCollider.enabled = false;
        }
    }

    void Update()
    {
        if (visualModel != null)
        {
            visualModel.position = Vector3.Lerp(visualModel.position, targetPos, Time.deltaTime * moveSpeed);
        }

        if (isCompleted || Time.timeScale == 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            if (!isPillowLifted && (hit.transform == transform || hit.transform.IsChildOf(transform) || hit.transform == visualModel))
            {
                LiftPillow();
            }
            else if (isPillowLifted && noteObject != null && (hit.transform == noteObject.transform || hit.transform.IsChildOf(noteObject.transform)))
            {
                ReadNoteAndResetPillow();
            }
        }
    }

    void LiftPillow()
    {
        isPillowLifted = true;
        targetPos = startPos + Vector3.up * liftAmountY;

        if (noteObject != null)
        {
            Collider noteCollider = noteObject.GetComponent<Collider>();
            if (noteCollider != null) noteCollider.enabled = true;
        }
    }

    void ReadNoteAndResetPillow()
    {
        targetPos = startPos;

        // Notu ekrana getir ve oyunu durdur
        if (noteManager != null)
        {
            noteManager.ShowNote(noteMessage);
        }

        // --- SENİN KAPI SCRİPTİNİ TETİKLİYORUZ ---
        if (targetDoor != null)
        {
            targetDoor.UnlockDoor();
        }

        isCompleted = true;

        if (noteObject != null)
        {
            Collider noteCollider = noteObject.GetComponent<Collider>();
            if (noteCollider != null) noteCollider.enabled = false;
        }
    }
}