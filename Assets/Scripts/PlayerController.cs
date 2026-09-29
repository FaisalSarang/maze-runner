// AI USAGE: Written with assistance from Claude (Anthropic, Claude Code).
// I (the student) built the scene, attached and tuned this script, and can explain it.

using UnityEngine;

/// <summary>
/// WASD-controlled Actor.
/// Input is READ in Update (runs every rendered frame, so no key press is missed).
/// Force is APPLIED in FixedUpdate (runs at the fixed physics rate, 50x/sec),
/// so movement is the same on fast and slow computers.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Force (Newtons) pushed in the input direction every physics step.")]
    public float moveForce = 40f;

    [Tooltip("Stop adding force in a direction once we're moving faster than this (m/s).")]
    public float maxSpeed = 7f;

    private Rigidbody rb;
    private Vector3 input;   // written in Update, read in FixedUpdate

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float x = 0f, z = 0f;
        if (Input.GetKey(KeyCode.W)) z += 1f;   // W = forward (+Z, away from camera)
        if (Input.GetKey(KeyCode.S)) z -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;   // D = right (+X)
        if (Input.GetKey(KeyCode.A)) x -= 1f;

        input = new Vector3(x, 0f, z);

        // W+D together gives length ~1.41, which would make diagonals faster. Clamp to 1.
        if (input.sqrMagnitude > 1f) input.Normalize();
    }

    void FixedUpdate()
    {
        if (input == Vector3.zero) return;

        // Horizontal velocity only (ignore falling).
        Vector3 vel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Dot product = how fast we're already moving IN the input direction.
        // Only push if below the cap, so we can't accelerate forever,
        // but we can still turn/brake instantly.
        if (Vector3.Dot(vel, input) < maxSpeed)
        {
            rb.AddForce(input * moveForce, ForceMode.Force);
        }
    }
}
