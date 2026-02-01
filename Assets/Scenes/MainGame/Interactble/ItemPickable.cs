using UnityEngine;

// Enumerare pentru a alege tipul de item direct din Inspector
public enum ItemType { Potion, Key, Mask }

public class ItemPickable : MonoBehaviour, IInteractable
{
    [Header("Item Settings")]
    public ItemType type;
    public int amount = 1;

    [Header("Audio")]
    [SerializeField] private string pickUpSound = "PickUp_Item";

    // Această metodă este cerută de interfața IInteractable
    public void Interact()
    {
        PickUp();
    }

    private void PickUp()
    {
        // 1. Adăugăm item-ul în inventar în funcție de tip
        if (SimpleInventoryManager.Instance != null)
        {
            switch (type)
            {
                case ItemType.Potion:
                    SimpleInventoryManager.Instance.potionCount += amount;
                    Debug.Log($"Picked up {amount} Potion(s).");
                    break;

                case ItemType.Key:
                    SimpleInventoryManager.Instance.hasKey = true;
                    Debug.Log("Picked up the Key.");
                    break;

                case ItemType.Mask:
                    // Aici poți adăuga logica pentru Mască (dacă ai una în Manager)
                    Debug.Log("Picked up the Mask.");
                    break;
            }
        }

        // 2. Declanșăm sunetul de ridicare
        GameEvents.TriggerSound(pickUpSound);

        // 3. Trimitem un zgomot (opțional, dacă vrei ca inamicii să audă când ridici ceva)
        GameEvents.TriggerNoise(transform.position, 2f);

        // 4. Distrugem obiectul din lume
        Destroy(gameObject);
    }
}