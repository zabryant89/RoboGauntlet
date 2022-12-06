using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCycle : MonoBehaviour
{
    //all required game objects to have available to spawn:
    public GameObject bossSpawn; //boss character
    public GameObject spawnerSpawn; //spawners

    //time variables (locked in, private)
    private bool spawnerComplete; //spawners stop spawning now
    public float spawnerIntervalTime; //interval between spawners spawning
    public float bossTimer; //timer until boss arrives

    //spawn area variables
    /*  some notes:
     *  center of playable area (x, y): (-8.9, -5)     big goof, i know
     *  no spawner/boss radius: 1 (2 diameter)
     *      ^ range (in coordinates): (-6.9, -3)U(-10.9, -7)
     *  spawn quadrants:
     *      upper left: (-16, -1)U(-8.9, -5)
     *      upper right: (-1, -1)U(-8.9, -5)
     *      lower left: (-16, -9)U(-8.9, -5)
     *      lower right: (-1, -9)U(-8.9, -5)
     */
    private float leftEdge = -8;
    private float rightEdge = 8;
    private float topEdge = 4;
    private float bottomEdge = -4;
    private float noSpawnRadius = 1; //diameter = 2 = 2*noSpawnRadius    from center
    private Transform player; //need player location to prevent unintentional collision damage

    // Start is called before the first frame update
    void Start()
    {
        //player location
        player = FindObjectOfType<PlayerControl>().transform;

        //set spawn boolean to not complete
        spawnerComplete = false;

        //initiate spawner interval time - note: each quadrant has a different start time, but same interval
        StartCoroutine(InitialSpawnerIntervals());

        //start boss timer
        StartCoroutine(BossTimer());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator InitialSpawnerIntervals()
    {
        //SpawnerSpawns(centerX, centerY, leftEdge, topEdge);
        int i;
        for (i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(4.5f);
            SpawnerSpawns();
        }
    }

    IEnumerator SpawnerIntervals()
    {
        yield return new WaitForSeconds(Random.Range(spawnerIntervalTime - 1, spawnerIntervalTime + 1));
        SpawnerSpawns();
    }

    public void SpawnerSpawns(/*float cenX, float cenY, float edgeX, float edgeY*/) //need coords for specific quadrant
    {
        if (spawnerComplete != true)
        {
            Vector3 spawnLocation = new Vector3(Random.Range(leftEdge, rightEdge), Random.Range(bottomEdge, topEdge), 0);

            //exceptions to spawn area, may change it to player... actually I will change it to player!
            while (spawnLocation.x >= player.position.x - 1 && spawnLocation.x <= player.position.x + 1)
            {
                spawnLocation.x = Random.Range(leftEdge, rightEdge);
            }

            while (spawnLocation.y >= player.position.y - 1 && spawnLocation.y <= player.position.y + 1)
            {
                spawnLocation.y = Random.Range(bottomEdge, topEdge);
            }

            Instantiate(spawnerSpawn, spawnLocation, Quaternion.identity);
            StartCoroutine(SpawnerIntervals());
        }
        else
        {

        }
    }

    IEnumerator SpawnerStop()
    {
        yield return new WaitForSeconds(spawnerIntervalTime * 3);
        spawnerComplete = true;
    }

    IEnumerator BossTimer()
    {
        yield return new WaitForSeconds(bossTimer);
        BossSpawn();
    }

    public void BossSpawn()
    {
        Vector3 spawnLocation = new Vector3(Random.Range(leftEdge + 2, rightEdge - 2), Random.Range(bottomEdge + 1, topEdge - 1), 0);

        //exceptions to spawn area, may change it to player... actually I will change it to player!
        while (spawnLocation.x >= player.position.x - 1 && spawnLocation.x <= player.position.x + 1)
        {
            spawnLocation.x = Random.Range(leftEdge + 2, rightEdge - 2);
        }

        while (spawnLocation.y >= player.position.y - 1 && spawnLocation.y <= player.position.y + 1)
        {
            spawnLocation.y = Random.Range(bottomEdge + 1, topEdge - 1);
        }

        Instantiate(bossSpawn, spawnLocation, Quaternion.identity);

        StartCoroutine(SpawnerStop());
    }
}
