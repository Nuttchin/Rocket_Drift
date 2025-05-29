using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerSystem : MonoBehaviour
{
    public GameObject followerPrefab;  
    public int maxFollowers = 10;  
    private List<Transform> followers = new List<Transform>();
    private Queue<Vector3> previousPositions = new Queue<Vector3>();

    void Update()
    {
        if (followers.Count > 0)
        {
            previousPositions.Enqueue(transform.position);

            if (previousPositions.Count > followers.Count * 10)  //ระยะห่างระหว่าง Follower
            {
                previousPositions.Dequeue();
            }

            for (int i = 0; i < followers.Count; i++)
            {
                followers[i].position = Vector3.Lerp(followers[i].position, previousPositions.ToArray()[i * 10], Time.deltaTime * 10);
            }
        }
    }

    public void AddFollower()
    {
        if (followers.Count < maxFollowers)
        {
            GameObject newFollower = Instantiate(followerPrefab, transform.position, Quaternion.identity);
            followers.Add(newFollower.transform);
        }
    }
    public int GetFollowerCount()
    {
        return followers.Count;
    }
}
