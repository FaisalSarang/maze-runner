// AI USAGE: Written with assistance from Claude (Anthropic, Claude Code).
// I (the student) built the scene, attached and tuned this script, and can explain it.

using UnityEngine;

/// <summary>
/// SENSOR. Put this on an object whose Collider has "Is Trigger" ticked.
/// A trigger doesn't block anything; it just reports when something overlaps it.
/// When the Actor enters, it tells the LevelManager to remove the roadblock.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    public LevelManager levelManager;              // drag the LevelManager object here
    public Color pressedColor = Color.green;

    private bool pressed;

    // Unity calls this automatically when a Rigidbody enters this trigger.
    void OnTriggerEnter(Collider other)
    {
        if (pressed) return;                         // only fire once

        // Only the Actor can press it (not the crate).
        // attachedRigidbody = the Rigidbody that owns the collider that touched us.
        if (other.attachedRigidbody == null) return;
        if (other.attachedRigidbody.GetComponent<PlayerController>() == null) return;

        pressed = true;
        GetComponent<Renderer>().material.color = pressedColor;   // visual feedback
        Debug.Log("[PressurePlate] Sensor triggered by the Actor.");
        levelManager.OnSensorTriggered();
    }
}
