using UnityEngine;

// All tunable numbers of the game in one place.
[CreateAssetMenu(menuName = "Game/Game Config", fileName = "GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Player")]
    [Tooltip("Start diameter of the player ball.")]
    public float startSize = 1.5f;
    [Tooltip("If the ball gets smaller than this - game over.")]
    public float minSize = 0.3f;

    [Header("Path")]
    [Tooltip("Path width = ball size * this value.")]
    public float pathWidthMultiplier = 1.3f;

    [Header("Obstacles")]
    public GameObject obstaclePrefab;
    public float obstacleSize = 0.9f;
    [Tooltip("How far obstacles can go outside the path, in path widths (0.5 = half of the path width).")]
    public float pathOverflow = 0.5f;
    [Tooltip("Free space after the player start.")]
    public float startOffset = 3f;
    [Tooltip("Free space before the door.")]
    public float endOffset = 3f;
    public int clusterCount = 4;
    public Vector2Int obstaclesPerCluster = new Vector2Int(4, 7);
    public float clusterRadius = 1.4f;
    public int singleCount = 6;
}
