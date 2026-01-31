using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Stealth/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    [Header("Visual Perception")]
    public float viewDistance = 15f;
    public float viewAngle = 90f;
    
    [Header("Detection Settings")]
    [Tooltip("Cât de repede crește bara de detecție (per secundă)")]
    public float detectionSpeed = 50f; 
    [Tooltip("Cât de repede scade când jucătorul se ascunde")]
    public float coolDownSpeed = 20f;
    [Header("Chase Settings")]
    public float chaseBoredomTime = 5f;

    [Header("Stats")]
    public int maxHealth = 100;
}