using System;
using System.Collections;
using UnityEngine;

// The ball jumps forward by itself while the path in front is free.
// Path check = sphere with ball size along the path centre, so the path gets narrower with the ball.
// At the end the ball waits for the door to open and jumps into the portal.
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private GameConfig config;
    [SerializeField] private PlayerBall ball;
    [SerializeField] private PlayerShooter shooter;
    [SerializeField] private Door door;

    private const float Gap = 0.05f;

    private Vector3 direction;
    private Vector3 endPoint;

    public bool IsJumping { get; private set; }
    public bool ReachedEnd { get; private set; }
    public bool InPortal { get; private set; }

    public event Action EnteredPortal;

    private void Start()
    {
        Vector3 start = Flat(transform.position);
        Vector3 doorPoint = Flat(door.transform.position);
        direction = (doorPoint - start).normalized;
        endPoint = doorPoint - direction * config.doorStopDistance;
    }

    private void Update()
    {
        if (IsJumping || InPortal) return;

        if (ReachedEnd)
        {
            if (door.IsOpen) StartCoroutine(JumpIntoPortal());
            return;
        }

        if (shooter.IsBusy) return;

        float step = Mathf.Min(config.jumpDistance, FreeDistance());
        if (step < config.minJumpDistance && DistanceToEnd() > config.minJumpDistance) return; // path is blocked

        StartCoroutine(Jump(step));
    }

    private float FreeDistance()
    {
        float toEnd = DistanceToEnd();
        float radius = ball.Size * 0.5f;
        if (Physics.SphereCast(transform.position, radius, direction, out RaycastHit hit, toEnd))
        {
            return Mathf.Max(0f, hit.distance - Gap);
        }
        return toEnd;
    }

    private IEnumerator Jump(float distance)
    {
        IsJumping = true;

        Vector3 from = transform.position;
        Vector3 to = from + direction * distance;
        for (float t = 0f; t < 1f; t += Time.deltaTime / config.jumpDuration)
        {
            Vector3 position = Vector3.Lerp(from, to, t);
            position.y = ball.Size * 0.5f + Mathf.Sin(t * Mathf.PI) * config.jumpHeight;
            transform.position = position;
            yield return null;
        }
        to.y = ball.Size * 0.5f;
        transform.position = to;

        IsJumping = false;
        if (DistanceToEnd() < Gap) ReachedEnd = true;
    }

    private IEnumerator JumpIntoPortal()
    {
        IsJumping = true;
        InPortal = true;

        Vector3 from = transform.position;
        Vector3 to = door.PortalPoint;
        to.y = from.y;
        Vector3 startScale = transform.localScale;
        for (float t = 0f; t < 1f; t += Time.deltaTime / config.portalJumpTime)
        {
            Vector3 position = Vector3.Lerp(from, to, t);
            position.y += Mathf.Sin(t * Mathf.PI) * config.jumpHeight;
            transform.position = position;
            // the ball goes into the portal and disappears
            transform.localScale = startScale * (1f - t * t);
            yield return null;
        }

        gameObject.SetActive(false);
        Debug.Log("Win: the ball is in the portal");
        EnteredPortal?.Invoke();
    }

    private float DistanceToEnd()
    {
        return Vector3.Distance(Flat(transform.position), endPoint);
    }

    private static Vector3 Flat(Vector3 v)
    {
        v.y = 0f;
        return v;
    }
}
