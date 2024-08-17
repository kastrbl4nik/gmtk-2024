using UnityEngine;

public class ScalesPlatform : MonoBehaviour
{
    private Scales parentScales;

    private void Awake()
    {
        parentScales = GetComponentInParent<Scales>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        parentScales.CollisionEnter(collision, this);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        parentScales.CollisionExit(collision, this);
    }
}
