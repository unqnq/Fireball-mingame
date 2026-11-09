using System.Collections.Generic;
using UnityEngine;

// Makes obstacle positions on the path between the player and the door:
// groups (good for big shots) and single ones (need small shots).
// Obstacles can go a bit outside the path (config.pathOverflow).
// Obstacles are created in the editor: Tools > Level > Generate Obstacles.
public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameConfig config;
    [SerializeField] private Transform player;
    [SerializeField] private Transform door;
    [SerializeField] private Transform obstaclesRoot;
    [Tooltip("New obstacles will not be placed on top of these.")]
    [SerializeField] private Transform staticObstaclesRoot;

    public Transform ObstaclesRoot => obstaclesRoot;
    public GameObject ObstaclePrefab => config.obstaclePrefab;
    public float ObstacleSize => config.obstacleSize;

    private System.Random random;
    private Vector3 start;
    private Vector3 forward;
    private Vector3 right;
    private float maxSide;

    public List<Pose> CreateLayout(int seed)
    {
        random = new System.Random(seed);
        var poses = new List<Pose>();
        // existing obstacles: position + diameter
        var blocked = new List<Vector4>();
        if (staticObstaclesRoot != null)
        {
            foreach (Transform child in staticObstaclesRoot)
            {
                Vector3 p = child.position;
                blocked.Add(new Vector4(p.x, p.y, p.z, child.lossyScale.x));
            }
        }

        start = Flat(player.position);
        Vector3 end = Flat(door.position);
        forward = (end - start).normalized;
        right = Vector3.Cross(Vector3.up, forward);

        float from = config.startOffset;
        float to = Vector3.Distance(start, end) - config.endOffset;
        float pathWidth = config.startSize * config.pathWidthMultiplier;
        float pathHalf = pathWidth * 0.5f;
        maxSide = pathHalf + pathWidth * config.pathOverflow;

        // groups: centre is on the path
        for (int i = 0; i < config.clusterCount; i++)
        {
            float along = Range(from, to);
            float side = Range(-pathHalf, pathHalf);
            int count = random.Next(config.obstaclesPerCluster.x, config.obstaclesPerCluster.y + 1);

            for (int j = 0; j < count; j++)
            {
                float angle = Range(0f, Mathf.PI * 2f);
                float radius = Range(0f, config.clusterRadius);
                TryAdd(poses, blocked, along + Mathf.Cos(angle) * radius, side + Mathf.Sin(angle) * radius);
            }
        }

        // single obstacles: anywhere on the path
        for (int i = 0; i < config.singleCount; i++)
        {
            TryAdd(poses, blocked, Range(from, to), Range(-maxSide, maxSide));
        }

        return poses;
    }

    private void TryAdd(List<Pose> poses, List<Vector4> blocked, float along, float side)
    {
        if (Mathf.Abs(side) > maxSide) return;

        Vector3 position = start + forward * along + right * side;
        position.y = config.obstacleSize * 0.5f;

        // new obstacles do not touch each other
        float size = config.obstacleSize;
        foreach (Pose other in poses)
        {
            if (TooClose(other.position, position, size * 1.05f)) return;
        }
        // static obstacles can overlap a bit, but centre can't be inside another obstacle
        foreach (Vector4 other in blocked)
        {
            if (TooClose(other, position, Mathf.Max(size, other.w) * 0.5f)) return;
        }

        poses.Add(new Pose(position, Quaternion.Euler(0f, Range(0f, 360f), 0f)));
    }

    private static bool TooClose(Vector3 a, Vector3 b, float distance)
    {
        a.y = b.y = 0f;
        return (a - b).sqrMagnitude < distance * distance;
    }

    private float Range(float min, float max)
    {
        return min + (float)random.NextDouble() * (max - min);
    }

    private static Vector3 Flat(Vector3 v)
    {
        v.y = 0f;
        return v;
    }
}
