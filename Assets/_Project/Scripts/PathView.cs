using UnityEngine;

// Path width follows the ball size.
public class PathView : MonoBehaviour
{
    [SerializeField] private GameConfig config;
    [SerializeField] private PlayerBall player;

    private void OnEnable()
    {
        player.SizeChanged += UpdateWidth;
    }

    private void OnDisable()
    {
        player.SizeChanged -= UpdateWidth;
    }

    private void Start()
    {
        UpdateWidth(player.Size);
    }

    private void UpdateWidth(float ballSize)
    {
        Vector3 scale = transform.localScale;
        scale.x = ballSize * config.pathWidthMultiplier;
        transform.localScale = scale;
    }
}
