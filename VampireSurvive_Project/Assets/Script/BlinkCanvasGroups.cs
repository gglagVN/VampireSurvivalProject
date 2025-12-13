using UnityEngine;

public class BlinkCanvasGroup : MonoBehaviour
{
    [Tooltip("CanvasGroup to fade")]
    public CanvasGroup cg;

    [Tooltip("Speed of blinking (cycles per second)")]
    public float speed = 1f;

    [Tooltip("Minimum alpha (0 = invisible)")]
    [Range(0f,1f)] public float minAlpha = 0f;

    [Tooltip("Maximum alpha (1 = fully visible)")]
    [Range(0f,1f)] public float maxAlpha = 1f;

    [Tooltip("Optional delay before starting (seconds)")]
    public float startDelay = 0f;

    [Tooltip("Use smoother interpolation")]
    public bool smooth = true;

    float startTime;

    void Start()
    {
        if (cg == null) cg = GetComponent<CanvasGroup>();
        startTime = Time.time + startDelay;
    }

    void Update()
    {
        if (cg == null) return;

        float t = (Time.time - startTime) * speed; // cycles per second
        if (t < 0f) return; // still in delay

        // PingPong goes 0..1..0
        float raw = Mathf.PingPong(t, 1f);

        // Option A: Smooth transition (ease)
        float interp = smooth ? Mathf.SmoothStep(0f, 1f, raw) : raw;

        // Map to minAlpha..maxAlpha
        cg.alpha = Mathf.Lerp(minAlpha, maxAlpha, interp);
    }

    // Helper: public method to stop blinking & set alpha (useful on click)
    public void StopAndShow(bool show)
    {
        if (cg == null) return;
        cg.alpha = show ? maxAlpha : minAlpha;
        enabled = false; // disables Update => stop blinking
    }
}
