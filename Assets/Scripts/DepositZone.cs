// AI USAGE: Written with assistance from Claude (Anthropic, Claude Code).
// I (the student) built the scene, attached and tuned this script, and can explain it.

using UnityEngine;

/// <summary>
/// GOAL TRIGGER. Put on an object whose BoxCollider has "Is Trigger" ticked.
/// A deposit only counts when ALL of these are true:
///   1. the object is the Payload (not the Actor),
///   2. the roadblock was already removed,
///   3. the Payload entered AFTER the unlock (sitting in the zone beforehand doesn't count),
///   4. the Payload's center is inside the zone (just touching the edge isn't enough).
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class DepositZone : MonoBehaviour
{
    public LevelManager levelManager;   // drag the LevelManager object here

    private BoxCollider zone;
    private bool enteredAfterUnlock;

    void Awake()
    {
        zone = GetComponent<BoxCollider>();
        zone.isTrigger = true;          // safety: make sure it's a trigger
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsPayload(other)) return;

        if (levelManager.RoadblockRemoved)
        {
            enteredAfterUnlock = true;
        }
        else
        {
            Debug.Log("[DepositZone] Payload entered before the roadblock was removed. Does not count.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Leaving resets it, so it has to come back in (after unlock) to count.
        if (IsPayload(other)) enteredAfterUnlock = false;
    }

    // Called every physics step while something is inside the trigger.
    void OnTriggerStay(Collider other)
    {
        if (!enteredAfterUnlock || levelManager.LevelComplete || !IsPayload(other)) return;

        // Rule 4: the crate's center of mass must be within the zone's box.
        if (zone.bounds.Contains(other.attachedRigidbody.worldCenterOfMass))
        {
            levelManager.OnPayloadDeposited();
        }
    }

    private static bool IsPayload(Collider other)
    {
        return other.attachedRigidbody != null
            && other.attachedRigidbody.GetComponent<Payload>() != null;
    }
}
