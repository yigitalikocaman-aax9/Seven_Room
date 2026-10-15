using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NoteSystem : MonoBehaviour
{
    [Header("UI Kutucuklari")]
    public GameObject interactPromptUI; 
    public GameObject notePanelUI; 
    public TMP_Text noteTextUI; 

    [Header("Not Icerigi")]
    [TextArea(5, 10)]
    public string noteContent = "Buraya okutmak istedigin notu yaz...";

    [Header("Gerekli Baglantilar")]
    public DoorController doorToUnlock;
    public PlateInteraction plateScript;             
    public LaptopInteraction laptopScript;           
    public LightswitchInteraction lightswitchScript; 
    public YorganInteraction yorganScript;           

    [Header("Gizli Not ve Final Ayarları")]
    public bool isSecretNote = false;
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

        
        if (isSecretNote)
        {
            if (PlayerPrefs.GetInt("GameFinished", 0) == 1)
            {
                SetNoteVisibility(true);
            }
            else
            {
                SetNoteVisibility(false);
            }
        }
    }

    private void SetNoteVisibility(bool visible)
    {
        Renderer rend = GetComponent<Renderer>();
        Collider col = GetComponent<Collider>();

        if (rend != null) rend.enabled = visible;
        if (col != null) col.enabled = visible;

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = visible;
        }
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = visible;
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
            if (interactPromptUI != null) interactPromptUI.SetActive(false);
        }
    }

    private void Update()
    {
       
        if (isSecretNote && isNoteOpen) return;

        
        if (!isAlreadyRead && isPlayerInsideTrigger && !isNoteOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenNote();
        }
       
        else if (isNoteOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)))
        {
            CloseNote();
        }

        
        if (!isAlreadyRead && !isNoteOpen && Input.GetMouseButtonDown(0))
        {
            CheckDirectClick();
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
                OpenNote();
                break;
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

       
        TriggerDoor();

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        
        if (isSecretNote)
        {
            isAlreadyRead = true;
            StartCoroutine(SecretNoteEndRoutine());
        }
    }

    public void CloseNote()
    {
        isNoteOpen = false;
        isAlreadyRead = true;

        if (notePanelUI != null) notePanelUI.SetActive(false);
        if (interactPromptUI != null) interactPromptUI.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

       
        if (plateScript != null) plateScript.LowerPlate();
        if (laptopScript != null) laptopScript.LowerLaptop();
        if (lightswitchScript != null) lightswitchScript.MoveBack();
        if (yorganScript != null) yorganScript.LowerYorgan();

        TriggerDoor();

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    private void TriggerDoor()
    {
        if (doorToUnlock != null)
        {
            doorToUnlock.UnlockDoor4();
            
        }
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