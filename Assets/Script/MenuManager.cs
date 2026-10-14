using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        // İlk oyun sahnenizin adı neyse "WhitePlace" yerine onu yazabilirsiniz
        SceneManager.LoadScene(1); 
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Unity Editor içinde de durdurması için
        #endif
    }
}