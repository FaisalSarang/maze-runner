// AI USAGE: Written with assistance from Claude (Anthropic, Claude Code).
// Editor-only helper that builds the maze scene. I (the student) reviewed and tuned the result.

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Menu: Tools > Build Maze Chamber Scene
/// Rebuilds Assets/Scenes/MazeChamber.unity from the ASCII layout below (overwrites it).
/// To change the maze, edit the strings and run the menu item again.
/// </summary>
public static class MazeSceneBuilder
{
    // Legend:  # wall   . floor   S actor start   P payload (crate)
    //          T pressure plate (sensor)   D door (roadblock)   Z deposit zone
    // Row 0 is the NORTH edge (+Z), so W on the keyboard moves "up" the map.
    static readonly string[] Layout =
    {
        "###############",
        "#T....#.......#",
        "####..#.#####.#",
        "#.....#.#...#.#",
        "#.#####.#.#.#.#",
        "#.#.....#.#...#",
        "#.#.#####.#####",
        "#...#.....#...#",
        "###.#.###.#.#.#",
        "#.....#.....#.#",
        "#.###########.#",
        "#.............#",
        "#.S......P....#",
        "#.............#",
        "######D########",
        "#####...#######",
        "#####.Z.#######",
        "#####...#######",
        "###############",
    };

    const float Cell = 3f;        // each grid cell is 3x3 metres
    const float WallHeight = 3f;
    const float CrateSize = 1.8f; // must stay well under Cell so it fits through the door
    const string ScenePath = "Assets/Scenes/MazeChamber.unity";

    [MenuItem("Tools/Build Maze Chamber Scene")]
    public static void Build()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets", "PhysicsMaterials");
        EnsureFolder("Assets", "Scenes");

        // ---- Physics materials: deliberately different per object ----
        var floorPhys = PhysMat("FloorPhys", 0.5f,  0.6f, PhysicsMaterialCombine.Average);
        var wallPhys  = PhysMat("WallPhys",  0f,    0f,   PhysicsMaterialCombine.Minimum); // slide along walls
        var actorPhys = PhysMat("ActorPhys", 0.1f,  0.1f, PhysicsMaterialCombine.Minimum); // glides
        var cratePhys = PhysMat("CratePhys", 0.25f, 0.3f, PhysicsMaterialCombine.Average); // lowered twice: pushing was too slow

        // ---- Render materials ----
        var floorMat = Mat("Floor", new Color(0.35f, 0.37f, 0.40f));
        var wallMat  = Mat("Wall",  new Color(0.18f, 0.22f, 0.30f));
        var actorMat = Mat("Actor", new Color(0.15f, 0.55f, 1f));
        var crateMat = Mat("Crate", new Color(0.62f, 0.42f, 0.22f));
        var doorMat  = Mat("Door",  new Color(1f, 0.45f, 0.10f));
        var plateMat = Mat("Plate", new Color(0.90f, 0.15f, 0.15f));
        var zoneMat  = Mat("Zone",  new Color(0.20f, 0.85f, 0.35f));

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        int rows = Layout.Length;
        int cols = Layout[0].Length;

        // ---- Floor covering the whole grid ----
        var floor = Box("Floor",
            new Vector3((cols - 1) * Cell * 0.5f, -0.1f, (rows - 1) * Cell * 0.5f),
            new Vector3(cols * Cell, 0.2f, rows * Cell), floorMat, floorPhys, null);
        floor.isStatic = true;

        var maze = new GameObject("Maze").transform;
        var levelManager = new GameObject("LevelManager").AddComponent<LevelManager>();
        GameObject actor = null;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector3 p = new Vector3(c * Cell, 0f, (rows - 1 - r) * Cell); // cell center on the floor

