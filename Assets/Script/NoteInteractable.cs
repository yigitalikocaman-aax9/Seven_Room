using UnityEngine;

public class NoteInteractable : MonoBehaviour
{
    [TextArea(5, 10)]
    public string noteMessage = "Buraya okutmak istediğin notu yaz...";
    public NoteManager noteManager;

    [Header("4. Kapı Bağlantısı")]
    public DoorController door4Controller; // 4. Kapının üzerindeki script

    private bool isPlayerNearby = false;
    private bool isAlreadyRead = false; // Tek kullanım kontrolü

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
        // 1. Alternatif: Yakındayken E'ye basarak okuma
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ReadNote();
        }
    }

    // 2. Alternatif: Mousela nota tıklandığında okuma
    private void OnMouseDown()
    {
        ReadNote();
    }

    private void ReadNote()
    {
        // Eğer not daha önce okunduysa bir daha çalışmaz
        if (isAlreadyRead) return;

        if (noteManager != null)
        {
            noteManager.ShowNote(noteMessage);
            isAlreadyRead = true; // Tekrar okunmasını engeller

            // 4. Kapının kilidini aç
            if (door4Controller != null)
            {
                door4Controller.UnlockDoor4();
                Debug.Log("4. Kapının kilidi açıldı!");
            }
        }
    }
}