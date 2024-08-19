using UnityEngine;
using System.Linq;

public class Altar : MonoBehaviour
{
    [SerializeField] private float pauseBeforeSpawn = 1f;
    [SerializeField] private bool isActive;
    [SerializeField] private GameObject glow;
    [SerializeField] private GameObject notGlow;
    [SerializeField] private GameObject altarLight;
    public bool IsActive => isActive;

    private void Awake()
    {
        altarLight.SetActive(isActive);
        glow.SetActive(isActive);
        notGlow.SetActive(!isActive);
    }

    public void Deactivate()
    {
        isActive = false;
        altarLight.SetActive(!isActive);
        glow.SetActive(!isActive);
        notGlow.SetActive(isActive);
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
        altarLight.SetActive(isActive);
        glow.SetActive(isActive);
        notGlow.SetActive(!isActive);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive && other.CompareTag("Player"))
        {
            Activate();
        }
    }
}
