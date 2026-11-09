using UnityEngine;

// Portal "breathes": lights and glow color go up and down.
public class PortalPulse : MonoBehaviour
{
    [SerializeField] private Light[] lights;
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color colorA = new Color(5f, 2f, 10f);
    [SerializeField] private Color colorB = new Color(2f, 3.5f, 9f);
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float lightAmount = 0.35f;

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private float[] baseIntensity;
    private MaterialPropertyBlock block;

    private void Start()
    {
        baseIntensity = new float[lights.Length];
        for (int i = 0; i < lights.Length; i++) baseIntensity[i] = lights[i].intensity;
        block = new MaterialPropertyBlock();
    }

    private void Update()
    {
        float k = (Mathf.Sin(Time.time * speed * Mathf.PI) + 1f) * 0.5f; // 0..1

        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].intensity = baseIntensity[i] * (1f - lightAmount + lightAmount * 2f * k);
        }

        block.SetColor(EmissionColor, Color.Lerp(colorA, colorB, k));
        foreach (Renderer r in renderers) r.SetPropertyBlock(block);
    }
}
