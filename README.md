# Coding Assignment #1: Maze Physics Chamber

Unity **6000.3.7f1**, Built-in Render Pipeline, legacy Input Manager.

## How to run
1. Open this folder in Unity Hub (**Add → Add project from disk**).
2. Open `Assets/Scenes/MazeChamber.unity`.
3. Press **Play**.

**Controls:** `W A S D` to move, `R` to restart.

**Goal:** Navigate the maze to the red pressure plate (top-left corner). Stepping on it sinks the orange door on the south wall of the start hall. Then push the brown crate from the start hall, through the doorway, and into the green DepositZone in the room below. The elapsed time is logged to the Console.

## Scripts
| Script | On | Role |
|---|---|---|
| `PlayerController.cs` | Actor | Reads WASD in `Update`, applies `AddForce` in `FixedUpdate`, with a soft speed cap |
| `Payload.cs` | Crate | Marker component so only the crate can complete the level |
| `PressurePlate.cs` | Plate (trigger) | Sensor: when the Actor enters, tells the LevelManager to remove the roadblock |
| `Roadblock.cs` | Door | Kinematic Rigidbody that sinks with `MovePosition` in `FixedUpdate`, then disables itself |
| `DepositZone.cs` | Goal (trigger) | Counts only if: it's the Payload, the door is open, the crate entered after unlock, and its center is inside |
| `LevelManager.cs` | Empty object | Level state, logs, timer, on-screen HUD, R to restart |
| `CameraFollow.cs` | Main Camera | Smooth follow of the Actor in `LateUpdate` |
| `Editor/MazeSceneBuilder.cs` | (editor) | **Tools → Build Maze Chamber Scene** builds the scene from an ASCII maze layout |

## Physics tuning (intentionally different)
| Object | Mass | Linear damping | Friction (dyn / static, combine) |
|---|---|---|---|
| Actor | 2 | 0.5 | 0.1 / 0.1, Minimum |
| Crate (1.8 m cube) | 3 | 0.8 | 0.25 / 0.3, Average |
| Floor | n/a | n/a | 0.5 / 0.6, Average |
| Walls / Door | n/a | n/a | 0 / 0, Minimum |

## Logs
- `[PressurePlate] Sensor triggered by the Actor.`
- `[LevelManager] Roadblock removed at X.XXs. The door is open!`
- `[DepositZone] Payload entered before the roadblock was removed. Does not count.` (if attempted)
- `[LevelManager] SUCCESS! Payload deposited. Elapsed time from Play: X.XX seconds.`

## AI usage
The C# scripts in `Assets/Scripts/` and `Assets/Editor/` were written with the help of Claude (Anthropic) via Claude Code; each file says so in a header comment. I set up the project, ran and tested the scene, tuned the physics values, and can explain how every script works. **The reflection below is my own writing, done without AI.**

---

## Reflection

### 1. What must be true (physically and in script state) for a run to count as success? One way to cheat, and does the design block it?
_TODO_

### 2. One unexpected behavior while building: what caused it, and what did you change?
_TODO_

### 3. One design decision: what did you optimize for, and what did you give up?
_TODO_

### 4. Weakest part of the system right now: what would you change first, and why?
_TODO_
