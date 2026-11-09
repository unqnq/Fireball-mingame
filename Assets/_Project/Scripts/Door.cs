using System.Collections;
using UnityEngine;

// The door opens when the ball comes closer than config.doorOpenDistance.
public class Door : MonoBehaviour
{
    [SerializeField] private GameConfig config;
    [SerializeField] private Transform player;
    [SerializeField] private Transform leafPivot;
    [SerializeField] private Transform portal;

    private bool isOpening;

    public bool IsOpen { get; private set; }
    public Vector3 PortalPoint => portal.position;

    private void Update()
    {
        if (isOpening) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.magnitude <= config.doorOpenDistance) StartCoroutine(Open());
    }

    private IEnumerator Open()
    {
        isOpening = true;

        // the leaf opens to the player side, so it doesn't go through the portal
        Quaternion closed = leafPivot.localRotation;
        Quaternion opened = closed * Quaternion.Euler(0f, config.doorOpenAngle, 0f);
        for (float t = 0f; t < 1f; t += Time.deltaTime / config.doorOpenTime)
        {
            float k = 1f - (1f - t) * (1f - t); // fast start, soft stop
            leafPivot.localRotation = Quaternion.Slerp(closed, opened, k);
            yield return null;
        }
        leafPivot.localRotation = opened;

        IsOpen = true;
    }
}
