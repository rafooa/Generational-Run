using System.Collections;
using UnityEngine;

public class timeSlow : MonoBehaviour
{
    public float slowdownFactor = 0.3f; // Adjust as needed
    public float slowdownDuration = 1f; // Adjust as needed
    public float smoothness = 0.1f; // Adjust as needed

    public float start;
    public AudioSource zaWARUDO;

    public AudioSource bgSound;
    public AudioLowPassFilter muffleSound;
    static public bool slowCheck = false;
    private bool soundCheck = false;

    private float originalTimeScale;

    void Start()
    {
        originalTimeScale = Time.timeScale;
        start = Time.time;
    }

    void Update()
    {
        // Check for input to trigger the slowdown effect
        if (Input.GetButtonDown("time") && (Time.time - start > 3f || start == 0)) // You can change KeyCode to any key you want
        {
            slowCheck = true;
            TriggerSlowdown();
        }
    }

    public void TriggerSlowdown()
    {
        StartCoroutine(SmoothTimeScale(slowdownFactor, slowdownDuration));
    }

    IEnumerator SmoothTimeScale(float targetScale, float duration)
    {
        if (!soundCheck)
        {
            StartCoroutine(transitionVolume(1f, 0.1f));

            zaWARUDO.Play();
            yield return new WaitForSeconds(0.75f);
            StartCoroutine(TransitionCutoffFrequency(20000f, 500f));
            soundCheck = true;
        }
        if (targetScale != originalTimeScale)
            slowCheck = true;
        float startTime = Time.time;
        float startScale = Time.timeScale;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float elapsedTime = Time.time - startTime;
            float newTimeScale = Mathf.Lerp(startScale, targetScale, elapsedTime / duration);
            Time.timeScale = newTimeScale;
            yield return null;
        }

        // Wait for a few seconds before resetting the time scale
        yield return new WaitForSeconds(2f); // Adjust as needed
        if (targetScale != originalTimeScale)
            ResetTimeScale();
    }

    IEnumerator transitionVolume(float startVol, float endVol)
    {
        // Start and end frequencies


        // Time elapsed
        float elapsedTime = 0.0f;

        // Smoothly transition the cutoff frequency over time
        while (elapsedTime < 1)
        {
            // Calculate the interpolation factor (0 to 1)
            float t = elapsedTime / 1f;

            // Interpolate the cutoff frequency
            float cutoffVol = Mathf.Lerp(startVol, endVol, t);


            // Set the cutoff frequency of the low-pass filter
            bgSound.volume = cutoffVol;
            // Increment time elapsed
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        bgSound.volume = endVol;

    }

    IEnumerator TransitionCutoffFrequency(float startFrequency, float endFrequency)
    {
        // Start and end frequencies


        // Time elapsed
        float elapsedTime = 0.0f;

        // Smoothly transition the cutoff frequency over time
        while (elapsedTime < 1f)
        {
            // Calculate the interpolation factor (0 to 1)
            float t = elapsedTime / 1f;

            // Interpolate the cutoff frequency
            float cutoffFrequency = Mathf.Lerp(startFrequency, endFrequency, t);


            // Set the cutoff frequency of the low-pass filter
            muffleSound.cutoffFrequency = cutoffFrequency;
            // Increment time elapsed
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        muffleSound.cutoffFrequency = endFrequency;

    }
    public void ResetTimeScale()
    {
        zaWARUDO.Stop();
        StartCoroutine(SmoothTimeScale(originalTimeScale, slowdownDuration));
        StartCoroutine(TransitionCutoffFrequency(500f, 20000f));
        StartCoroutine(transitionVolume(0.05f, 1f));
        soundCheck = false;
        slowCheck = false;
    }
}
