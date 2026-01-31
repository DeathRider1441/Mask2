using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    
    private GameObject spawnedKey;

    private void Start()
    {
        SpawnKey();
    }

    public void SpawnKey()
    {
        if (spawnedKey != null)
            Destroy(spawnedKey);

        if (spawnPoints.Length == 0 || keyPrefab == null)
        {
            Debug.LogWarning("KeySpawner: Missing prefab or spawn points!");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform chosenPoint = spawnPoints[randomIndex];
        
        spawnedKey = Instantiate(keyPrefab, chosenPoint.position, chosenPoint.rotation);
    }
}