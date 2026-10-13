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
    public float eyeCloseDuration = 1.5f; 
    public float sleepDuration = 5.0f;    

    [Header("Karakter Kontrolü")]
    public MonoBehaviour playerMovementScript; 

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

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;
            float timer = 0f;

            while (timer < eyeCloseDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / eyeCloseDuration;
                fadeCanvasGroup.alpha = Mathf.SmoothStep(0f, 1f, progress);
                yield return null;
            }

            fadeCanvasGroup.alpha = 1f;
        }

        yield return new WaitForSeconds(sleepDuration);

        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}