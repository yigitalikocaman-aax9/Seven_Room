using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TypewriterEffect : MonoBehaviour
{
    [Header("UI & Metin Ayarları")]
    public TextMeshProUGUI textUI;        // Yazının yazılacağı TextMeshPro bileşeni
    [TextArea(5, 10)]
    public string fullText;               // Görünmesini istediğin tüm metin
    public float fontSize = 60f;          // Yazının punto/boyut değeri

    [Header("Hız ve Zamanlama Ayarları")]
    public float typingSpeed = 0.05f;     // Her harf arası bekleme süresi
    public float startDelay = 1.0f;       // Siyah ekran geldikten kaç sn sonra yazı başlasın?
    public float delayBeforeSceneChange = 7.0f; // Yazı bittikten kaç sn sonra StartScene'e dönsün?

    [Header("Sahne Ayarları")]
    public string targetSceneName = "StartScene";

    [Header("Ses Ayarları")]
    public AudioSource audioSource;       // Klavye sesini çalacak AudioSource
    public AudioClip typingAudioClip;     // İndirdiğin klavye/yazma ses dosyası
    public float soundDuration = 3.0f;    // Sesin çalacağı maksimum süre (Saniye)

    void Start()
    {
        if (textUI != null)
        {
            textUI.text = "";
            textUI.fontSize = fontSize;   // Yazı boyutunu ayarla
            StartCoroutine(TypeTextAndReturn());
        }
    }

    IEnumerator TypeTextAndReturn()
    {
        // 1. Sahne açıldıktan sonra siyah ekranda bekle
        yield return new WaitForSeconds(startDelay);

        // 2. Klavye sesini başlat ve 3 saniye sonra durduracak zamanlayıcıyı çalıştır
        if (audioSource != null && typingAudioClip != null)
        {
            audioSource.clip = typingAudioClip;
            audioSource.loop = true; // Ses 3 saniyeden kısaysa kesilmesin diye döngüye alıyoruz
            audioSource.Play();
            StartCoroutine(StopAudioAfterDelay(soundDuration));
        }

        // 3. Metindeki her harfi sırayla ekrana yazdır
        foreach (char c in fullText.ToCharArray())
        {
            textUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Yazı erken biterse ve ses hala çalıyorsa sesi tamamen kapat
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // 4. Yazı bittikten sonra 7 saniye bekle ve ana menüye dön
        yield return new WaitForSeconds(delayBeforeSceneChange);
        SceneManager.LoadScene(targetSceneName);
    }

    // Sesi belirlenen süre (3 saniye) sonunda durduran Coroutine
    IEnumerator StopAudioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}