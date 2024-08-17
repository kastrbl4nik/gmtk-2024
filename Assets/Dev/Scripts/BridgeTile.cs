using UnityEngine;

public class BridgeTile : MonoBehaviour
{
    private Bridge parentBridge;

    private void Awake()
    {
        parentBridge = GetComponentInParent<Bridge>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        parentBridge.CollisionEnter(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        parentBridge.CollisionExit(collision);
    }
}
