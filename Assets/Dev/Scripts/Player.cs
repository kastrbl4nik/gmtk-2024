using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IWeightable, IKeyable
{
    public float Weight { get; private set; }
    public bool IsHoldingKey { get; set; }
    public GameObject Key { get; set; }

    public static readonly float Lifespan = 10f;
    private const float KeyShrinkingScale = 5f;
    public Action OnDeath;
    private GameObject keyContainer;

    private void Awake()
    {
        keyContainer = new GameObject("KeyContainer")
        {
            transform = { position = transform.position + new Vector3(0.3f, 0.6f, 0f) }
        };
        keyContainer.transform.SetParent(transform);
    }

    private void Start()
    {
        Weight = 10f;
        StartCoroutine(ScaleOverTime(transform, Vector2.one * 3));
    }

    private IEnumerator ScaleOverTime(Transform targetTransform, Vector3 targetScale)
    {
        Vector2 initialScale = targetTransform.localScale;
        var scaleDifference = targetScale.x  / initialScale.x;
        var targetWeight = Weight * scaleDifference;
        var initialWeight = Weight;
        var elapsedTime = 0f;

        while (elapsedTime < Lifespan)
        {
            var alpha = Mathf.Clamp01(elapsedTime / Lifespan);
            targetTransform.localScale = Vector2.Lerp(initialScale, targetScale, alpha);
            Weight = Mathf.Lerp(initialWeight, targetWeight, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetTransform.localScale = targetScale;

        Die();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropKey();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Key") && !IsHoldingKey && Key == null)
        {
            Key = other.gameObject;
            FindObjectOfType<AudioManager>().Play("keyPickup");
            StartCoroutine(MoveKeyToContainer());
        }
    }

    private IEnumerator MoveKeyToContainer()
    {
        if (Key == null)
        {
            yield break;
        }
        var originalScale = Key.transform.localScale;
        var targetScale = originalScale / KeyShrinkingScale;
        var duration = 0.5f;
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = Mathf.Clamp01(elapsedTime / duration);
            Key.transform.position = Vector3.Lerp(Key.transform.position, keyContainer.transform.position, t);
            Key.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        Key.transform.position = keyContainer.transform.position;
        Key.transform.localScale = targetScale;
        Key.transform.SetParent(keyContainer.transform);
        IsHoldingKey = true;
    }

    private void DropKey()
    {
        if (!IsHoldingKey || Key == null)
        {
            return;
        }
        IsHoldingKey = false;
        Key.transform.SetParent(null);
        StartCoroutine(GrowAndFlyAway());
    }

    private IEnumerator GrowAndFlyAway()
    {
        if (Key == null)
        {
            yield break;
        }
        var originalScale = Key.transform.localScale;
        var targetScale = originalScale * KeyShrinkingScale;
        var duration = 0.5f;
        var elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = Mathf.Clamp01(elapsedTime / duration);
            Key.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        Key.transform.localScale = targetScale;
        Key = null;
    }

    private void Die()
    {
        DropKey();
        GetComponent<PlayerController>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        OnDeath?.Invoke();
    }
}
