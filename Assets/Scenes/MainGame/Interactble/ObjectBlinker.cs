using UnityEngine;

public class ObjectBlinker : MonoBehaviour
{
    [Header("Blink Settings")]
    [SerializeField] private Color blinkColor = Color.white; // Culoarea sclipirii
    [SerializeField] private float speed = 2.0f;             // Viteza pulsației
    [SerializeField] private float minIntensity = 0f;        // Intensitatea minimă
    [SerializeField] private float maxIntensity = 1.5f;      // Intensitatea maximă

    private Material targetMaterial;
    private static readonly int EmissionColorProperty = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        // Luăm materialul obiectului (facem o instanță unică să nu le afectăm pe toate)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            targetMaterial = renderer.material;
            // Activăm keyword-ul de emisie (important pentru unele shadere)
            targetMaterial.EnableKeyword("_EMISSION");
        }
        else
        {
            Debug.LogError("Obiectul nu are un Renderer!");
            enabled = false;
        }
    }

    void Update()
    {
        // Calculăm intensitatea folosind sinus (oscilează între -1 și 1, apoi o mapăm)
        float lerp = (Mathf.Sin(Time.time * speed) + 1.0f) / 2.0f;
        float currentIntensity = Mathf.Lerp(minIntensity, maxIntensity, lerp);

        // Aplicăm culoarea cu intensitatea calculată
        // Folosim formatul HDR (înmulțim culoarea cu intensitatea)
        Color finalColor = blinkColor * currentIntensity;
        targetMaterial.SetColor(EmissionColorProperty, finalColor);
    }
}