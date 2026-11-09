using UnityEngine;

// Flies forward and explodes on the first obstacle.
// Explosion infects all obstacles in radius, bigger shot = bigger radius.
public class Projectile : MonoBehaviour
{
    public float Size { get; private set; }

    private float infectionRadius;
    private float spreadSpeed;

    private Vector3 direction;
    private float speed;
    private float distanceLeft;
    private bool isFlying;

    public void SetSize(float size)
    {
        Size = size;
        transform.localScale = Vector3.one * size;
    }

    public void Launch(Vector3 direction, float speed, float maxDistance, float infectionRadius, float spreadSpeed)
    {
        this.infectionRadius = infectionRadius;
        this.spreadSpeed = spreadSpeed;
        this.direction = direction;
        this.speed = speed;
        distanceLeft = maxDistance;
        isFlying = true;
    }

    private void Update()
    {
        if (!isFlying) return;

        float step = speed * Time.deltaTime;
        if (Physics.SphereCast(transform.position, Size * 0.5f, direction, out RaycastHit hit, step))
        {
            transform.position += direction * hit.distance;
            Explode();
            return;
        }

        transform.position += direction * step;
        distanceLeft -= step;
        if (distanceLeft <= 0f) Destroy(gameObject);
    }

    private void Explode()
    {
        Vector3 center = transform.position;
        foreach (Collider hit in Physics.OverlapSphere(center, infectionRadius))
        {
            Obstacle obstacle = hit.GetComponentInParent<Obstacle>();
            if (obstacle == null) continue;

            // closer obstacles burn first, so infection looks like a wave
            float delay = Vector3.Distance(center, hit.transform.position) / spreadSpeed;
            obstacle.Infect(delay);
        }
        Destroy(gameObject);
    }
}
