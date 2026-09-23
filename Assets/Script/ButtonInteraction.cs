using System.Collections;
using UnityEngine;
using TMPro;

public class ButtonInteraction : MonoBehaviour
{
    [Header("UI / Ekran Elemanları")]
    [SerializeField] private GameObject eKeyPrompt;     // "E" simgesi veya yazısı
    [SerializeField] private GameObject targetScreen;    // Açılacak ekran
    [SerializeField] private TextMeshPro timerText;     // 3D TextMeshPro sayaç nesnesi

    [Header("Buton Animasyonu Ayarları")]
    [SerializeField] private Transform buttonMesh;      // Kırmızı buton mesh'i
    [SerializeField] private Vector3 pushOffset = new Vector3(0, 0, -0.05f); 
    [SerializeField] private float animationDuration = 0.15f; 

    [Header("Geri Sayım Ayarları")]
    [SerializeField] private float startingTime = 180f;   // Başlangıç süresi (saniye)

    private bool isPlayerInRange = false;
    private bool hasBeenUsed = false;                    // Butonun kullanılıp kullanılmadığını tutar
    private float currentTime;

    private Vector3 buttonInitialPosition;

    private void Start()
    {
        if (buttonMesh != null)
        {
            buttonInitialPosition = buttonMesh.localPosition;
        }

        // Oyun başlangıcında "E" ikonunu gizle
        if (eKeyPrompt != null) eKeyPrompt.SetActive(false);
        
        // Oyun başlangıcında EKRANI KAPALI YAP
        if (targetScreen != null) targetScreen.SetActive(false);

        // Oyun başlangıcında TIMER'I (SAYAÇ YAZISINI) KAPALI YAP
        if (timerText != null) timerText.gameObject.SetActive(false);

        currentTime = startingTime;
        UpdateTimerUI();
    }

    private void Update()
    {
        // Oyuncu alandaysa, 'E' tuşuna bastıysa ve buton daha önce kullanılmadıysa
        if (isPlayerInRange && !hasBeenUsed && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        // Butonu bir daha kullanılamaz hale getir
        hasBeenUsed = true;

        // "E" ikonunu hemen kapat
        if (eKeyPrompt != null) eKeyPrompt.SetActive(false);

        // Butonun içeri çöküp çıkma animasyonunu başlat
        if (buttonMesh != null)
        {
            StartCoroutine(AnimateButton());
        }

        // Ekranı AÇ
        if (targetScreen != null)
        {
            targetScreen.SetActive(true);
        }

        // Timer nesnesini AÇ
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }

        // Geri sayımı başlat
        StartCoroutine(StartCountdown());
    }

    private IEnumerator AnimateButton()
    {
        Vector3 targetPos = buttonInitialPosition + pushOffset;
        float elapsedTime = 0f;

        // İçeri çökme
        while (elapsedTime < animationDuration)
        {
            buttonMesh.localPosition = Vector3.Lerp(buttonInitialPosition, targetPos, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        buttonMesh.localPosition = targetPos;

        elapsedTime = 0f;

        // Eski yerine çıkma
        while (elapsedTime < animationDuration)
        {
            buttonMesh.localPosition = Vector3.Lerp(targetPos, buttonInitialPosition, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        buttonMesh.localPosition = buttonInitialPosition;
    }

    private IEnumerator StartCountdown()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0) currentTime = 0;
            
            UpdateTimerUI();
            yield return null;
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Eğer buton henüz kullanılmadıysa "E" ikonunu göster
        if (!hasBeenUsed && other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (eKeyPrompt != null) eKeyPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (eKeyPrompt != null) eKeyPrompt.SetActive(false);
        }
    }
}