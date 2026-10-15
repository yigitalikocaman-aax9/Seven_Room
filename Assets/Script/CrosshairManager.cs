using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    public Image crosshairImage;
    public Sprite defaultDot;      
    public Sprite interactHand;     
    public float interactDistance = 3f;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            
            if (hit.collider.CompareTag("Interactable"))
            {
                crosshairImage.sprite = interactHand;
                return;
            }
        }

        crosshairImage.sprite = defaultDot;
    }
}