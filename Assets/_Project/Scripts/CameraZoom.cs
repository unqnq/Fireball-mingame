using UnityEngine;

// The smaller the ball, the closer the camera, so the ball is always easy to see.
// Camera only moves closer, never back. It stops at minDistance
// and never goes so close that the door leaves the screen.
[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
    [SerializeField] private GameConfig config;
    [SerializeField] private PlayerBall player;
    [SerializeField] private Transform door;
    [Tooltip("Camera can't be closer to the ball than this.")]
    [SerializeField] private float minDistance = 14f;
    [Tooltip("How fast the camera follows the zoom (0..1 per second).")]
    [SerializeField] private float zoomSpeed = 1.5f;
    [Tooltip("Door top point, measured from the door pivot.")]
    [SerializeField] private float doorHeight = 5f;
    [Tooltip("Free space around the door on screen (0..1 of the screen).")]
    [SerializeField] private float screenMargin = 0.05f;

    private Camera cam;
    private Vector3 basePosition;
    private float zoom; // 0 = start position, 1 = as close as possible

    private void Start()
    {
        cam = GetComponent<Camera>();
        basePosition = transform.position;
    }

    private void LateUpdate()
    {
        // ball start size -> 0, ball min size -> 1; never go back
        float target = Mathf.InverseLerp(config.startSize, config.minSize, player.Size);
        target = Mathf.Max(target, zoom);

        float next = Mathf.MoveTowards(zoom, target, zoomSpeed * Time.deltaTime);

        // zoom in only while the door is still on the screen
        if (next > zoom && !DoorVisible(PositionFor(next))) next = zoom;

        zoom = next;
        transform.position = PositionFor(zoom);
    }

    // camera moves on the line to the ball, so the ball stays in the same place on the screen
    private Vector3 PositionFor(float t)
    {
        Vector3 toBall = player.transform.position - basePosition;
        float maxMove = Mathf.Max(0f, toBall.magnitude - minDistance);
        return basePosition + toBall.normalized * (maxMove * t);
    }

    private bool DoorVisible(Vector3 cameraPosition)
    {
        Vector3 old = transform.position;
        transform.position = cameraPosition;
        bool visible = OnScreen(door.position) && OnScreen(door.position + Vector3.up * doorHeight);
        transform.position = old;
        return visible;
    }

    private bool OnScreen(Vector3 point)
    {
        Vector3 v = cam.WorldToViewportPoint(point);
        return v.z > 0f
            && v.x > screenMargin && v.x < 1f - screenMargin
            && v.y > screenMargin && v.y < 1f - screenMargin;
    }
}
