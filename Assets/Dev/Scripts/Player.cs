using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IWeightable
{
    public float Weight { get; set; }

    public static readonly float Lifespan = 10f;
    public Action OnDeath;
    public bool IsAlive { get; set; } = true;

    private void Start()
    {
        Weight = 10f;
        StartCoroutine(ScaleOverTime(transform, Vector2.one * 3));
    }

    private IEnumerator ScaleOverTime(Transform targetTransform, Vector3 targetScale)
    {
        Vector2 initialScale = targetTransform.localScale;
        var elapsedTime = 0f;

        while (elapsedTime < Lifespan)
        {
            targetTransform.localScale = Vector2.Lerp(initialScale, targetScale, Mathf.Clamp01(elapsedTime / Lifespan));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetTransform.localScale = targetScale;

        Die();
    }

    public void Die()
    {
        IsAlive = false;
        GetComponent<PlayerController>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        OnDeath?.Invoke();
    }
}
