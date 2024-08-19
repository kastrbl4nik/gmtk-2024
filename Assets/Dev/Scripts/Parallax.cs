using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float parallaxEffectMultiplier = 0.5f;

    private Transform mainCameraTransform;
    private Vector3 lastCameraPosition;

    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
        lastCameraPosition = mainCameraTransform.position;
    }

    private void Update()
    {
        var deltaMovement = mainCameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier, deltaMovement.y * parallaxEffectMultiplier, 0f);
        lastCameraPosition = mainCameraTransform.position;
    }
}
