using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEditor;
using System.Linq;
using UnityEngine.Rendering.Universal;

public class Altar : MonoBehaviour
{
    [SerializeField] private float pauseBeforeSpawn = 1f;
    [SerializeField] private bool isActive;
    private GameObject playerPrefab;
    private CinemachineVirtualCamera cam;
    private Coroutine spawnCoroutine;
    private GameObject altarLight;

    private void Awake()
    {
        altarLight = GetComponentInChildren<Light2D>().gameObject;
        playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Level/Prefabs/Player.prefab");
        cam = FindObjectOfType<CinemachineVirtualCamera>();
        altarLight.SetActive(isActive);
    }

    private void Start()
    {
        if (isActive)
        {
            spawnCoroutine = StartCoroutine(SpawnCoroutine());
        }
    }

    public void Activate()
    {
        DeactivateOtherAltars();

        isActive = true;
        altarLight.SetActive(true);

        // kill the player who activated the altar
        var playerToDie = FindObjectsOfType<Player>().FirstOrDefault(p => p.IsAlive);
        if (playerToDie != null)
        {
            // destroy instead of die to avoid cramming the altar with dead players
            Destroy(playerToDie.gameObject);
        }

        spawnCoroutine = StartCoroutine(SpawnCoroutine());
    }

    public void Deactivate()
    {
        isActive = false;
        altarLight.SetActive(false);

        StopCoroutine(spawnCoroutine);
    }

    private void DeactivateOtherAltars()
    {
        var altars = FindObjectsOfType<Altar>();

        foreach (var altar in altars)
        {
            if (altar != this)
            {
                altar.Deactivate();
            }
        }
    }

    private IEnumerator SpawnCoroutine()
    {
        Spawn();
        while (true)
        {
            yield return new WaitForSeconds(Player.Lifespan + pauseBeforeSpawn);
            Spawn();
        }
    }

    private void Spawn()
    {
        var instanciatedEntity = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        instanciatedEntity.GetComponent<Player>().OnDeath += () => cam.Follow = transform;
        cam.Follow = instanciatedEntity.transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive && other.CompareTag("Player"))
        {
            Activate();
        }
    }
}
