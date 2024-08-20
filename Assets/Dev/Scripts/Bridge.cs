using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Bridge : MonoBehaviour
{
    private const float MaxWeightToHandle = 20;
    private readonly Dictionary<GameObject, int> objectsOnBridge = new();
    private GameObject bridgeTilePrefab;
    public Vector2 startPosition;
    public Vector2 endPosition;
    public List<GameObject> bridgeTiles;
    [Range(0.2f, 1f)] public float tileLength = 0.5f;
    [Range(0.1f, 0.5f)] public float tileHeight = 0.2f;
    [Range(0f, 0.1f)] public float betweenTilesGap = 0.05f;

    private void Awake()
    {
        bridgeTilePrefab = Resources.Load<GameObject>("Prefabs/Rope Bridge Tile");
        var tiles = GetBridgeTiles();
        var lastInChain = new GameObject("Bridge Start") { transform = { position = startPosition } };
        var lastInChainRigidBody = lastInChain.AddComponent<Rigidbody2D>();
        lastInChainRigidBody.bodyType = RigidbodyType2D.Static;
        lastInChain.AddComponent<CircleCollider2D>().radius = 0.02f;
        lastInChain.transform.SetParent(transform);
        foreach (var tilePositionRotation in tiles)
        {
            var nextTile = Instantiate(bridgeTilePrefab, tilePositionRotation.Key, tilePositionRotation.Value, transform);
            bridgeTiles.Add(nextTile);
            var tileHingeJoint = nextTile.GetComponent<HingeJoint2D>();
            tileHingeJoint.connectedBody = lastInChainRigidBody;
            lastInChainRigidBody = nextTile.GetComponent<Rigidbody2D>();
        }

        var endOfBridge = new GameObject("Bridge End");
        endOfBridge.transform.SetParent(transform);
        endOfBridge.transform.position = endPosition;
        endOfBridge.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        var hingeJoint2D = endOfBridge.AddComponent<HingeJoint2D>();
        hingeJoint2D.connectedBody = lastInChainRigidBody;
        hingeJoint2D.enableCollision = true;
        endOfBridge.AddComponent<CircleCollider2D>().radius = 0.02f;
    }

    private List<KeyValuePair<Vector3, Quaternion>> GetBridgeTiles()
    {
        var tiles = new List<KeyValuePair<Vector3, Quaternion>>();
        var bridgeLength = Vector3.Distance(endPosition, startPosition);
        var bridgeDirection = (endPosition - startPosition).normalized;
        var totalTileLength = tileLength + betweenTilesGap;
        var tilesAmount = Mathf.FloorToInt(bridgeLength / totalTileLength);
        var tileDirection = new Vector2(-bridgeDirection.y, bridgeDirection.x);
        for (var i = 0; i < tilesAmount; i++)
        {
            var tilePosition = startPosition + (bridgeDirection * (i * totalTileLength));
            tilePosition.x += totalTileLength / 2;
            tilePosition.y += tileHeight / 2;
            var rotation = Quaternion.LookRotation(Vector3.forward, tileDirection);
            tiles.Add(new KeyValuePair<Vector3, Quaternion>(tilePosition, rotation));
        }

        return tiles;
    }

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
            AudioManager.Instance.Play("bridgeClick" + Random.Range(1, 4));
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
        var middleTile = bridgeTiles[bridgeTiles.Count / 2];
        Destroy(middleTile);
    }

    private void OnDrawGizmos()
    {
        var tiles = GetBridgeTiles();
        foreach (var tile in tiles)
        {
            Gizmos.matrix = Matrix4x4.TRS(tile.Key, tile.Value, Vector3.one);
            Gizmos.DrawCube(Vector3.zero, new Vector3(tileLength, tileHeight, 0.1f));
            Gizmos.matrix = Matrix4x4.identity;
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPosition, 0.2f);
        Gizmos.DrawWireSphere(endPosition, 0.2f);
    }
}
