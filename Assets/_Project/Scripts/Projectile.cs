using UnityEngine;

// Flies forward and explodes on the first obstacle in the ball corridor
// (the same corridor the ball checks before jumping), so any obstacle that blocks the ball can be hit.
// The hit obstacle always burns, others burn if they are in infection radius from the hit point.
public class Projectile : MonoBehaviour
{
    public float Size { get; private set; }

    private float hitRadius;
    private float castBack; // first check starts from the ball centre, not from the shot
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

    public void Launch(Vector3 direction, float speed, float maxDistance, float hitRadius, float castBack, float infectionRadius, float spreadSpeed)
    {
        this.hitRadius = hitRadius;
        this.castBack = castBack;
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

        // corridor check at the ball height
        Vector3 origin = transform.position - direction * castBack;
        origin.y = hitRadius;
        if (Physics.SphereCast(origin, hitRadius, direction, out RaycastHit hit, step + castBack))
        {
            Vector3 position = origin + direction * hit.distance;
            position.y = transform.position.y;
            transform.position = position;
            Explode(hit);
            return;
        }
        castBack = 0f;

        transform.position += direction * step;
        distanceLeft -= step;
        if (distanceLeft <= 0f) Destroy(gameObject);
    }

    private void Explode(RaycastHit target)
    {
        Obstacle hitObstacle = target.collider.GetComponentInParent<Obstacle>();
        if (hitObstacle != null) hitObstacle.Infect(0f);

        Vector3 center = target.point;
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
