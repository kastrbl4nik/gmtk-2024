using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    private const int MaxObjectsOnBridge = 20;
    private const float MaxWeightToHandle = 20;
    private readonly Dictionary<GameObject, int> objectsOnBridge = new();

    private void Update()
    {
        var objectsThatWeight = GetObjectsOnTop();
        var totalWeight = objectsThatWeight.Values.Cast<IWeightable>().Sum(o => o.Weight);
        if (totalWeight > MaxWeightToHandle)
        {
            Break();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        CollisionEnter(other);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        CollisionExit(other);
    }

    public void CollisionEnter(Collision2D other)
    {
        var otherGameObject = other.gameObject;
        var weightable = otherGameObject.GetComponent<IWeightable>();
        if (weightable != null)
        {
            if (!objectsOnBridge.TryAdd(otherGameObject, 1))
            {
                objectsOnBridge[otherGameObject] += 1;
            }
        }
    }

    public void CollisionExit(Collision2D other)
    {
        var otherGameObject = other.gameObject;
        if (objectsOnBridge.ContainsKey(otherGameObject))
        {
            var oldCount = objectsOnBridge[otherGameObject];
            if (oldCount == 1)
            {
                objectsOnBridge.Remove(otherGameObject);
            }
            else
            {
                objectsOnBridge[otherGameObject] -= 1;
            }
        }
    }

    private Hashtable GetObjectsOnTop()
    {
        var queueToCheck = new HashSet<GameObject>();
        foreach (var objectOnBridge in objectsOnBridge.Keys.Cast<GameObject>())
        {
            queueToCheck.Add(objectOnBridge);
        }

        var objectsOnTop = new Hashtable();
        GetIncidentObjects(objectsOnTop, queueToCheck);
        return objectsOnTop;
    }

    private void GetIncidentObjects(Hashtable objectsOnTop, HashSet<GameObject> queueToCheck)
    {
        while (queueToCheck.Count > 0)
        {
            var objectToCheck = queueToCheck.First();
            queueToCheck.Remove(objectToCheck);
            if (objectsOnTop.ContainsKey(objectToCheck))
            {
                continue;
            }

            objectsOnTop.Add(objectToCheck, objectToCheck.GetComponent<IWeightable>());
            var results = new Collider2D[MaxObjectsOnBridge];
            var objectCollider = objectToCheck.GetComponent<CapsuleCollider2D>();
            var size = Physics2D.OverlapCapsuleNonAlloc(objectCollider.bounds.center, objectCollider.size,
                objectCollider.direction, 0f, results);
            for (var i = 0; i < size; i++)
            {
                var incidentObject = results[i];
                var weigtable = incidentObject.GetComponent<IWeightable>();
                if (weigtable != null && !objectsOnTop.ContainsKey(incidentObject.gameObject))
                {
                    queueToCheck.Add(incidentObject.gameObject);
                }
            }
        }
    }

    private void Break()
    {
        Destroy(gameObject, 2f);
    }
}
