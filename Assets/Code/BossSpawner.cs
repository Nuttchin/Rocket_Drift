using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPrefab;
    public float spawnInterval = 40f;
    public int maxBossCount = 2;
    public float spawnDistanceOutsideView = 3f;

    private Camera mainCamera;
    private List<GameObject> activeBosses = new List<GameObject>();

    void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(SpawnBossRoutine());
    }

    IEnumerator SpawnBossRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

           
            activeBosses.RemoveAll(boss => boss == null);

            if (activeBosses.Count < maxBossCount)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition();
                GameObject boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
                activeBosses.Add(boss);
            }
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        int edge = Random.Range(0, 4);
        Vector3 camPos = mainCamera.transform.position;

        switch (edge)
        {
            case 0:
                return new Vector3(camPos.x - camWidth - spawnDistanceOutsideView, Random.Range(camPos.y - camHeight, camPos.y + camHeight), 0);
            case 1:
                return new Vector3(camPos.x + camWidth + spawnDistanceOutsideView, Random.Range(camPos.y - camHeight, camPos.y + camHeight), 0);
            case 2:
                return new Vector3(Random.Range(camPos.x - camWidth, camPos.x + camWidth), camPos.y + camHeight + spawnDistanceOutsideView, 0);
            default:
                return new Vector3(Random.Range(camPos.x - camWidth, camPos.x + camWidth), camPos.y - camHeight - spawnDistanceOutsideView, 0);
        }
    }
}