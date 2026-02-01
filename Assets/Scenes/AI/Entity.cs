using UnityEngine;

public class Entity : MonoBehaviour
{
    public EnemyData data;

    [Header("Instanced Stats")]
    public int currentHealth;
    public bool isDead = false;

    [Header("Stealth Status")]
    [Range(0, 100)] public float currentDetection = 0f;
    public bool canSeePlayer;
    public bool isPlayerInTrigger; // Verifică dacă playerul e în conul fizic

    [Header("Detection Components")]
    [Tooltip("Obiectul care are Trigger-ul pentru conul de vedere.")]
    public GameObject detectionTriggerObject;

    public Transform playerTransform;
    protected UnityEngine.AI.NavMeshAgent agent;

    protected virtual void Awake()
    {
        // 1. Setează viața din ScriptableObject la inițializarea obiectului
        if (data != null)
        {
            currentHealth = data.maxHealth;
        }
    }

    protected virtual void Start()
    {
        // Căutăm jucătorul după Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        
        // 2. Aplicăm viteza de mers din date
        if (agent != null && data != null) 
            agent.speed = data.walkSpeed;
    }

    protected virtual void Update()
    {
        // Dacă e mort, nu mai procesăm detecția
        if (isDead || playerTransform == null || data == null) return;

        CheckDetection();
        // HandleDetectionLogic();
    }

    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }
    

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"<color=black><b>{gameObject.name} a murit.</b></color>");

        // Oprim AI-ul imediat la moarte
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
    }

    private void CheckDetection()
    {
        if (!isPlayerInTrigger)
        {
            canSeePlayer = false;
            return;
        }

        // Calculăm direcția către jucător
        // Adăugăm un mic offset pe Y (Vector3.up * 0.5f) pentru a ținti corpul, nu picioarele
        Vector3 targetPos = playerTransform.position + Vector3.up * 0.5f;
        Vector3 directionToPlayer = (targetPos - (transform.position + Vector3.up)).normalized;
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("Ignore Raycast"); 

        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, data.viewDistance, layerMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // Verificăm dacă jucătorul este crouch pentru a reduce viteza de detecție (Opțional)
                canSeePlayer = true;
                return;
            }
        }
        
        canSeePlayer = false;
    }

    public void SetPlayerInTrigger(bool state)
    {
        isPlayerInTrigger = state;
    }

    protected virtual void HandleDetectionLogic()
    {
        // if (canSeePlayer)
        //     currentDetection += data.detectionSpeed * Time.deltaTime;
        // else
        //     currentDetection -= data.coolDownSpeed * Time.deltaTime;

        // currentDetection = Mathf.Clamp(currentDetection, 0, 100);

        // ADAUGĂ ACEASTĂ VERIFICARE:
        // Trigerăm detecția DOAR dacă bara abia a ajuns la 100. 
        // Dacă e deja 100, nu mai chemăm OnPlayerDetected() continuu.
        // if (currentDetection >= 100 && CurrentStateID != NPCBase.NPCStateID.Chase && CurrentStateID != NPCBase.NPCStateID.Attack) 
        // {
        //     OnPlayerDetected();
        // }
    }

    protected virtual void OnPlayerDetected()
    {
        // Debug.Log("<color=red>TE-AM VĂZUT!</color>");
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal) angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    // --- Editor Tools ---
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (data == null) return;

        // --- 1. SETĂRI VIZUALE ---
        // Folosim culori semi-transparente pentru volum
        Color rangeColor = isDead ? new Color(0.5f, 0.5f, 0.5f, 0.1f) : new Color(1f, 1f, 1f, 0.05f);
        Color viewColor = canSeePlayer ? new Color(1f, 0f, 0f, 0.3f) : new Color(1f, 0.92f, 0.016f, 0.1f);
        
        Vector3 eyePos = transform.position + Vector3.up;

        // --- 2. DESENARE SFERĂ DISTANȚĂ ---
        Gizmos.color = rangeColor;
        Gizmos.DrawWireSphere(transform.position, data.viewDistance);

        // --- 3. DESENARE CON VIZIBIL (EVANTAI) ---
        Gizmos.color = viewColor;
        int segments = 20; // Cu cât e mai mare, cu atât e mai fin cercul
        Vector3 previousLine = DirFromAngle(-data.viewAngle / 2, false) * data.viewDistance;
        
        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = -data.viewAngle / 2 + (data.viewAngle / segments) * i;
            Vector3 nextLine = DirFromAngle(currentAngle, false) * data.viewDistance;
            
            // Desenăm "felia" de con
            Gizmos.DrawLine(eyePos, eyePos + nextLine);
            Gizmos.DrawLine(eyePos + previousLine, eyePos + nextLine);
            
            previousLine = nextLine;
        }
        
        // Liniile de margine ale conului (mai groase vizual)
        Gizmos.color = viewColor * 2f; // Puțin mai opace
        Gizmos.DrawLine(eyePos, eyePos + DirFromAngle(-data.viewAngle / 2, false) * data.viewDistance);
        Gizmos.DrawLine(eyePos, eyePos + DirFromAngle(data.viewAngle / 2, false) * data.viewDistance);

        // --- 4. LINIA DE DETECȚIE SPRE JUCĂTOR ---
        if (playerTransform != null && !isDead)
        {
            float dist = Vector3.Distance(eyePos, playerTransform.position + Vector3.up);
            if (dist <= data.viewDistance)
            {
                // Schimbăm culoarea liniei în funcție de cât de detectat este jucătorul
                // 0 = Galben, 100 = Roșu aprins
                Gizmos.color = Color.Lerp(Color.yellow, Color.red, currentDetection / 100f);
                
                // Desenăm o linie solidă dacă îl vede, punctată (sau deloc) dacă nu
                if (canSeePlayer)
                {
                    Gizmos.DrawLine(eyePos, playerTransform.position + Vector3.up);
                    // Desenăm un mic cub pe jucător când e vizibil
                    Gizmos.DrawCube(playerTransform.position + Vector3.up, Vector3.one * 0.3f);
                }
            }
        }
    }
#endif
}