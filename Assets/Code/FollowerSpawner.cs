using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerSpawner : MonoBehaviour
{
    public GameObject FollowerPrefab;
    public float spawnInterval = 3f; 
    public float spawnDistanceOutsideView = 2f; 
    public float FollowerLifetime = 40f; 

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(SpawnFollower());
    }

    IEnumerator SpawnFollower()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnFollowerOutsideView();
        }
    }

    void SpawnFollowerOutsideView()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject Follower = Instantiate(FollowerPrefab, spawnPosition, Quaternion.identity);
        Destroy(Follower, FollowerLifetime); 
    }

    Vector3 GetRandomSpawnPosition()
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        int edge = Random.Range(0, 4); 
        Vector3 spawnPos = Vector3.zero;
        Vector3 camPos = mainCamera.transform.position;

        switch (edge)
        {
            case 0: // ซ้าย
                spawnPos = new Vector3(camPos.x - camWidth - spawnDistanceOutsideView,
                                       Random.Range(camPos.y - camHeight, camPos.y + camHeight),
                                       0);
                break;
            case 1: // ขวา
                spawnPos = new Vector3(camPos.x + camWidth + spawnDistanceOutsideView,
                                       Random.Range(camPos.y - camHeight, camPos.y + camHeight),
                                       0);
                break;
            case 2: // บน
                spawnPos = new Vector3(Random.Range(camPos.x - camWidth, camPos.x + camWidth),
                                       camPos.y + camHeight + spawnDistanceOutsideView,
                                       0);
                break;
            case 3: // ล่าง
                spawnPos = new Vector3(Random.Range(camPos.x - camWidth, camPos.x + camWidth),
                                       camPos.y - camHeight - spawnDistanceOutsideView,
                                       0);
                break;
        }

        return spawnPos;
    }
}
