using UnityEngine;

public class ShowImageOnQ : MonoBehaviour
{
    [Header("Image to inspect")]
    public GameObject imageObject;

    [Header("Press Q text")]
    public GameObject inspectText;

    private bool playerInRange = false;

    void Start()
    {
        if (imageObject != null)
            imageObject.SetActive(false);

        if (inspectText != null)
            inspectText.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Q))
        {
            imageObject.SetActive(!imageObject.activeSelf);

            // Ascunde textul cât timp citești foaia
            if (inspectText != null)
                inspectText.SetActive(!imageObject.activeSelf);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (inspectText != null)
                inspectText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (imageObject != null)
                imageObject.SetActive(false);

            if (inspectText != null)
                inspectText.SetActive(false);
        }
    }
}