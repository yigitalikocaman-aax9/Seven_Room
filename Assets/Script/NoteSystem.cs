using UnityEngine;
using TMPro;

public class NoteSystem : MonoBehaviour
{
    [Header("UI Kutucuklari")]
    [Tooltip("Kagida yaklasinca ekranda cikacak E harfi / Etkilesim Ikonu")]
    public GameObject interactPromptUI; 

    [Tooltip("Kagidin kendisinin ve metninin oldugu Buyuk Not Paneli")]
    public GameObject notePanelUI; 

    [Tooltip("Not panelinin icindeki TextMeshPro metin alani")]
    public TMP_Text noteTextUI; 

    [Header("Not Icerigi")]
    [TextArea(5, 10)]
    public string noteContent = "Buraya okutmak istedigin notu yaz...";

    [Header("Gerekli Baglantilar")]
    [Tooltip("Bu not okununca kilidi acilacak olan kapi (4. Kapi)")]
    public DoorController doorToUnlock;

    [Tooltip("Yorganin geri inmesini saglayacak YorganInteraction script'i")]
    public YorganInteraction yorganScript;

    private bool isPlayerInsideTrigger = false;
    private bool isNoteOpen = false;
    private bool isAlreadyRead = false; // Tek kullanım kontrolü
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;

        if (interactPromptUI != null) interactPromptUI.SetActive(false);
        if (notePanelUI != null) notePanelUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = true;

            if (!isAlreadyRead && !isNoteOpen && interactPromptUI != null)
            {
                interactPromptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = false;

            if (interactPromptUI != null)
            {
                interactPromptUI.SetActive(false);
            }
        }
    }

    private void Update()
    {
        // 1. E Tuşu ile açma
        if (!isAlreadyRead && isPlayerInsideTrigger && !isNoteOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenNote();
        }
        // 2. Not açıkken ESC ile kapatma
        else if (isNoteOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseNote();
        }

        // 3. Sol Tık ile Yatak/Yorgan Collider'larını DELİP GEÇEN Tıklama Mantığı
        if (!isAlreadyRead && !isNoteOpen && Input.GetMouseButtonDown(0))
        {
            CheckDirectClick();
        }
    }

    // Yatağın ve yorganın collider'larını delip doğrudan kağıdı bulan metot
    private void CheckDirectClick()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        
        // Işının çarptığı TÜM objeleri tarar (RaycastAll)
        RaycastHit[] hits = Physics.RaycastAll(ray, 10f);

        foreach (RaycastHit hit in hits)
        {
            // Eğer ışının çarptığı objelerden BİRİ bu not kağıdı ise aç
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                OpenNote();
                break; // Notu bulduğu an döngüden çıkar
            }
        }
    }

    public void OpenNote()
    {
        if (isAlreadyRead) return;

        isNoteOpen = true;

        if (interactPromptUI != null) interactPromptUI.SetActive(false);
        if (notePanelUI != null) notePanelUI.SetActive(true);
        if (noteTextUI != null) noteTextUI.text = noteContent;

        // Oyunu ve zamanı durdur, imleci serbest bırak
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseNote()
    {
        isNoteOpen = false;
        isAlreadyRead = true; // Not okundu, bir daha açılamaz!

        if (notePanelUI != null) notePanelUI.SetActive(false);
        if (interactPromptUI != null) interactPromptUI.SetActive(false);

        // Oyunu devam ettir ve imleci tekrar kilitle
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // --- 1. YORGAN ESKİ YERİNE GERİ İNSİN ---
        if (yorganScript != null)
        {
            yorganScript.LowerYorgan();
        }

        // --- 2. NOT KAPANDI, 4. KAPININ KİLİDİNİ AÇ ---
        if (doorToUnlock != null)
        {
            doorToUnlock.UnlockDoor4();
            Debug.Log("Not okundu ve kapatıldı. Yorgan yerine indi ve 4. Kapının kilidi açıldı!");
        }

        // Tıklamayı tamamen engellemek için Collider'ı kapat
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}