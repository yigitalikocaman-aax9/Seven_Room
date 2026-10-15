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

   
    private bool isLifted = false;
    private bool isYorganClicked = false;
    private bool isNoteOpen = false;
    private bool isNoteRead = false;

    private Vector3 startPosition;  
    private Vector3 liftedPosition; 
    private Vector3 targetPosition; 

    private void Start()
    {
        
        startPosition = transform.position;
        liftedPosition = startPosition + new Vector3(0f, liftDistance, 0f);
        targetPosition = startPosition;

       
        if (noteUIPanel != null)
        {
            noteUIPanel.SetActive(false);
        }
    }

    private void Update()
    {
        
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * liftSpeed);
        }

        
        if (isNoteOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseNoteUI();
        }
    }

    
    private void OnMouseDown()
    {
        if (!isYorganClicked)
        {
            isYorganClicked = true;
            isLifted = true;

            
            targetPosition = liftedPosition;

            
            if (note3DObject != null)
            {
                note3DObject.SetActive(true);
            }

            
        }
    }

    
    public void OpenNoteUI()
    {
        if (isNoteRead || isNoteOpen) return;

        isNoteOpen = true;

        if (noteUIPanel != null) noteUIPanel.SetActive(true);
        if (noteUIText != null) noteUIText.text = noteMessage;

       
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseNoteUI()
    {
        if (!isNoteOpen) return;

        isNoteOpen = false;
        isNoteRead = true; 

        if (noteUIPanel != null) noteUIPanel.SetActive(false);

        
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        
        LowerYorgan();

        
        if (door4Controller != null)
        {
            door4Controller.UnlockDoor4();
            
        }
    }

    
    public void LowerYorgan()
    {
        targetPosition = startPosition; 
    }

   
    public bool IsYorganLifted()
    {
        return isLifted || isYorganClicked;
    }
}