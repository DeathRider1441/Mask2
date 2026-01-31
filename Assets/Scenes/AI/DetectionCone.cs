using UnityEngine;

public class DetectionCone : MonoBehaviour
{
    private Entity parentEntity;

    void Start()
    {
        // Căutăm scriptul Entity pe părinte
        parentEntity = GetComponentInParent<Entity>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) parentEntity.SetPlayerInTrigger(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) parentEntity.SetPlayerInTrigger(false);
    }
}