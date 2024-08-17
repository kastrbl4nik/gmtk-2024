using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IWeightable
{
    // it is very unlikely that some object will touch more then 20 other objects at the same time
    private const int MaxIncidentObjects = 20;
    float Weight { get; }

    public static void GetIncidentWeightableObjects(Hashtable objectsOnTop, HashSet<GameObject> queueToCheck)
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
            var results = new Collider2D[MaxIncidentObjects];
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
}
