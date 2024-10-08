using System.Collections;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private float pauseBeforeSpawn = 4.0f;
    [SerializeField] private GameObject spawnEffect;
    private CinemachineVirtualCamera cam;
    private GameObject playerPrefab;
    private Player player;
    private Altar lastAltar;
    [SerializeField] private int maxPlayersPerLevel;
    private int spawnedSPlayers;

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
            GetComponent<CameraShake>().ShakeyShakey(pauseBeforeSpawn, 1f);
            yield return new WaitForSeconds(pauseBeforeSpawn);
            Spawn();
            spawnedSPlayers++;
            yield return new WaitForSeconds(spawnInterval - pauseBeforeSpawn);
        } while (spawnedSPlayers < maxPlayersPerLevel);
        StartCoroutine(WaitForPlayerDeathAndRestart());
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

    private IEnumerator WaitForPlayerDeathAndRestart()
    {
        while (player.IsAlive)
        {
            yield return new WaitForSeconds(0.5f);
        }

        FindObjectOfType<GameManager>().ReloadScene();
    }

    public int GetMaxPlayersPerLevel()
    {
        return maxPlayersPerLevel;
    }
}
