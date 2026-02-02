using UnityEngine;

public class PlayerClimbing : MonoBehaviour
{
    [Header("Settings")]
    public float climbSpeed = 5f;
    private bool isCollidingWithLadder = false;
    
    private Rigidbody rb;
    private FirstPersonController fpc; // Referință către controller-ul tău

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        fpc = GetComponent<FirstPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isCollidingWithLadder = true;
            Debug.Log("On Ladder: Press W to climb");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isCollidingWithLadder = false;
            rb.useGravity = true; // Revenim la normal
        }
    }

    void FixedUpdate()
    {
        if (isCollidingWithLadder)
        {
            // Dezactivăm gravitația cât timp suntem pe scară
            rb.useGravity = false;

            // Luăm input-ul vertical (W/S)
            float verticalInput = Input.GetAxis("Vertical");

            // Aplicăm mișcarea pe verticală
            Vector3 climbVelocity = new Vector3(rb.linearVelocity.x, verticalInput * climbSpeed, rb.linearVelocity.z);
            rb.linearVelocity = climbVelocity;
        }
    }
}