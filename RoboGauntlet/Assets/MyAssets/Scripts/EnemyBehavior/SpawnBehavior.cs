using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBehavior : MonoBehaviour
{

    public GameObject spawn;
    public float spawnRadius = 1, time = 2f;
    public GameObject[] enemies;
    public GameObject[] enemyTypes = new GameObject[4]; //manually insert enemies in prefab
    private GameObject player; //only way to recover health is to destroy these

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerControl>().gameObject;
        int i;

        for (i = 0; i < enemies.Length; i++)
        {
            enemies[i] = enemyTypes[(int)Random.Range(0, enemyTypes.Length)];
        }

        StartCoroutine(SpawnAnEnemy());
    }

    IEnumerator SpawnAnEnemy()
    {
        Vector2 spawnPos = spawn.transform.position;
        spawnPos += Random.insideUnitCircle.normalized * spawnRadius;

        Instantiate(enemies[Random.Range(0, enemies.Length)], spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(time);
        StartCoroutine(SpawnAnEnemy());
    }

    private void OnDestroy()
    {
        player.GetComponent<PlayerHealth>().UpdateHealth(1);
    }
}
