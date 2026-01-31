using UnityEngine;
using UnityEngine.UI;

public class DetectionBar : MonoBehaviour
{
    private Entity enemyEntity;
    public GameObject barUI;      // Referință către panoul care conține bara (pentru a-l ascunde la 0%)
    public Image fillImage;       // Imaginea de tip "Filled" (bara roșie/galbenă)
    private Camera mainCamera;

    void Start()
    {
        enemyEntity = GetComponentInParent<Entity>();
        mainCamera = Camera.main;
        
        if (barUI != null) barUI.SetActive(false); // Ascundem la început
    }

    void LateUpdate()
    {
        if (enemyEntity == null || enemyEntity.isDead)
        {
            if (barUI != null) barUI.SetActive(false);
            return;
        }

        float detection = enemyEntity.currentDetection;

        // Afișăm bara doar dacă există puțină detecție
        if (detection > 0)
        {
            if (barUI != null) barUI.SetActive(true);
            
            // Actualizăm umplerea barei (0.0 la 1.0)
            if (fillImage != null)
            {
                fillImage.fillAmount = detection / 100f;
                
                // Opțional: Schimbăm culoarea din Galben în Roșu pe măsură ce crește
                fillImage.color = Color.Lerp(Color.yellow, Color.red, detection / 100f);
            }

            // Facem bara să se uite mereu spre cameră (Billboard effect)
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }
        else
        {
            if (barUI != null) barUI.SetActive(false);
        }
    }
}