using UnityEngine;

// Top bar: how much fire (ball size) is left for shots.
// While charging, the preview part shows how much the current shot takes.
public class FireBar : MonoBehaviour
{
    [SerializeField] private GameConfig config;
    [SerializeField] private PlayerBall ball;
    [SerializeField] private PlayerShooter shooter;
    [SerializeField] private RectTransform fill;
    [SerializeField] private RectTransform preview;

    private void Update()
    {
        float range = config.startSize - config.minSize;
        float value = Mathf.Clamp01((ball.Size - config.minSize) / range);
        float shot = Mathf.Clamp01(shooter.ChargeSize / range);

        SetRange(fill, 0f, value);
        SetRange(preview, value, Mathf.Min(1f, value + shot));
    }

    // bar part from a to b (0..1 of the bar width)
    private static void SetRange(RectTransform rect, float a, float b)
    {
        rect.anchorMin = new Vector2(a, 0f);
        rect.anchorMax = new Vector2(b, 1f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
