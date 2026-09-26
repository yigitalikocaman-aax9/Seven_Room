using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    public Image crosshairImage;
    public Sprite defaultDot;      // Normal nokta ikonu
    public Sprite interactHand;     // El ikonu
    public float interactDistance = 3f;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Kameranın tam ortasından ileriye ray atar
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // Etkileşime geçilebilir nesnelere "Interactable" Tag'i verebilirsin
            if (hit.collider.CompareTag("Interactable"))
            {
                crosshairImage.sprite = interactHand;
                return;
            }
        }

        crosshairImage.sprite = defaultDot;
    }
}