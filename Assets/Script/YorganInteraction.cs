using UnityEngine;
using TMPro;

public class YorganInteraction : MonoBehaviour
{
    [Header("Yorgan Ayarları")]
    public float liftDistance = 0.6f;
    public float liftSpeed = 2.0f;

    [Header("Not Objeleri & UI Referansları")]
    [Tooltip("Yorganın altındaki 3D Not objesi")]
    public GameObject note3DObject; 

    [Tooltip("Ekrana açılacak olan Not UI Paneli (Canvas içindeki)")]
    public GameObject noteUIPanel; 

    [Tooltip("UI Paneli içindeki TextMeshPro metin alanı")]
    public TMP_Text noteUIText; 

    [TextArea(4, 8)]
    [Tooltip("Ekranda görünecek not metni")]
    public string noteMessage = "Buraya 4. kapı için yazmak istediğin mesajı yaz...";

    [Header("Açılacak Kapı Bağlantısı")]
    [Tooltip("4. Kapının üzerindeki DoorController")]
    public DoorController door4Controller;

    // Durum ve Pozisyon Kontrolleri
    private bool isLifted = false;
    private bool isYorganClicked = false;
    private bool isNoteOpen = false;
    private bool isNoteRead = false;

    private Vector3 startPosition;  // Yorganın ilk (orijinal) pozisyonu
    private Vector3 liftedPosition; // Yorganın kalkacağı pozisyon
    private Vector3 targetPosition; // Anlık gidilecek hedef pozisyon

    private void Start()
    {
        // İlk pozisyonu ve kalkacağı pozisyonu kaydet
        startPosition = transform.position;
        liftedPosition = startPosition + new Vector3(0f, liftDistance, 0f);
        targetPosition = startPosition;

        // Başlangıçta Not UI panelini kapalı tut
        if (noteUIPanel != null)
        {
            noteUIPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // 1. YORGAN HAREKETİ (Anlık targetPosition'a doğru pürüzsüzce kayar)
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * liftSpeed);
        }

        // 2. NOT AÇIKKEN ESC İLE KAPATMA
        if (isNoteOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseNoteUI();
        }
    }

    // Yorgana tıklanınca çalışan fonksiyon
    private void OnMouseDown()
    {
        if (!isYorganClicked)
        {
            isYorganClicked = true;
            isLifted = true;

            // Hedef pozisyonu yukarısı yap
            targetPosition = liftedPosition;

            // Yorgan kalkınca altındaki 3D notu görünür yap
            if (note3DObject != null)
            {
                note3DObject.SetActive(true);
            }

            Debug.Log("Yorgan kalktı!");
        }
    }

    // 3D Not kağıdına tıklandığında çalışan metot
    public void OpenNoteUI()
    {
        if (isNoteRead || isNoteOpen) return;

        isNoteOpen = true;

        if (noteUIPanel != null) noteUIPanel.SetActive(true);
        if (noteUIText != null) noteUIText.text = noteMessage;

        // Oyunu ve zamanı durdur, imleci serbest bırak
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseNoteUI()
    {
        if (!isNoteOpen) return;

        isNoteOpen = false;
        isNoteRead = true; // Not bir daha okunamaz

        if (noteUIPanel != null) noteUIPanel.SetActive(false);

        // Oyunu devam ettir ve imleci kilitle
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // --- YORGAN ESKİ YERİNE GERİ İNSİN ---
        LowerYorgan();

        // --- 4. KAPININ KİLİDİNİ AÇ ---
        if (door4Controller != null)
        {
            door4Controller.UnlockDoor4();
            Debug.Log("Not kapatıldı, yorgan yerine indi ve 4. Kapının kilidi açıldı!");
        }
    }

    // Yorganı eski yerine indiren fonksiyon
    public void LowerYorgan()
    {
        targetPosition = startPosition; // Hedefi tekrar başlangıç noktası yap
    }

    // Dışarıdan Yorganın kalkıp kalkmadığını sorgulamak istersen:
    public bool IsYorganLifted()
    {
        return isLifted || isYorganClicked;
    }
}