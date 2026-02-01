using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private bool needsKey = true;
    [SerializeField] private string lockedMessage = "It's locked. I need a key...";
    [SerializeField] private string openSound = "Door_Open";
    [SerializeField] private string lockedSound = "Door_Locked";

    public void Interact()
    {
        // 1. Verificăm dacă ușa are nevoie de cheie
        if (needsKey)
        {
            // 2. Verificăm în Inventar dacă avem cheia
            if (SimpleInventoryManager.Instance != null && SimpleInventoryManager.Instance.hasKey)
            {
                OpenDoor();
            }
            else
            {
                Debug.Log(lockedMessage);
                GameEvents.TriggerSound(lockedSound);
                // Aici poți adăuga un mesaj pe UI dacă vrei
            }
        }
        else
        {
            // Dacă ușa nu e încuiată, se deschide direct
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        Debug.Log("Door opened!");
        GameEvents.TriggerSound(openSound);

        // Putem distruge obiectul sau doar să îl dezactivăm
        // Dezactivarea e mai bună dacă vrei să o refolosești sau să o închizi ulterior
        gameObject.SetActive(false); 
    }
}