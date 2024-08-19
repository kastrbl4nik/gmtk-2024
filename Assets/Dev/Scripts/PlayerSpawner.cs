using System.Collections;
using System.Linq;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private float pauseBeforeSpawn = 4.5f;
    [SerializeField] private GameObject spawnEffect;
    private CinemachineVirtualCamera cam;
    private GameObject playerPrefab;
    private Player player;
    private Altar lastAltar;

    private void Awake()
    {
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        cam = FindObjectOfType<CinemachineVirtualCamera>();

        LocateLastAltar();

        cam.Follow = lastAltar.transform;
    }

    private void Start()
    {
        StartCoroutine(SpawnPlayerPrefabRepeatedly());
    }

    private IEnumerator SpawnPlayerPrefabRepeatedly()
    {
        do
        {
            AudioManager.Instance.Play("spawn");
            yield return new WaitForSeconds(pauseBeforeSpawn);
            Spawn();
            yield return new WaitForSeconds(spawnInterval - pauseBeforeSpawn);
        } while (true);
    }

    private void LocateLastAltar()
    {
        var activeAltars = FindObjectsOfType<Altar>().Where(a => a.IsActive).ToArray();
        lastAltar = activeAltars.FirstOrDefault();

        if (activeAltars.Length > 1)
        {
            Debug.LogWarning("Multiple active altars found");
        }

        if (lastAltar == null)
        {
            Debug.LogWarning("No active altar found");
        }
    }

    private void Spawn()
    {
        LocateLastAltar();
        Instantiate(spawnEffect, lastAltar.transform.position, Quaternion.identity);
        player = Instantiate(playerPrefab, lastAltar.transform.position, Quaternion.identity).GetComponent<Player>();
        cam.Follow = player.transform;
    }
}
