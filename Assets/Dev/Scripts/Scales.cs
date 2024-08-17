using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scales : MonoBehaviour
{
    private const float RotationSpeed = 50f;
    private const float MinAngle = -30f;
    private const float MaxAngle = 30f;
    [SerializeField] private ScalesPlatform leftPlatform;
    [SerializeField] private ScalesPlatform rightPlatform;

    private readonly HashSet<GameObject> objectsOnLeftPlatform = new();
    private readonly HashSet<GameObject> objectsOnRightPlatform = new();
    private float currentAngle;

    private void Update()
    {
        var OnLeftPlatform = new Hashtable();
        var OnRightPlatform = new Hashtable();
        IWeightable.GetIncidentWeightableObjects(OnLeftPlatform, new HashSet<GameObject>(objectsOnLeftPlatform));
        IWeightable.GetIncidentWeightableObjects(OnRightPlatform, new HashSet<GameObject>(objectsOnRightPlatform));
        var difference = OnLeftPlatform.Count - OnRightPlatform.Count;
        var rotationAngle = difference * RotationSpeed * Time.deltaTime;
        currentAngle += rotationAngle;

        currentAngle = Mathf.Clamp(currentAngle, MinAngle, MaxAngle);

        transform.localEulerAngles = new Vector3(0f, 0f, currentAngle);
    }

    public void CollisionEnter(Collision2D other, ScalesPlatform platform)
    {
        var otherGameObject = other.gameObject;
        var weightable = otherGameObject.GetComponent<IWeightable>();
        if (weightable != null)
        {
            if (leftPlatform == platform)
            {
                objectsOnLeftPlatform.Add(otherGameObject);
            }
            else if (rightPlatform == platform)
            {
                objectsOnRightPlatform.Add(otherGameObject);
            }
            else
            {
                Debug.LogWarning($"Platform {platform.name} is not supported");
            }
        }
    }

    public void CollisionExit(Collision2D other, ScalesPlatform platform)
    {
        var otherGameObject = other.gameObject;
        if (leftPlatform == platform && objectsOnLeftPlatform.Contains(otherGameObject))
        {
            objectsOnLeftPlatform.Remove(otherGameObject);
        }
        else if (rightPlatform == platform && objectsOnRightPlatform.Contains(otherGameObject))
        {
            objectsOnRightPlatform.Remove(otherGameObject);
        }
        else
        {
            Debug.LogWarning($"Platform {platform.name} is not supported");
        }
    }
}
