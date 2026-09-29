// AI USAGE: Written with assistance from Claude (Anthropic, Claude Code).
// I (the student) built the scene, attached and tuned this script, and can explain it.

using UnityEngine;

/// <summary>
/// "Tag" component: marks this object as the Payload (the crate).
/// The DepositZone checks for this component, so the Actor walking into
/// the zone can't complete the level. Only the crate can.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Payload : MonoBehaviour
{
}
