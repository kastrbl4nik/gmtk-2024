using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static readonly float Lifespan = 10f;
    private void Start()
    {
        StartCoroutine(ScaleOverTime(transform, Vector2.one * 3, Lifespan));
    }

    private IEnumerator ScaleOverTime(Transform targetTransform, Vector3 targetScale, float duration)
    {
        Vector2 initialScale = targetTransform.localScale;
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            targetTransform.localScale = Vector2.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }

        targetTransform.localScale = targetScale;

        Die();
    }

    private void Die()
    {
        GetComponent<PlayerController>().enabled = false;
    }
}
