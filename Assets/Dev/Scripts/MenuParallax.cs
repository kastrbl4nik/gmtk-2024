using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    private Vector2 startPos;
    [SerializeField] private float moveModifier;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        Vector2 pz = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        var posX = Mathf.Lerp(transform.position.x, startPos.x + (pz.x * moveModifier), 2f * Time.deltaTime);
        var posY = Mathf.Lerp(transform.position.y, startPos.y + (pz.y * moveModifier), 2f * Time.deltaTime);

        transform.position = new Vector3(posX, posY, transform.position.z);
    }
}
