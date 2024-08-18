using System;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour, IWeightable, IKeyable
{
    public float Weight { get; private set; }
    public bool IsHoldingKey { get; set; }
    public GameObject Key { get; set; }

    public static readonly float Lifespan = 10f;
    private GameObject lifeIndicator;
    private const float KeyShrinkingScale = 5f;
    public Action OnDeath;
    private GameObject keyContainer;
    public bool IsAlive { get; set; } = true;

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
        StartCoroutine(ShowLifeline());
    }

    private IEnumerator ScaleOverTime(Transform targetTransform, Vector3 targetScale)
    {
        Vector2 initialScale = targetTransform.localScale;
        var scaleDifference = targetScale.x / initialScale.x;
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

    private IEnumerator ShowLifeline()
    {
        var lifeLineWidth = 2f;
        var lifeLineHeight = 0.2f;
        var initialSize = new Vector3(lifeLineWidth * 100, lifeLineHeight * 100, 1);
        var targetSize = new Vector3(0, lifeLineHeight * 100, 1);
        lifeIndicator = CreateRectangle("Life", initialSize, transform.position + new Vector3(0, 1, 0));
        lifeIndicator.transform.SetParent(transform);
        var lifeRenderer = lifeIndicator.GetComponent<Renderer>();

        var elapsedTime = 0f;
        var halfTime = Lifespan / 2;
        var halfSize = initialSize / new Vector2(2, 1);

        while (elapsedTime < halfTime)
        {
            var alpha = Mathf.Clamp01(elapsedTime / halfTime);
            lifeIndicator.transform.localScale = Vector2.Lerp(initialSize, halfSize, alpha);
            lifeRenderer.material.color = Color.Lerp(Color.green, Color.yellow, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < halfTime)
        {
            var alpha = Mathf.Clamp01(elapsedTime / halfTime);
            lifeIndicator.transform.localScale = Vector2.Lerp(halfSize, targetSize, alpha);
            lifeRenderer.material.color = Color.Lerp(Color.yellow, Color.red, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private GameObject CreateRectangle(string rectangleName, Vector3 size, Vector3 position)
    {
        var rectangle = new GameObject(rectangleName);
        var spriteRenderer = rectangle.AddComponent<SpriteRenderer>();
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        var sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = sprite;
        rectangle.transform.localScale = size;
        rectangle.transform.localPosition = position;
        return rectangle;
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
            Key = other.gameObject.transform.parent.gameObject;
            Key.GetComponentInChildren<CapsuleCollider2D>().enabled = false;
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
        var duration = 0.2f;
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
        var originalPosition = Key.transform.position;
        var targetPosition = originalPosition + (Vector3.up * 2);
        var duration = 0.5f;
        var elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = Mathf.Clamp01(elapsedTime / duration);
            Key.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            Key.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, t);
            yield return null;
        }

        Key.transform.localScale = targetScale;
        Key.transform.localPosition = targetPosition;
        Key.GetComponentInChildren<CapsuleCollider2D>().enabled = true;
        Key = null;
    }

    public void UseKey()
    {
        IsHoldingKey = false;
        Destroy(Key);
        Key = null;
    }

    public void Die()
    {

        StopAllCoroutines();
        IsAlive = false;
        if (lifeIndicator != null)
        {
            lifeIndicator.GetComponent<Renderer>().material.color = Color.red;
            Destroy(lifeIndicator, 1f);
        }
        DropKey();
        GetComponent<PlayerController>().enabled = false;
        GetComponent<Rigidbody2D>().drag = 1000;
        GetComponent<Rigidbody2D>().gravityScale = 3000;

        OnDeath?.Invoke();
    }
}
