using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Hedef Sahne")]
    public string targetSceneName = "RoomPlace";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Siyah kapıdan geçildiğini hafızaya alıyoruz
            GameSession.hasWokenUpFromDoor = true;

            SceneManager.LoadScene(targetSceneName);
        }
    }
}