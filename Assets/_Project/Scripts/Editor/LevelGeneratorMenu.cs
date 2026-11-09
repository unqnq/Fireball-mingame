using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Editor menu: make a new random obstacle layout (old obstacles are removed). Ctrl+Z works.
// Only balanced layouts are used: start size must clear the path with config.balanceMargin extra.
public static class LevelGeneratorMenu
{
    private const string UndoName = "Generate Obstacles";
    private const int MaxAttempts = 100;

    [MenuItem("Tools/Level/Generate Obstacles")]
    private static void Generate()
    {
        LevelGenerator generator = FindGenerator();
        if (generator == null) return;

        Undo.SetCurrentGroupName(UndoName);
        int undoGroup = Undo.GetCurrentGroup();

        Clear(generator.ObstaclesRoot);

        // try random layouts until one is balanced
        int seed = System.Environment.TickCount;
        List<Pose> layout = null;
        float need = 0f;
        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            layout = generator.CreateLayout(seed + attempt);
            need = generator.EstimateRequiredSize(layout);
            if (generator.IsBalanced(need)) break;
        }
        foreach (Pose pose in layout)
        {
            var obstacle = (GameObject)PrefabUtility.InstantiatePrefab(generator.ObstaclePrefab, generator.ObstaclesRoot);
            obstacle.transform.SetPositionAndRotation(pose.position, pose.rotation);
            obstacle.transform.localScale = Vector3.one * generator.ObstacleSize;
            Undo.RegisterCreatedObjectUndo(obstacle, UndoName);
        }

        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log($"Generated {layout.Count} obstacles.");
        LogBalance(generator, need);
    }

    [MenuItem("Tools/Level/Check Balance")]
    private static void CheckBalance()
    {
        LevelGenerator generator = FindGenerator();
        if (generator != null) LogBalance(generator, generator.EstimateRequiredSize());
    }

    private static void LogBalance(LevelGenerator generator, float need)
    {
        float available = generator.AvailableSize;
        float margin = need > 0f ? available / need - 1f : 1f;
        string text = $"Balance: need ~{need:0.00} size, ball has {available:0.00}, extra {margin:P0} (must be {generator.BalanceMargin:P0}).";
        if (generator.IsBalanced(need)) Debug.Log(text);
        else Debug.LogWarning(text + " Not balanced: generate again or change config.");
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
