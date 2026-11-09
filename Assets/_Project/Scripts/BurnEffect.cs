using UnityEngine;

// Fire sparks + smoke when an obstacle burns. Made from code, destroys itself.
public static class BurnEffect
{
    public static void Play(Vector3 position, float size, Material material)
    {
        Create("Sparks", position, size, material,
            count: 25, lifetime: 0.6f, speed: 4f * size, particleSize: 0.15f * size, gravity: 0.5f,
            from: new Color(1f, 0.85f, 0.3f), to: new Color(1f, 0.25f, 0.05f, 0f));

        Create("Smoke", position + Vector3.up * size * 0.3f, size, material,
            count: 10, lifetime: 1.4f, speed: 0.8f * size, particleSize: 0.7f * size, gravity: -0.15f,
            from: new Color(0.25f, 0.22f, 0.25f, 0.7f), to: new Color(0.1f, 0.1f, 0.12f, 0f));
    }

    private static void Create(string name, Vector3 position, float size, Material material,
        int count, float lifetime, float speed, float particleSize, float gravity, Color from, Color to)
    {
        var obj = new GameObject(name);
        obj.transform.position = position;
        var ps = obj.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration = 0.2f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(lifetime * 0.6f, lifetime);
        main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.4f, speed);
        main.startSize = new ParticleSystem.MinMaxCurve(particleSize * 0.5f, particleSize);
        main.gravityModifier = gravity;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction = ParticleSystemStopAction.Destroy;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = size * 0.4f;

        var color = ps.colorOverLifetime;
        color.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(from, 0f), new GradientColorKey(to, 1f) },
            new[] { new GradientAlphaKey(from.a, 0f), new GradientAlphaKey(to.a, 1f) });
        color.color = gradient;

        ps.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;
        ps.Play();
    }
}
