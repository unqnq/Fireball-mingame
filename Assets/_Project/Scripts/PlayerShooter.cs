using UnityEngine;
using UnityEngine.InputSystem;

// Hold tap: the shot grows and the ball gets smaller by the same amount.
// Release tap: the shot flies to the target.
public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private GameConfig config;
    [SerializeField] private PlayerBall player;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform target;

    private Projectile charging;
    private Projectile flying;

    public bool IsCharging => charging != null;

    private void Update()
    {
        Pointer pointer = Pointer.current;
        if (pointer == null) return;

        if (charging == null)
        {
            // one shot at a time
            if (flying == null && pointer.press.wasPressedThisFrame) StartCharge();
            return;
        }

        if (pointer.press.isPressed) Charge(Time.deltaTime);
        else Release();
    }

    private void StartCharge()
    {
        charging = Instantiate(projectilePrefab);
        charging.SetSize(0f);
        UpdateChargePosition();
    }

    private void Charge(float deltaTime)
    {
        // the longer you hold, the bigger the shot
        float amount = Mathf.Min(config.chargeSpeed * deltaTime, player.Size);
        player.SetSize(player.Size - amount);
        charging.SetSize(charging.Size + amount);
        UpdateChargePosition();
    }

    private void Release()
    {
        Vector3 direction = DirectionToTarget();
        float distance = Vector3.Distance(Flat(player.transform.position), Flat(target.position));
        float radius = charging.Size * config.infectionRadius;
        charging.Launch(direction, config.projectileSpeed, distance, radius, config.infectionSpreadSpeed);

        flying = charging;
        charging = null;
    }

    // the shot sits on the ground in front of the ball
    private void UpdateChargePosition()
    {
        float offset = (player.Size + charging.Size) * 0.5f;
        Vector3 position = Flat(player.transform.position) + DirectionToTarget() * offset;
        position.y = charging.Size * 0.5f;
        charging.transform.position = position;
    }

    private Vector3 DirectionToTarget()
    {
        return (Flat(target.position) - Flat(player.transform.position)).normalized;
    }

    private static Vector3 Flat(Vector3 v)
    {
        v.y = 0f;
        return v;
    }
}
