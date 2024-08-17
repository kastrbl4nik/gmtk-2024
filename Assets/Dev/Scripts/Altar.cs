using UnityEngine;
using System.Linq;

public class Altar : MonoBehaviour
{
    [SerializeField] private float pauseBeforeSpawn = 1f;
    [SerializeField] private bool isActive;
    public bool IsActive => isActive;
    private GameObject altarLight;

    private void Awake()
    {
        altarLight = transform.GetChild(0).gameObject;
        altarLight.SetActive(isActive);
    }

    public void Deactivate()
    {
        isActive = false;
        altarLight.SetActive(false);
    }

    private void Activate()
    {
        // deactivate previous altars
        FindObjectsOfType<Altar>()
            .Where(a => a != this && a.IsActive)
            .FirstOrDefault()
            .Deactivate();

        FindObjectOfType<AudioManager>().Play("flame-ignition");

        isActive = true;
        altarLight.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive && other.CompareTag("Player"))
        {
            Activate();
        }
    }
}
