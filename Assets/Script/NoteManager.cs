using UnityEngine;
using TMPro;

public class NoteManager : MonoBehaviour
{
    public GameObject notePanel;
    public TMP_Text noteTextUI;

    private bool isNoteOpen = false;

    public void ShowNote(string message)
    {
        noteTextUI.text = message;
        notePanel.SetActive(true);
        
       
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isNoteOpen = true;
    }

    public void CloseNote()
    {
        notePanel.SetActive(false);
        
       
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isNoteOpen = false;
    }

    private void Update()
    {
        if (isNoteOpen && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)))
        {
            CloseNote();
        }
    }
}