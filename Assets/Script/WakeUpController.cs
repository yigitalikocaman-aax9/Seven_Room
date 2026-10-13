using System.Collections;
using UnityEngine;

public class WakeUpController : MonoBehaviour
{
    [Header("Test Modu")]
    [Tooltip("İşaretlerseniz diğer sahneden gelmeseniz bile bu sahnede Play'e basınca uyanma çalışır.")]
    public bool forceWakeUpInEditor = true;

    [Header("UI & Göz Kırpma Ayarları")]
    public GameObject fadePanelObject;    
    public CanvasGroup blinkCanvasGroup;  
    public int blinkCount = 3;            
    public float blinkSpeed = 1.5f;       

    [Header("Oyuncu ve Yatak Pozisyonları")]
    public Transform playerTransform;     
    public Transform bedSleepTransform;   // Yatağın üzerindeki yatış noktası
    public Transform bedStandTransform;   // Yataktan kalkınca duracağı nokta
    public MonoBehaviour playerMovementScript; 

    [Header("Kamera Ayarları")]
    public Transform playerCameraTransform; // Main Camera Transform'u
    [Tooltip("Yatakta yatarken bakış açısı (-90 X açısı tam tavana/yukarı baktırır)")]
    public Vector3 sleepCameraRotation = new Vector3(-90f, 0f, 0f); 

    [Header("Ayağa Kalkma Ayarları")]
    public float standUpSpeed = 1.2f;     // Yataktan kalkıp doğrulma süresi hızı

    [Header("Müzik Ayarı")]
    [Tooltip("Uyanırken kapatılacak arka plan müziği objesi")]
    public GameObject backgroundMusicObject;

    private CharacterController charController;

    private void Start()
    {
        if (playerTransform != null)
        {
            charController = playerTransform.GetComponent<CharacterController>();
        }

        // Yalnızca siyah kapıdan geçildiyse uyanma sekansını ve müzik kapatmayı çalıştır
        if (forceWakeUpInEditor || GameSession.hasWokenUpFromDoor)
        {
            // Siyah kapıdan dönüldüyse arka plan müziğini anında kapat
            if (backgroundMusicObject != null)
            {
                backgroundMusicObject.SetActive(false);
            }

            StartCoroutine(WakeUpSequence());
        }
        else
        {
            if (fadePanelObject != null)
                fadePanelObject.SetActive(false);
        }
    }

    private IEnumerator WakeUpSequence()
    {
        // 1. FADE PANELİ AÇ VE SİYAH YAP
        if (fadePanelObject != null)
            fadePanelObject.SetActive(true);

        if (blinkCanvasGroup != null)
            blinkCanvasGroup.alpha = 1f;

        // 2. KONTROLLERİ VE FİZİĞİ KAPAT
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (charController != null) charController.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 3. YATAĞA YERLEŞTİR VE KAMERAYI TAVANA ÇEVİR
        if (playerTransform != null && bedSleepTransform != null)
        {
            playerTransform.position = bedSleepTransform.position;
            playerTransform.rotation = bedSleepTransform.rotation;
        }

        if (playerCameraTransform != null)
        {
            playerCameraTransform.localRotation = Quaternion.Euler(sleepCameraRotation);
        }

        yield return new WaitForSeconds(0.4f);

        // 4. GÖZ KIRPMA EFEKTİ
        for (int i = 0; i < blinkCount; i++)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * blinkSpeed;
                if (blinkCanvasGroup != null)
                    blinkCanvasGroup.alpha = Mathf.Lerp(1f, 0.15f, t);
                yield return null;
            }

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * blinkSpeed;
                if (blinkCanvasGroup != null)
                    blinkCanvasGroup.alpha = Mathf.Lerp(0.15f, 1f, t);
                yield return null;
            }
        }

        // 5. YATAKTAN KALKMA & DOĞRULMA ANİMASYONU
        Vector3 startPos = bedSleepTransform.position;
        Quaternion startRot = bedSleepTransform.rotation;
        
        Vector3 targetPos = bedStandTransform.position;
        Quaternion targetRot = bedStandTransform.rotation;

        float standT = 0f;

        while (standT < 1f)
        {
            standT += Time.deltaTime * standUpSpeed;

            if (playerTransform != null && bedSleepTransform != null && bedStandTransform != null)
            {
                playerTransform.position = Vector3.Lerp(startPos, targetPos, standT);
                playerTransform.rotation = Quaternion.Slerp(startRot, targetRot, standT);
            }

            if (blinkCanvasGroup != null)
                blinkCanvasGroup.alpha = Mathf.Lerp(1f, 0f, standT);

            if (playerCameraTransform != null)
            {
                playerCameraTransform.localRotation = Quaternion.Slerp(
                    Quaternion.Euler(sleepCameraRotation), 
                    Quaternion.identity, 
                    standT
                );
            }

            yield return null;
        }

        // 6. SON POZİSYONLARI KESİNLEŞTİR VE ROTASYONU ZORLA EŞİTLE
        if (blinkCanvasGroup != null) blinkCanvasGroup.alpha = 0f;
        if (fadePanelObject != null) fadePanelObject.SetActive(false);

        if (playerTransform != null && bedStandTransform != null)
        {
            playerTransform.position = bedStandTransform.position;
            playerTransform.rotation = bedStandTransform.rotation;
        }

        if (playerCameraTransform != null)
        {
            playerCameraTransform.localRotation = Quaternion.identity;
        }

        // 7. KONTROLLERİ GERİ AÇMADAN ÖNCE SIFIRLAMA
        yield return new WaitForEndOfFrame();

        if (charController != null) charController.enabled = true;
        if (playerMovementScript != null) playerMovementScript.enabled = true;
    }
}