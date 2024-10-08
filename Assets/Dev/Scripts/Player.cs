using System;
using System.Collections;
using System.Linq;
using UnityEngine;

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
    private PlayerAnimator playerAnimator;
    [SerializeField] private GameObject[] ageStages;
    public bool IsAlive { get; set; } = true;

    private float timeSinceBorn = 0f;

    private void Awake()
    {
        keyContainer = new GameObject("KeyContainer")
        {
            transform = { position = transform.position + new Vector3(0.6f, 0.8f, 0f) }
        };
        keyContainer.transform.SetParent(transform);
        playerAnimator = GetComponentInParent<PlayerAnimator>();
        playerAnimator.SetSprite(ageStages.First());
    }

    private void Start()
    {
        Weight = 10f;
        StartCoroutine(ScaleOverTime(transform, Vector2.one * 3));
    }

    private IEnumerator ScaleOverTime(Transform targetTransform, Vector3 targetScale)
    {
        var timePerStage = Lifespan / ageStages.Length;
        var stageNumber = 0;
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

            if (elapsedTime >= timePerStage * (stageNumber + 1))
            {
                ageStages[stageNumber].SetActive(false);
                stageNumber++;
                ageStages[stageNumber].SetActive(true);
                playerAnimator.SetSprite(ageStages[stageNumber]);
            }

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
        if (IsAlive)
        {

            timeSinceBorn += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsAlive && other.gameObject.CompareTag("Key") && !IsHoldingKey && Key == null)
        {
            Key = other.gameObject.transform.parent.gameObject;
            Key.GetComponentInChildren<CapsuleCollider2D>().enabled = false;
            AudioManager.Instance.Play("keyPickup");
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

        DropKeyForced();
    }

    private void DropKeyForced()
    {
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
        var keyCollider = Key.GetComponentInChildren<CapsuleCollider2D>();
        keyCollider.enabled = true;
        var originalScale = Key.transform.localScale;
        var targetScale = Vector3.one;
        var originalPosition = Key.transform.position;
        var targetPosition = originalPosition + (Vector3.up * 2);
        var duration = 0.5f;
        var elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = Mathf.Clamp01(elapsedTime / duration);
            var collisions = new Collider2D[1];
            var numberOfCollisions = Physics2D.OverlapCollider(keyCollider, new ContactFilter2D(), collisions);
            var newPosition = Vector3.Lerp(originalPosition, targetPosition, t);
            if (numberOfCollisions == 0)
            {
                Key.transform.localPosition = newPosition;
            }
            else
            {
                var positionDifference = newPosition - Key.transform.localPosition;
                targetPosition -= positionDifference;
            }
            Key.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        Key.transform.localScale = targetScale;
        Key.transform.localPosition = targetPosition;
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
        StopCoroutine(nameof(ScaleOverTime));
        StopCoroutine(nameof(MoveKeyToContainer));
        IsAlive = false;
        if (lifeIndicator != null)
        {
            lifeIndicator.GetComponent<Renderer>().material.color = Color.red;
            Destroy(lifeIndicator, 1f);
        }
        DropKey();
        GetComponent<PlayerController>().enabled = false;
        GetComponent<PlayerAnimator>().enabled = false;
        GetComponent<Rigidbody2D>().drag = 1000;
        GetComponent<Rigidbody2D>().gravityScale = 1000;

        OnDeath?.Invoke();
    }

    public float GetLifePercentage()
    {
        return Mathf.Clamp01((Lifespan - timeSinceBorn) / Lifespan);
    }
}
