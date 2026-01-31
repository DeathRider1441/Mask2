using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[System.Serializable]
public struct PatrolPoint
{
    public Transform point;
    public float waitTime;
}

public abstract class NPCBase : Entity
{
    [Header("Movement")]
    [SerializeField] private float defaultSpeed = 3.5f;
    public NavMeshAgent Agent { get; private set; }

    [Header("Stealth Patrol")]
    public List<PatrolPoint> patrolRoute = new List<PatrolPoint>();
    [HideInInspector] public int currentPatrolIndex = 0;
    [HideInInspector] public bool isForward = true;

    // State Machine
    public enum NPCStateID { Patrol, Wait, Idle, Dead, Investigate,Attack, Chase }
    public INPCState currentState;
    public NPCStateID CurrentStateID { get; private set; }
    
    public Animator animator;
    private bool isChangingState = false;

    // --- INSTANȚE STĂRI (Declarate la început pentru acces direct) ---
    public PatrolState patrolState = new PatrolState();
    public WaitState waitState = new WaitState();
    public ChaseState chaseState = new ChaseState();
    public InvestigateState investigateState = new InvestigateState();
    public AttackState attackState = new AttackState();

    protected virtual void Awake()
    {
        base.Awake();
        Agent = GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
        base.Start(); // Din Entity: Health & Visuals
        
        if (Agent != null) Agent.speed = defaultSpeed;

        // Pornim patrularea dacă avem traseu
        if (patrolRoute.Count > 0) ToPatrol();
        else ToIdle();
    }

    public void ChangeState(INPCState newState)
    {
        if (isChangingState || (currentState != null && currentState.StateID == newState.StateID))
        if (Agent == null || !Agent.enabled) return;

        isChangingState = true;
        currentState?.ExitState(this);
        currentState = newState;
        CurrentStateID = newState.StateID;
        currentState.EnterState(this);
        isChangingState = false;
    }
    


    protected override void HandleDetectionLogic()
    {
        // Verificăm dacă masca e activă și funcțională
        bool isInvisible = false;
        if (MaskSystem.Instance != null)
        {
            isInvisible = MaskSystem.Instance.IsPlayerGhosted();
        }

        // Dacă are masca, inamicul nu îl vede (scade bara de detecție)
        bool effectivelyCanSee = canSeePlayer && !isInvisible;

        if (effectivelyCanSee)
            currentDetection += data.detectionSpeed * Time.deltaTime;
        else
            currentDetection -= data.coolDownSpeed * Time.deltaTime;

        currentDetection = Mathf.Clamp(currentDetection, 0, 100);
        
        // Raportăm la ScreenColorController pentru roșul de pe margini
        if (ScreenColorController.Instance != null)
            ScreenColorController.Instance.ReportDetection(currentDetection);

        // Verificăm dacă inamicul trebuie să treacă la atac/urmărire
        if (currentDetection >= 100 && CurrentStateID != NPCStateID.Chase && CurrentStateID != NPCStateID.Attack)
        {
            OnPlayerDetected();
        }
    }

    protected override void Update()
    {
        if (Agent == null || !Agent.isOnNavMesh) return;

        base.Update();
        currentState?.DoState(this);
        Debug.Log($"{gameObject.name} este în starea: {CurrentStateID}");

        // Sync Animator
        if (animator != null)
        {
            animator.SetInteger("State", (int)CurrentStateID);
        }
    }

    protected override void OnPlayerDetected()
    {
        // Dacă sunt deja la bătaie, nu mă mai pune să "încep" urmărirea
        if (CurrentStateID == NPCStateID.Attack || CurrentStateID == NPCStateID.Chase) return;
        
        ToChase();
    }

    void OnEnable() 
    {
        GameEvents.OnNoiseMade += HandleNoise;
    }

    void OnDisable() 
    {
        GameEvents.OnNoiseMade -= HandleNoise;
    }

    private void HandleNoise(Vector3 position, float range)
    {
        // Ignorăm dacă e mort sau în urmărire directă
        if (CurrentStateID == NPCStateID.Chase || CurrentStateID == NPCStateID.Dead) return;

        float dist = Vector3.Distance(transform.position, position);
        
        if (dist <= range) 
        {
            // Dacă este DEJA în starea de investigație, doar îi actualizăm ținta
            if (CurrentStateID == NPCStateID.Investigate)
            {
                investigateState.SetTarget(position);
                // Forțăm re-intrarea sau apelăm o metodă de refresh
                investigateState.RefreshTarget(this, position); 
            }
            else
            {
                // Dacă era în Patrol/Wait, schimbăm starea normal
                investigateState.SetTarget(position);
                ChangeState(investigateState);
            }

            currentDetection = Mathf.Max(currentDetection, 30f); 
        }
    }

    // Metodă helper pentru schimbarea stării
    public void ToInvestigate(Vector3 pos)
    {
        // --- IMPLEMENTARE CORECTATĂ ---
        investigateState.SetTarget(pos);
        ChangeState(investigateState);
    }

    // 2. Implementarea metodei ToAttack
    public void ToAttack() 
    {
        ChangeState(attackState);
    }

    // --- Tranziții ---
    public virtual void ToPatrol() => ChangeState(patrolState);
    
    public virtual void ToWait(float duration) 
    {
        waitState.SetDuration(duration);
        ChangeState(waitState);
    }
    
    public void ToIdle()
    {
        if (Agent.isOnNavMesh) Agent.isStopped = true;
        // Aici poți implementa o logică de IdleState dacă ai nevoie
    }
    
    public void ToChase() => ChangeState(chaseState);

    // === LOGICĂ DE MOARTE (OVERRIDE) ===
    protected override void Die()
    {
        // 1. Executăm logica de bază din Entity (Loot, IsDead = true, Salvare stare)
        base.Die();

        // 2. Oprim AI-ul imediat
        if (Agent != null)
        {
            Agent.isStopped = true;
            Agent.enabled = false;
        }

        // 3. Feedback vizual/animație
        if (animator != null)
        {
            animator.SetTrigger("Die");
            animator.SetInteger("State", (int)NPCStateID.Dead);
        }

        // Dezabonăm de la evenimente ca să nu dea erori post-mortem
        GameEvents.OnNoiseMade -= HandleNoise;

        Debug.Log($"<color=red>[Stealth]</color> {gameObject.name} a fost eliminat.");
    }
}