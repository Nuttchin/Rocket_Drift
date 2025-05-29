using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f; 
    public float spawnDistanceOutsideView = 2f; 
    public float enemyLifetime = 40f; 
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemyOutsideView();
        }
    }

    void SpawnEnemyOutsideView()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        Destroy(enemy, enemyLifetime); 
    }

    Vector3 GetRandomSpawnPosition()
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        int edge = Random.Range(0, 4); // ���͡�ͺ��Ẻ���� (0 = ����, 1 = ���, 2 = ��, 3 = ��ҧ)
        Vector3 spawnPos = Vector3.zero;
        Vector3 camPos = mainCamera.transform.position;

        switch (edge)
        {
            case 0: // ����
                spawnPos = new Vector3(camPos.x - camWidth - spawnDistanceOutsideView,
                                       Random.Range(camPos.y - camHeight, camPos.y + camHeight),
                                       0);
                break;
            case 1: // ���
                spawnPos = new Vector3(camPos.x + camWidth + spawnDistanceOutsideView,
                                       Random.Range(camPos.y - camHeight, camPos.y + camHeight),
                                       0);
                break;
            case 2: // ��
                spawnPos = new Vector3(Random.Range(camPos.x - camWidth, camPos.x + camWidth),
                                       camPos.y + camHeight + spawnDistanceOutsideView,
                                       0);
                break;
            case 3: // ��ҧ
                spawnPos = new Vector3(Random.Range(camPos.x - camWidth, camPos.x + camWidth),
                                       camPos.y - camHeight - spawnDistanceOutsideView,
                                       0);
                break;
        }

        return spawnPos;
    }
}
