using System.Collections;
using UnityEngine;

public class WakeUpController : MonoBehaviour
{
    [Header("UI & Kamera / Karakter")]
    public CanvasGroup fadeCanvasGroup;
    public Transform playerTransform;
    public MonoBehaviour playerMovementScript; // Karakter yürüme/bakma scriptin

    [Header("Pozisyonlar")]
    public Transform bedSleepPoint;  // Yatakta yatış noktası
    public Transform bedStandPoint;  // Yatağın ucunda ayağa kalkış noktası

    [Header("Göz Kırpma Ayarları")]
    public int blinkCount = 3;         // Kaç kere göz açıp kapanacak
    public float blinkDuration = 0.8f; // Göz açılıp kapanma hızı

    private void Start()
    {
        StartCoroutine(WakeUpSequence());
    }

    private IEnumerator WakeUpSequence()
    {
        // 1. Ekran Simsiyah Başlar & Karakter Yatakta Yatış Pozisyonuna Geçer
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 1f;
        if (playerMovementScript != null) playerMovementScript.enabled = false;

        if (playerTransform != null && bedSleepPoint != null)
        {
            playerTransform.position = bedSleepPoint.position;
            playerTransform.rotation = bedSleepPoint.rotation;
        }

        yield return new WaitForSeconds(1.0f); // 1 saniye siyah ekranda bekle

        // 2. Göz Açılıp Kapanma (Uyku Sersemliği)
        for (int i = 0; i < blinkCount; i++)
        {
            // Göz hafifçe açılır
            float timer = 0f;
            while (timer < blinkDuration)
            {
                timer += Time.deltaTime;
                if (fadeCanvasGroup != null)
                    fadeCanvasGroup.alpha = Mathf.SmoothStep(1f, 0.2f, timer / blinkDuration);
                yield return null;
            }

            // Göz tekrar kapanır
            timer = 0f;
            while (timer < blinkDuration)
            {
                timer += Time.deltaTime;
                if (fadeCanvasGroup != null)
                    fadeCanvasGroup.alpha = Mathf.SmoothStep(0.2f, 1f, timer / blinkDuration);
                yield return null;
            }
        }

        // 3. Ekran Son Kez Simsiyahken Karakter Yatağın Başında Ayağa Kalkar
        if (playerTransform != null && bedStandPoint != null)
        {
            playerTransform.position = bedStandPoint.position;
            playerTransform.rotation = bedStandPoint.rotation;
        }

        yield return new WaitForSeconds(0.4f);

        // 4. Gözler Tamamen Açılır
        float finalTimer = 0f;
        while (finalTimer < 1.2f)
        {
            finalTimer += Time.deltaTime;
            if (fadeCanvasGroup != null)
                fadeCanvasGroup.alpha = Mathf.SmoothStep(1f, 0f, finalTimer / 1.2f);
            yield return null;
        }

        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 0f;

        // 5. Karakter Kontrolü Açılır
        if (playerMovementScript != null) playerMovementScript.enabled = true;
    }
}