// AI USAGE: Written with assistance from Claude (Anthropic, Claude Code).
// I (the student) built the scene, attached and tuned this script, and can explain it.

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The "referee". Holds the level state (is the roadblock removed? is the level done?)
/// and the timer. The sensor and the DepositZone REPORT to this object; it decides
/// what counts. Put this on an empty GameObject called "LevelManager".
/// </summary>
public class LevelManager : MonoBehaviour
{
    public Roadblock roadblock;   // drag the Door object here

    // Other scripts can READ these, but only LevelManager can change them.
    public bool RoadblockRemoved { get; private set; }
    public bool LevelComplete { get; private set; }

    private float startTime;
    private float finishTime;
    private string status = "Find the pressure plate to open the door.";

    void Start()
    {
        startTime = Time.time;   // "Play" moment for this run (resets on R restart too)
    }

    void Update()
    {
        // R = restart (useful if the crate gets stuck in a corner).
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>Called by PressurePlate.</summary>
    public void OnSensorTriggered()
    {
        if (RoadblockRemoved) return;

        RoadblockRemoved = true;
        roadblock.Open();
        Debug.Log($"[LevelManager] Roadblock removed at {Time.time - startTime:F2}s. The door is open!");
        status = "Door open! Push the crate into the DepositZone.";
    }

    /// <summary>Called by DepositZone.</summary>
    public void OnPayloadDeposited()
    {
        if (LevelComplete) return;

        // Double-check: even if the crate somehow got there, no unlock = no win.
        if (!RoadblockRemoved)
        {
            Debug.LogWarning("[LevelManager] Deposit ignored: roadblock still active.");
            return;
        }

        LevelComplete = true;
        finishTime = Time.time - startTime;
        Debug.Log($"[LevelManager] SUCCESS! Payload deposited. Elapsed time from Play: {finishTime:F2} seconds.");
        status = $"SUCCESS! Time: {finishTime:F2}s  (R to play again)";
    }

    // Simple on-screen text (no Canvas/UI setup needed).
    void OnGUI()
    {
        float t = LevelComplete ? finishTime : Time.time - startTime;
        var style = new GUIStyle(GUI.skin.box) { fontSize = 18, alignment = TextAnchor.UpperLeft };
        GUI.Box(new Rect(10, 10, 520, 80),
            $"Time: {t:F1}s   Door: {(RoadblockRemoved ? "OPEN" : "LOCKED")}\n{status}\nWASD move | R restart",
            style);
    }
}
