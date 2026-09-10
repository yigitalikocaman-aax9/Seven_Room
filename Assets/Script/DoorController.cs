using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("UI / İkon Ayarı")]
    public GameObject ePromptObject;

    [Header("Kapı / Menteşe Ayarları")]
    public Transform doorHinge;
    public float openAngle = 90f;
    public float speed = 2f;

    private bool isNear = false;
    private bool isOpen = false;
    private Quaternion defaultLocalRotation;
    private Quaternion targetLocalRotation;

    void Start()
    {
        if (doorHinge == null) doorHinge = transform;

        // Dönüşü local (yerel) eksene sabitle
        defaultLocalRotation = doorHinge.localRotation;
        targetLocalRotation = defaultLocalRotation * Quaternion.Euler(0, openAngle, 0);

        if (ePromptObject != null)
            ePromptObject.SetActive(false);
    }

    void Update()
    {
        if (isNear && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        Quaternion target = isOpen ? targetLocalRotation : defaultLocalRotation;
        
        // localRotation kullanarak parent-child çakışmasını engelle
        doorHinge.localRotation = Quaternion.Slerp(doorHinge.localRotation, target, Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = true;
            if (ePromptObject != null) ePromptObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = false;
            if (ePromptObject != null) ePromptObject.SetActive(false);
        }
    }
}