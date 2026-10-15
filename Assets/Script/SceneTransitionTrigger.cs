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
            
            GameSession.hasWokenUpFromDoor = true;

            SceneManager.LoadScene(targetSceneName);
        }
    }
}