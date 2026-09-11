using System.Collections;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [Header("Bileşenler")]
    public Light targetLight;
    public MeshRenderer targetRenderer; // Floresan tüpü
    public MeshRenderer wallRenderer;   // Kapının üstündeki duvar (Varsa)

    [Header("Kırpışma Ayarları")]
    public float minWaitTime = 0.02f;
    public float maxWaitTime = 0.15f;
    public float maxIntensity = 5f; // Yüksek tutarak baked lekeyi baskılarız

    private Material targetMaterial;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private Color baseEmissionColor;

    void Start()
    {
        if (targetLight == null) targetLight = GetComponent<Light>();

        if (targetRenderer != null)
        {
            targetMaterial = targetRenderer.material;
            baseEmissionColor = targetMaterial.GetColor(EmissionColor);
            targetMaterial.EnableKeyword("_EMISSION");
        }

        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

            bool isOn = Random.value > 0.3f;

            if (isOn)
            {
                // Işık açıldığında yüksek şiddette vurur
                if (targetLight != null) targetLight.intensity = maxIntensity;
                if (targetMaterial != null) targetMaterial.SetColor(EmissionColor, baseEmissionColor);
            }
            else
            {
                // Işık kapandığında alanı tamamen gölgede bırakır
                if (targetLight != null) targetLight.intensity = 0f;
                if (targetMaterial != null) targetMaterial.SetColor(EmissionColor, Color.black);
            }
        }
    }
}