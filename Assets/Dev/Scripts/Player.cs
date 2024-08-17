using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IWeightable, IKeyable
{
    public float Weight { get; set; }
    public GameObject Key { get; set; }

    public static readonly float Lifespan = 10f;
    public Action OnDeath;
    private GameObject keyContainer;

    private void Awake()
    {
        keyContainer = new GameObject("KeyContainer");
        keyContainer.transform.position = transform.position + new Vector3(0f, 1f, 0f);
        keyContainer.transform.SetParent(transform);
    }

    private void Start()
    {
        Weight = 10f;
        StartCoroutine(ScaleOverTime(transform, Vector2.one * 3));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropKey();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Key") && Key == null)
        {

            FindObjectOfType<AudioManager>().Play("keyPickup");
            Key = other.gameObject;
            StartCoroutine(MoveKeyToContainer());
        }
    }

    private IEnumerator MoveKeyToContainer()
    {
        var originalScale = Key.transform.localScale; // Store the original scale
        var targetScale = Vector2.one / 5;
        var duration = 0.5f; // Duration for the animation
        var elapsedTime = 0f;

        // Animate the key shrinking and moving
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = Mathf.Clamp01(elapsedTime / duration);

            // Move the key towards the container
            Key.transform.position = Vector3.Lerp(Key.transform.position, keyContainer.transform.position, t);
            // Shrink the key
            Key.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);

            yield return null; // Wait for the next frame
        }
        Key.transform.position = keyContainer.transform.position;
        Key.transform.localScale = targetScale;
        Key.transform.SetParent(keyContainer.transform);

        // After the animation, set the key inactive
        // Key.gameObject.SetActive(false);
    }

    private void DropKey()
    {
        if (Key == null)
        {
            return;
        }

        // var droppedKey = Instantiate(Key.gameObject, dropPosition, Quaternion.identity);
        var movementDirection = (Vector3)(GetComponent<Rigidbody2D>().velocity * new Vector2(1f, 2f)).normalized;
        var dropPosition = transform.position + (movementDirection * 3f);
        // Key.SetActive(true);
        // Key.transform.position = dropPosition;
        //
        // var rb = Key.GetComponent<Rigidbody2D>();
        // if (rb != null)
        // {
        //     rb.isKinematic = false;
        //     // rb.velocity = GetComponent<Rigidbody2D>().velocity * 1.2f;
        //     rb.AddForce(movementDirection * 5f, ForceMode2D.Impulse);
        // }

        Key = null;
        StartCoroutine(GrowAndFlyAway(Key));
        // Destroy(Key);
    }

    private IEnumerator GrowAndFlyAway(GameObject key)
    {
        var originalScale = key.transform.localScale; // Store the original scale
        var movementDirection = (Vector3)(GetComponent<Rigidbody2D>().velocity * new Vector2(1f, 2f)).normalized;
        var dropPosition = transform.position + (movementDirection * 3f);
        var duration = 0.5f; // Duration for the animation
        var elapsedTime = 0f;

        // Animate the key growing and moving away
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = Mathf.Clamp01(elapsedTime / duration);

            // Move the key away
            key.transform.position = Vector3.Lerp(key.transform.position, dropPosition, t);
            // Grow the key
            key.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);

            yield return null; // Wait for the next frame
        }

        // After the animation, set the key inactive
        // Destroy(key); // Destroy the key after it flies away
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

    private void Die()
    {
        DropKey();
        GetComponent<PlayerController>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        OnDeath?.Invoke();
    }

    private void OnDrawGizmos()
    {
        var movementDirection = (Vector3)(GetComponent<Rigidbody2D>().velocity * new Vector2(1f, 2f)).normalized;
        var dropPosition = transform.position + (movementDirection * 3f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(dropPosition, 1);
    }
}
