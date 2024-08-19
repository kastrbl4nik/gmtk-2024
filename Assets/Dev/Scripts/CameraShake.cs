using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    public void ShakeyShakey(float duration, float maxAmplitude)
    {
        StartCoroutine(ShakeCoroutine(duration, maxAmplitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float maxAmplitude)
    {
        var noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 0.0f;
        noise.m_FrequencyGain = 1.0f;

        var elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            var t = elapsedTime / duration;
            noise.m_AmplitudeGain = Mathf.Lerp(0.0f, maxAmplitude, t);
            noise.m_FrequencyGain = 1.0f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        noise.m_AmplitudeGain = 0.0f;
        noise.m_FrequencyGain = 1.0f;
    }
}
