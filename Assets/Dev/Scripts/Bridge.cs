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
        IWeightable.GetIncidentWeightableObjects(objectsOnTop, queueToCheck);
        return objectsOnTop;
    }

    private void Break()
    {
        Destroy(gameObject, 2f);
    }
}