                switch (Layout[r][c])
                {
                    case '#':
                        Box($"Wall_{r}_{c}", p + Vector3.up * WallHeight / 2,
                            new Vector3(Cell, WallHeight, Cell), wallMat, wallPhys, maze).isStatic = true;
                        break;

                    case 'S':
                        actor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        actor.name = "Actor";
                        actor.transform.position = p + Vector3.up * 0.5f;
                        actor.GetComponent<Renderer>().sharedMaterial = actorMat;
                        actor.GetComponent<Collider>().sharedMaterial = actorPhys;
                        var arb = actor.AddComponent<Rigidbody>();
                        arb.mass = 2f;
                        arb.linearDamping = 0.5f;
                        arb.constraints = RigidbodyConstraints.FreezeRotation; // slides instead of rolling
                        arb.interpolation = RigidbodyInterpolation.Interpolate;
                        arb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        actor.AddComponent<PlayerController>();
                        break;

                    case 'P':
                        var crate = Box("Crate", p + Vector3.up * CrateSize / 2,
                            Vector3.one * CrateSize, crateMat, cratePhys, null);
                        var crb = crate.AddComponent<Rigidbody>();
                        crb.mass = 3f;              // 1.5x the actor (was 4: too sluggish)
                        crb.linearDamping = 0.8f;   // still stops soon after you let go
                        crb.angularDamping = 2f;
                        crb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // no tipping
                        crb.interpolation = RigidbodyInterpolation.Interpolate;
                        crate.AddComponent<Payload>();
                        break;

                    case 'D':
                        var door = Box("Door", p + Vector3.up * WallHeight / 2,
                            new Vector3(Cell, WallHeight, 0.8f), doorMat, wallPhys, null);
                        var roadblock = door.AddComponent<Roadblock>();   // RequireComponent adds the Rigidbody
                        door.GetComponent<Rigidbody>().isKinematic = true;
                        roadblock.sinkDistance = WallHeight + 0.2f;
                        levelManager.roadblock = roadblock;
                        break;

                    case 'T':
                        var plate = Box("PressurePlate", p + Vector3.up * 0.05f,
                            new Vector3(2.2f, 0.1f, 2.2f), plateMat, null, null);
                        var plateCol = plate.GetComponent<BoxCollider>();
                        plateCol.isTrigger = true;
                        plateCol.size = new Vector3(1, 15, 1);   // 0.1 * 15 = 1.5m tall trigger, reaches the Actor
                        plate.AddComponent<PressurePlate>().levelManager = levelManager;
                        break;

                    case 'Z':
                        var zone = Box("DepositZone", p + Vector3.up * 0.02f,
                            new Vector3(2.8f, 0.04f, 2.8f), zoneMat, null, null);
                        var zoneCol = zone.GetComponent<BoxCollider>();
                        zoneCol.isTrigger = true;
                        zoneCol.size = new Vector3(1, 50, 1);    // 0.04 * 50 = 2m tall
                        zoneCol.center = new Vector3(0, 25, 0);  // sits on the floor, extends upward
                        zone.AddComponent<DepositZone>().levelManager = levelManager;
                        break;
                }
            }
        }

        // ---- Follow camera ----
        var cam = Camera.main;
        cam.transform.position = actor.transform.position + new Vector3(0f, 16f, -9f);
        cam.transform.LookAt(actor.transform.position);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.09f, 0.12f);
        cam.gameObject.AddComponent<CameraFollow>().target = actor.transform;

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        Debug.Log("[MazeSceneBuilder] Built " + ScenePath);
    }

    static GameObject Box(string name, Vector3 pos, Vector3 scale, Material mat, PhysicsMaterial phys, Transform parent)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        if (parent != null) go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        if (phys != null) go.GetComponent<Collider>().sharedMaterial = phys;
        return go;
    }

    static PhysicsMaterial PhysMat(string name, float dyn, float stat, PhysicsMaterialCombine combine)
    {
        string path = $"Assets/PhysicsMaterials/{name}.physicMaterial";
        var m = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(path);
        if (m == null) { m = new PhysicsMaterial(name); AssetDatabase.CreateAsset(m, path); }
        m.dynamicFriction = dyn;
        m.staticFriction = stat;
        m.frictionCombine = combine;
        m.bounciness = 0f;
        m.bounceCombine = PhysicsMaterialCombine.Minimum;
        EditorUtility.SetDirty(m);
        return m;
    }

    static Material Mat(string name, Color color)
    {
        string path = $"Assets/Materials/{name}.mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null) { m = new Material(Shader.Find("Standard")); AssetDatabase.CreateAsset(m, path); }
        m.color = color;
        EditorUtility.SetDirty(m);
        return m;
    }

    static void EnsureFolder(string parent, string child)
    {
        if (!AssetDatabase.IsValidFolder($"{parent}/{child}")) AssetDatabase.CreateFolder(parent, child);
    }
}
