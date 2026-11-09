using UnityEditor;
using UnityEngine;

// Editor menu: make a new random obstacle layout (old obstacles are removed). Ctrl+Z works.
public static class LevelGeneratorMenu
{
    private const string UndoName = "Generate Obstacles";

    [MenuItem("Tools/Level/Generate Obstacles")]
    private static void Generate()
    {
        LevelGenerator generator = FindGenerator();
        if (generator == null) return;

        Undo.SetCurrentGroupName(UndoName);
        int undoGroup = Undo.GetCurrentGroup();

        Clear(generator.ObstaclesRoot);

        var layout = generator.CreateLayout(System.Environment.TickCount);
        foreach (Pose pose in layout)
        {
            var obstacle = (GameObject)PrefabUtility.InstantiatePrefab(generator.ObstaclePrefab, generator.ObstaclesRoot);
            obstacle.transform.SetPositionAndRotation(pose.position, pose.rotation);
            obstacle.transform.localScale = Vector3.one * generator.ObstacleSize;
            Undo.RegisterCreatedObjectUndo(obstacle, UndoName);
        }

        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log($"Generated {layout.Count} obstacles.");
    }

    [MenuItem("Tools/Level/Clear Obstacles")]
    private static void ClearMenu()
    {
        LevelGenerator generator = FindGenerator();
        if (generator != null) Clear(generator.ObstaclesRoot);
    }

    private static void Clear(Transform root)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            Undo.DestroyObjectImmediate(root.GetChild(i).gameObject);
        }
    }

    private static LevelGenerator FindGenerator()
    {
        var generator = Object.FindFirstObjectByType<LevelGenerator>();
        if (generator == null) Debug.LogWarning("No LevelGenerator in the scene.");
        return generator;
    }
}
