// AI USAGE: Written with assistance from Claude (Anthropic, Claude Code).
// I (the student) built the scene, attached and tuned this script, and can explain it.

using UnityEngine;

/// <summary>Angled top-down camera that smoothly follows the Actor.</summary>
public class CameraFollow : MonoBehaviour
{
    public Transform target;                          // drag the Actor here
    public Vector3 offset = new Vector3(0f, 16f, -9f); // above and behind the Actor
    public float smoothTime = 0.15f;                  // lower = snappier

    private Vector3 velocity;

    // LateUpdate runs AFTER all Update calls, so the Actor has already moved
    // this frame. Following it here avoids camera jitter.
    void LateUpdate()
    {
        if (target == null) return;
        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, smoothTime);
        transform.LookAt(target.position);
    }
}
