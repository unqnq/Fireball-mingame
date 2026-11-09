using System.Collections;
using TMPro;
using UnityEngine;

// "Hold to charge, release to shoot" - fades out after the first tap.
public class StartHint : MonoBehaviour
{
    [SerializeField] private PlayerShooter shooter;
    [SerializeField] private TMP_Text text;
    [SerializeField] private float fadeTime = 0.4f;

    private bool hidden;

    private void Update()
    {
        if (hidden) return;

        // soft blinking while waiting
        Color c = text.color;
        c.a = 0.65f + Mathf.Sin(Time.time * 3f) * 0.35f;
        text.color = c;

        if (shooter.IsCharging)
        {
            hidden = true;
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        Color c = text.color;
        float start = c.a;
        for (float t = 0f; t < 1f; t += Time.deltaTime / fadeTime)
        {
            c.a = Mathf.Lerp(start, 0f, t);
            text.color = c;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
