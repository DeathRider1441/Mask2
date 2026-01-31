using UnityEngine;
using UnityEngine.UI;

public class ScreenColorController : MonoBehaviour
{
    public static ScreenColorController Instance { get; private set; }
    
    [Header("UI Reference")]
    [SerializeField] private Image screenOverlay; // Image care acoperă tot ecranul
    
    [Header("Color Settings")]
    [SerializeField] private Color targetColor = Color.red; // Culoarea dorită
    [SerializeField] [Range(0f, 1f)] private float colorIntensity = 0.5f; // Intensitatea culorii (0 = transparent, 1 = opac)
    
    [Header("Transition Settings")]
    [SerializeField] private float transitionSpeed = 2f; // Viteza de tranziție
    [SerializeField] private bool useSmoothing = true; // Folosește tranziție smooth sau instant
    
    private Color currentColor;
    private float currentIntensity = 0f;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        // Găsim Image component dacă nu e setat
        if (screenOverlay == null)
        {
            screenOverlay = GetComponent<Image>();
        }
        
        // Setăm culoarea inițială transparent
        if (screenOverlay != null)
        {
            currentColor = targetColor;
            currentColor.a = 0f;
            screenOverlay.color = currentColor;
            
            // Dezactivăm raycast pentru a nu bloca input-ul
            screenOverlay.raycastTarget = false;
        }
    }
    
    private void Update()
    {
        // Update continuu pentru a aplica schimbările din Inspector în timp real
        ApplyColorTransition();
    }
    
    private void ApplyColorTransition()
    {
        if (screenOverlay == null) return;
        
        // Calculăm culoarea target cu intensitatea curentă
        Color targetColorWithAlpha = targetColor;
        targetColorWithAlpha.a = colorIntensity;
        
        if (useSmoothing)
        {
            // Tranziție smooth
            currentColor = Color.Lerp(currentColor, targetColorWithAlpha, Time.deltaTime * transitionSpeed);
        }
        else
        {
            // Tranziție instant
            currentColor = targetColorWithAlpha;
        }
        
        // Aplicăm culoarea
        screenOverlay.color = currentColor;
        currentIntensity = currentColor.a;
    }
    
    // Funcții publice pentru control programatic
    
    public void SetColor(Color color)
    {
        targetColor = color;
    }
    
    public void SetIntensity(float intensity)
    {
        colorIntensity = Mathf.Clamp01(intensity);
    }
    
    public void SetColorAndIntensity(Color color, float intensity)
    {
        targetColor = color;
        colorIntensity = Mathf.Clamp01(intensity);
    }
    
    public void FadeIn(float targetIntensity)
    {
        colorIntensity = Mathf.Clamp01(targetIntensity);
    }
    
    public void FadeOut()
    {
        colorIntensity = 0f;
    }
    
    public void SetTransitionSpeed(float speed)
    {
        transitionSpeed = speed;
    }
    
    public void SetSmoothingEnabled(bool enabled)
    {
        useSmoothing = enabled;
    }
    
    public Color GetCurrentColor()
    {
        return currentColor;
    }
    
    public float GetCurrentIntensity()
    {
        return currentIntensity;
    }
}

 