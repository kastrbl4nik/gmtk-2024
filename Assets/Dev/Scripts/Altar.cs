using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEditor;

public class Altar : MonoBehaviour
{
    [SerializeField] private float pauseBeforeSpawn = 1f;
    private GameObject playerPrefab;
    private CinemachineVirtualCamera cam;

    private void Awake()
    {
        playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Level/Prefabs/Player.prefab");
        cam = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        Spawn();
        StartCoroutine(SpawnEntityRoutine());
    }

    private IEnumerator SpawnEntityRoutine()
    {
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
}
