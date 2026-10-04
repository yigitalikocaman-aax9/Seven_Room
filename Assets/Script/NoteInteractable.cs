using UnityEngine;

public class NoteInteractable : MonoBehaviour
{
    [TextArea(5, 10)]
    public string noteMessage = "Buraya okutmak istediğin notu yaz...";
    public NoteManager noteManager;

    private bool isPlayerNearby = false;

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
            if (noteManager != null)
            {
                noteManager.ShowNote(noteMessage);
            }
        }
    }
}