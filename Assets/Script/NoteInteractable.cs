using UnityEngine;

public class NoteInteractable : MonoBehaviour
{
    [TextArea(5, 10)]
    public string noteMessage = "Buraya okutmak istediğin notu yaz...";
    public NoteManager noteManager;

    [Header("4. Kapı Bağlantısı")]
    public DoorController door4Controller; 
    private bool isPlayerNearby = false;
    private bool isAlreadyRead = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    private void Update()
    {
       
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ReadNote();
        }
    }

   
    private void OnMouseDown()
    {
        ReadNote();
    }

    private void ReadNote()
    {
        
        if (isAlreadyRead) return;

        if (noteManager != null)
        {
            noteManager.ShowNote(noteMessage);
            isAlreadyRead = true; 

           
            if (door4Controller != null)
            {
                door4Controller.UnlockDoor4();
               
            }
        }
    }
}