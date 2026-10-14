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
    public float eyeCloseDuration = 2.0f; 
    public float sleepDuration = 2.0f;    

    [Header("Karakter ve Yatak Dönüşümü")]
    public MonoBehaviour playerMovementScript; 
    public Transform playerTransform; // Oyuncunun veya Kameranın Transform'u
    public Transform sleepPoint;       // Yatağın üzerindeki hedef pozisyon (SleepPoint)
    public float layDownSpeed = 2.0f;  

    [Header("Yatış Yönü Ayarı")]
    [Tooltip("Sağa doğru devrilme açısı (Derece)")]
    public float rightTiltAngle = -90f; // Sağa yatması için Z ekseninde rotasyon

    private bool isPlayerInRange = false;
    private bool isSleeping = false;

    private void Start()
    {
        // Yalnızca siyah kapıdan dönülüp uyanıldıysa yatak etkileşimini kapat
        if (GameSession.hasWokenUpFromDoor)
        {
            if (interactUI != null) interactUI.SetActive(false);
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
            return;
        }

        if (interactUI != null) interactUI.SetActive(false);
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (GameSession.hasWokenUpFromDoor) return;

        if (isPlayerInRange && !isSleeping && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(EyeCloseRoutine());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameSession.hasWokenUpFromDoor) return;

        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactUI != null && !isSleeping) interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (GameSession.hasWokenUpFromDoor) return;

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

        Vector3 startPos = playerTransform != null ? playerTransform.position : Vector3.zero;
        Quaternion startRot = playerTransform != null ? playerTransform.rotation : Quaternion.identity;

        // SleepPoint rotasyonunun üzerine tam sağa devrilme açısını kodla ekliyoruz
        Quaternion baseTargetRot = sleepPoint != null ? sleepPoint.rotation : startRot;
        Quaternion targetRot = baseTargetRot * Quaternion.Euler(0, 0, rightTiltAngle);

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;
        }

        float timer = 0f;

        while (timer < eyeCloseDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / eyeCloseDuration;
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            // Oyuncuyu SleepPoint'e kaydırıp sağa doğru yatırır
            if (playerTransform != null && sleepPoint != null)
            {
                playerTransform.position = Vector3.Lerp(startPos, sleepPoint.position, smoothProgress);
                playerTransform.rotation = Quaternion.Slerp(startRot, targetRot, smoothProgress);
            }

            // Göz kapanma (Fade) etkisi
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = smoothProgress;
            }

            yield return null;
        }

        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(sleepDuration);

        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}