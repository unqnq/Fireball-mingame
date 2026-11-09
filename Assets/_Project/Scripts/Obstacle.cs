using System.Collections;
using UnityEngine;

// Obstacle can be infected by a shot: it starts to burn and then explodes.
public class Obstacle : MonoBehaviour
{
    [SerializeField] private Material infectedMaterial;
    [SerializeField] private float burnTime = 0.35f;
    [SerializeField] private Material particleMaterial;

    private bool isInfected;

    public void Infect(float delay)
    {
        if (isInfected) return;
        isInfected = true;

        // shots and path check ignore it from this moment
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;

        StartCoroutine(Burn(delay));
    }

    private IEnumerator Burn(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (Renderer r in GetComponentsInChildren<Renderer>()) r.sharedMaterial = infectedMaterial;
        BurnEffect.Play(transform.position, transform.lossyScale.x, particleMaterial);

        // small swell, then shrink to nothing
        Vector3 startScale = transform.localScale;
        for (float t = 0f; t < 1f; t += Time.deltaTime / burnTime)
        {
            float k = t < 0.3f ? Mathf.Lerp(1f, 1.25f, t / 0.3f) : Mathf.Lerp(1.25f, 0f, (t - 0.3f) / 0.7f);
            transform.localScale = startScale * k;
            yield return null;
        }

        Destroy(gameObject);
    }
}
