using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    public GameObject starPrefab;
    public int starCount = 100;
    public float spawnBuffer = 5f;
    public float despawnRadius = 40f;
    public Transform player;

    [Header("Star Size")]
    public float minScale = 0.2f;
    public float maxScale = 1.0f;

    [Header("Parallax")]
    public float followSpeed = 1f; 

    private List<GameObject> stars = new List<GameObject>();
    private Vector3 followOffset;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("StarManager: ต้องใส่ Transform ของ Player ด้วยครับ!");
            return;
        }

        followOffset = transform.position - player.position;

        for (int i = 0; i < starCount; i++)
        {
            Vector2 pos = GetRandomPositionOutsideCamera();
            GameObject star = Instantiate(starPrefab, pos, Quaternion.identity, transform);
            float scale = Random.Range(minScale, maxScale);
            star.transform.localScale = new Vector3(scale, scale, 1f);
            stars.Add(star);
        }
    }

    void Update()
    {
        Vector3 targetPos = player.position + followOffset;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        foreach (GameObject star in stars)
        {
            float distance = Vector2.Distance(player.position, star.transform.position);
            if (distance > despawnRadius)
            {
                Vector2 newPos = GetRandomPositionOutsideCamera();
                star.transform.position = newPos;

                float scale = Random.Range(minScale, maxScale);
                star.transform.localScale = new Vector3(scale, scale, 1f);
            }
        }
    }

    Vector2 GetRandomPositionOutsideCamera()
    {
        Camera cam = Camera.main;
        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        float width = max.x - min.x;
        float height = max.y - min.y;

        Vector2 spawnPos = Vector2.zero;

        int side = Random.Range(0, 4); 

        switch (side)
        {
            case 0: 
                spawnPos = new Vector2(min.x - spawnBuffer, Random.Range(min.y - spawnBuffer, max.y + spawnBuffer));
                break;
            case 1: 
                spawnPos = new Vector2(max.x + spawnBuffer, Random.Range(min.y - spawnBuffer, max.y + spawnBuffer));
                break;
            case 2: 
                spawnPos = new Vector2(Random.Range(min.x - spawnBuffer, max.x + spawnBuffer), max.y + spawnBuffer);
                break;
            case 3: 
                spawnPos = new Vector2(Random.Range(min.x - spawnBuffer, max.x + spawnBuffer), min.y - spawnBuffer);
                break;
        }

        return spawnPos;
    }
}