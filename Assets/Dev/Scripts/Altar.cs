using System.Collections;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] private GameObject entityToSpawn;

    private void Start()
    {
        Instantiate(entityToSpawn, transform.position, Quaternion.identity);
        StartCoroutine(SpawnEntityRoutine());
    }

    private IEnumerator SpawnEntityRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Player.Lifespan);
            Instantiate(entityToSpawn, transform.position, Quaternion.identity);
        }
    }
}
