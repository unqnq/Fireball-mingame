using System;
using UnityEngine;

// Keeps the ball size and tells others when it changes.
public class PlayerBall : MonoBehaviour
{
    [SerializeField] private GameConfig config;

    public float Size { get; private set; }
    public event Action<float> SizeChanged;

    private void Awake()
    {
        SetSize(config.startSize);
    }

    public void SetSize(float size)
    {
        Size = Mathf.Max(0f, size);
        transform.localScale = Vector3.one * Size;

        // ball always stays on the ground
        Vector3 position = transform.position;
        position.y = Size * 0.5f;
        transform.position = position;

        SizeChanged?.Invoke(Size);
    }
}
