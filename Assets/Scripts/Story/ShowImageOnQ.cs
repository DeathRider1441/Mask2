using UnityEngine;

public class ShowImageOnQ : MonoBehaviour
{
    [Header("Imaginea care se va afișa")]
    public GameObject imageObject;

    private bool playerInRange = false;

    private void Start()
    {
        if (imageObject != null)
        {
            imageObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Q))
        {
            if (imageObject != null)
            {
                imageObject.SetActive(!imageObject.activeSelf);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (imageObject != null)
            {
                imageObject.SetActive(false);
            }
        }
    }
}