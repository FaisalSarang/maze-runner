# Coding Assignment #1: Maze Physics Chamber

Unity **6000.3.24f1**, Built-in Render Pipeline, legacy Input Manager.

## How to run
1. Open this folder in Unity Hub (**Add → Add project from disk**).
2. Open `Assets/Scenes/MazeChamber.unity`.
3. Press **Play**.

**Controls:** `W A S D` to move, `R` to restart.

**Goal:** Navigate the maze to the red pressure plate (top-left corner). Stepping on it sinks the orange door on the south wall of the start hall. Then push the brown crate from the start hall, through the doorway, and into the green DepositZone in the room below. The elapsed time is logged to the Console.

## AI usage
The C# scripts in `Assets/Scripts/` and `Assets/Editor/` were written with the help of Claude (Anthropic) via Claude Code; each file says so in a header comment. I set up the project, ran and tested the scene, made some meaningful design changes, tuned the physics values, and can explain how every script works. **The reflection below is my own writing, done without AI.**

---

## Reflection

### 1. What must be true (physically and in script state) for a run to count as success? One way to cheat, and does the design block it?
Ans) For physical conditions, the player must push the box in the center of the zone. Having just an edge overlap is not enough. In the script, this box must have a Payload so that the Actor does not count. But to get to the zone, the player has to unlock a door which physically blocks the only path possible. In the script, "RoadblockRemoved == true" must be set to let the player pass through the door. This is achieved when the player triggers the pressure plate.

One way to cheat would be to directly push the box on the Deposit Zone. However, the design of the door ensures the player or the crate cannot pass through it without triggering the pressure plate first.

### 2. One unexpected behavior while building: what caused it, and what did you change?
Ans) The crate felt tedious to push in the beginning when I did not optimize any of its rigidbody properties. I figured just lowering the value of mass does not solve the problem. The ideal and least frustrating way to move the box was achieved after several rounds of tuning. I understood that having different values of mass, friction and damping create very different interactions with something as simple as pushing a box.

### 3. One design decision: what did you optimize for, and what did you give up?
Ans) I used Claude to guide me with the scene creation process. The RigidBody values which Claude gave for each object was intended to simulate a realistic environment with different forces of physics at play. I felt that for a game, realism should not be the goal since it makes the game less enjoyable and more tedious. For this reason, I reduced the values of friction of everything. In fact, for the walls I set friction as 0. This ensured that if the player sticks the crate to a wall, it is still possible to drag the crate while sliding along the wall. In short, for optimizing functionality I gave up realism.

### 4. Weakest part of the system right now: what would you change first, and why?
Ans) I think the weakest part of the system right now is the lack of an option to reset the position of the crate. If the player pushes the crate to a corner, the game essentially gets softlocked and there is no other way to get out of it except by resetting. I would change this because it may feel annoying to players. Maybe an undo button would help. However, the scope of what is undone is another task in itself which is why it is not addressed in this system.
