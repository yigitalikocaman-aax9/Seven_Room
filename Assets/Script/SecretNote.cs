using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SecretNote : MonoBehaviour
{
    [Header("UI Elemanları")]
    public GameObject interactPromptUI; 
    public GameObject notePanelUI; 
    public TMP_Text noteTextUI; 

    [Header("Not İçeriği")]
    [TextArea(5, 10)]
    public string noteContent = "Sana söylenen notları bulup bir bak...";

    [Header("Final & Sahne Ayarları")]
    public string endGameSceneName = "StartScene"; 
    public CanvasGroup fadeCanvasGroup;          
    public float fadeDuration = 2.0f;            

    private bool isPlayerInsideTrigger = false;
    private bool isNoteOpen = false;
    private bool isAlreadyRead = false;
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;

        if (interactPromptUI != null) interactPromptUI.SetActive(false);
        if (notePanelUI != null) notePanelUI.SetActive(false);

        // Siyah kapıdan geçildiyse kâğıdı aç, yoksa KESİNLİKLE KAPAT
        if (GameSession.hasWokenUpFromDoor)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
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

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = false;
            if (interactPromptUI != null) interactPromptUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (isNoteOpen || isAlreadyRead) return;

        // SADECE MOUSE SOL TIK (E Tuşu Yoktur)
        if (Input.GetMouseButtonDown(0))
        {
            if (isPlayerInsideTrigger)
            {
                OpenSecretNote();
            }
            else
            {
                CheckDirectClick();
            }
        }
    }

    private void CheckDirectClick()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 10f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                OpenSecretNote();
                break;
            }
        }
    }

    public void OpenSecretNote()
    {
        if (isAlreadyRead) return;

        isNoteOpen = true;
        isAlreadyRead = true;

        if (interactPromptUI != null) interactPromptUI.SetActive(false);
        if (notePanelUI != null) notePanelUI.SetActive(true);
        if (noteTextUI != null) noteTextUI.text = noteContent;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(SecretNoteEndRoutine());
    }

    private IEnumerator SecretNoteEndRoutine()
    {
        yield return new WaitForSecondsRealtime(8f);

        if (fadeCanvasGroup != null)
        {
            float timer = 0f;
            fadeCanvasGroup.blocksRaycasts = true;

            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                yield return null;
            }

            fadeCanvasGroup.alpha = 1f;
        }

        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(endGameSceneName))
        {
            SceneManager.LoadScene(endGameSceneName);
        }
    }
}