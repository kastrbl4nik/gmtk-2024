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
        altarLight.SetActive(false);
        glow.SetActive(false);
        notGlow.SetActive(true);
    }

    private void Activate()
    {
        // deactivate previous altars
        FindObjectsOfType<Altar>()
            .Where(a => a != this && a.IsActive)
            .FirstOrDefault()
            .Deactivate();

        AudioManager.Instance.Play("flame-ignition");

        isActive = true;
        altarLight.SetActive(true);
        glow.SetActive(true);
        notGlow.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive && other.CompareTag("Player"))
        {
            Activate();
        }
    }
}
