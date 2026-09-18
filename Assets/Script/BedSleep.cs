using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BedSleep : MonoBehaviour
{
    [Header("UI Ayarları")]
    public GameObject interactUI; 
    public CanvasGroup fadeCanvasGroup; 

    [Header("Sahne Ayarları")]
    public string targetSceneName = "Scene/WhitePlace"; 

    [Header("Zamanlama Ayarları")]
    public float eyeCloseDuration = 1.5f; // Gözlerin kapanma süresi
    public float sleepDuration = 5.0f;    // Siyah ekranda bekleme süresi

    [Header("Karakter Kontrolü")]
    public MonoBehaviour playerMovementScript; 

    private bool isPlayerInRange = false;
    private bool isSleeping = false;

    private void Start()
    {
        if (interactUI != null) interactUI.SetActive(false);
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && !isSleeping && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(EyeCloseRoutine());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactUI != null && !isSleeping) interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    private IEnumerator EyeCloseRoutine()
    {
        isSleeping = true;

        if (interactUI != null) interactUI.SetActive(false);
        if (playerMovementScript != null) playerMovementScript.enabled = false;

        // Göz Kapanıyor (Soft Fade In)
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;
            float timer = 0f;

            while (timer < eyeCloseDuration)
            {
                timer += Time.deltaTime;
                // Soft/Yumuşak geçiş için SmoothStep kullanımı
                float progress = timer / eyeCloseDuration;
                fadeCanvasGroup.alpha = Mathf.SmoothStep(0f, 1f, progress);
                yield return null;
            }

            fadeCanvasGroup.alpha = 1f;
        }

        // 5 Saniye Uykuda Bekleme
        yield return new WaitForSeconds(sleepDuration);

        // Yeni Sahneye Geçiş
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}