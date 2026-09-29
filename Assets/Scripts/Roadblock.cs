// AI USAGE: Written with assistance from Claude (Anthropic, Claude Code).
// I (the student) built the scene, attached and tuned this script, and can explain it.

using UnityEngine;

/// <summary>
/// ROADBLOCK: a physical door that blocks the route to the DepositZone.
/// It has a KINEMATIC Rigidbody: physics doesn't push it around, but we can move it
/// with MovePosition so the physics engine knows it moved (instead of teleporting).
/// When opened, it sinks into the floor, then turns itself off.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Roadblock : MonoBehaviour
{
    public float sinkDistance = 3.2f;   // should be a bit more than the door's height
    public float sinkSpeed = 3f;        // metres per second

    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool opening;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;          // immovable by collisions
    }

    /// <summary>Called by the LevelManager when the sensor is triggered.</summary>
    public void Open()
    {
        if (opening) return;
        opening = true;
        targetPosition = rb.position + Vector3.down * sinkDistance;
    }

    // Moving a physics object, so this happens in FixedUpdate.
    void FixedUpdate()
    {
        if (!opening) return;

        Vector3 next = Vector3.MoveTowards(rb.position, targetPosition, sinkSpeed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        if (Vector3.Distance(next, targetPosition) < 0.01f)
        {
            opening = false;
            gameObject.SetActive(false);   // fully sunk, remove it completely
        }
    }
}
