using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float baseSpeed = 2f;  
    public float maxSpeed = 6f;   
    public float detectionRadius = 5f; 
    public float chaseAccelerationFactor = 2f; 
    public float wanderSpeed = 1.5f; 
    public float changeDirectionTime = 2f; 

    private float currentSpeed; 
    private Transform player;
    private Rigidbody2D rb;
    private Vector2 wanderDirection;
    private bool isChasingPlayer = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        currentSpeed = baseSpeed;
        StartCoroutine(ChangeWanderDirection());
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRadius)
            {
                isChasingPlayer = true;
                AdjustSpeed(distanceToPlayer);
                ChasePlayer();
            }
            else
            {
                isChasingPlayer = false;
                WanderAround();
            }
        }
        

    }

    void AdjustSpeed(float distanceToPlayer)
    {
        float distanceFactor = Mathf.Clamp01(1 - (distanceToPlayer / detectionRadius));
        currentSpeed = Mathf.Lerp(baseSpeed, maxSpeed, distanceFactor);
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * currentSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void WanderAround()
    {
        rb.velocity = wanderDirection * wanderSpeed;

        float angle = Mathf.Atan2(wanderDirection.y, wanderDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    IEnumerator ChangeWanderDirection()
    {
        while (true)
        {
            if (!isChasingPlayer)
            {
                wanderDirection = Random.insideUnitCircle.normalized;
            }
            yield return new WaitForSeconds(changeDirectionTime);
        }
    }
}
