using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;
    public float spawnInterval = 5f; 
    public float spawnDistanceOutsideView = 2f; 
    public float itemLifetime = 30f; 
    public float minSpawnDistance = 2f; 
    public float checkRadius = 1.5f; 

    private Camera mainCamera;
    private List<Vector3> spawnedPositions = new List<Vector3>(); 

    void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(SpawnItems());
    }

    IEnumerator SpawnItems()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnItemOutsideView();
        }
    }

    void SpawnItemOutsideView()
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            GameObject item = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
            spawnedPositions.Add(spawnPosition);
            Destroy(item, itemLifetime); 
        }
    }

    Vector3 GetValidSpawnPosition()
    {
        int maxAttempts = 20; 
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 newSpawnPos = GetRandomSpawnPosition();
            bool isFarEnough = true;

           
            foreach (Vector3 pos in spawnedPositions)
            {
                if (Vector3.Distance(newSpawnPos, pos) < minSpawnDistance)
                {
                    isFarEnough = false;
                    break;
                }
            }

            
            Collider2D hit = Physics2D.OverlapCircle(newSpawnPos, checkRadius);
            if (isFarEnough && hit == null)
            {
                return newSpawnPos;
            }
        }

        return Vector3.zero; 
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
                                       Random.Range(camPos.y - camHeight, camPos.y + camHeight), 0);
                break;
            case 1: // ขวา
                spawnPos = new Vector3(camPos.x + camWidth + spawnDistanceOutsideView,
                                       Random.Range(camPos.y - camHeight, camPos.y + camHeight), 0);
                break;
            case 2: // บน
                spawnPos = new Vector3(Random.Range(camPos.x - camWidth, camPos.x + camWidth),
                                       camPos.y + camHeight + spawnDistanceOutsideView, 0);
                break;
            case 3: // ล่าง
                spawnPos = new Vector3(Random.Range(camPos.x - camWidth, camPos.x + camWidth),
                                       camPos.y - camHeight - spawnDistanceOutsideView, 0);
                break;
        }

        return spawnPos;
    }
}