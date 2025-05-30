using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float followSpeed = 3f;
    public float wanderSpeed = 1.5f;
    public float changeDirectionTime = 2f;
    public float smoothFollowSpeed = 5f;
    public float delayPosition = 0.2f;
    public ParticleSystem hitEffect;

    [Header("Sound Effects")]
    public AudioClip collectSound;
    private AudioSource audioSource;

    private bool isFollowing = false;
    private Transform target;
    private Queue<Vector3> positionHistory = new Queue<Vector3>();
    private static List<Follower> allFollowers = new List<Follower>();

    private Vector3 currentVelocity = Vector3.zero;

    private Vector2 wanderDirection;
    private float wanderTimer = 0f;

    void Start()
    {
        PickNewWanderDirection();

        // Audio source setup
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (isFollowing && target != null)
        {
            FollowTarget();
        }
        else
        {
            Wander();
        }
    }

    void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f)
        {
            PickNewWanderDirection();
        }

        Vector3 wanderTarget = transform.position + (Vector3)wanderDirection;
        transform.position = Vector3.SmoothDamp(transform.position, wanderTarget, ref currentVelocity, 1f / smoothFollowSpeed, wanderSpeed);

        if (currentVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * smoothFollowSpeed);
        }
    }

    void PickNewWanderDirection()
    {
        wanderDirection = Random.insideUnitCircle.normalized;
        wanderTimer = changeDirectionTime;
    }

    void FollowTarget()
    {
        positionHistory.Enqueue(target.position);

        int historyLimit = Mathf.RoundToInt(delayPosition / Time.deltaTime);
        if (positionHistory.Count > historyLimit)
        {
            Vector3 targetPosition = positionHistory.Dequeue();

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 1f / smoothFollowSpeed, followSpeed);

            Vector2 direction = (targetPosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * smoothFollowSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Player") || collision.CompareTag("Follower")) && !isFollowing)
        {
            isFollowing = true;
            ScoreManager.Instance?.AddScore(1);

            if (collision.CompareTag("Player"))
                target = collision.transform;
            else if (collision.CompareTag("Follower"))
                target = allFollowers[allFollowers.Count - 1].transform;

            allFollowers.Add(this);

            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            if (collectSound != null && audioSource != null)
                audioSource.PlayOneShot(collectSound);
        }
    }

    public void ForceFollow(Transform followTarget)
    {
        if (!isFollowing)
        {
            isFollowing = true;
            target = followTarget;

            allFollowers.Add(this);

            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            if (collectSound != null && audioSource != null)
                audioSource.PlayOneShot(collectSound);
        }
    }
}