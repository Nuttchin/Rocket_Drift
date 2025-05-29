using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float baseSpeed = 3f;
    public int maxHP = 10;
    private int currentHP;
    public float maxSpeed = 8f;
    public float accelerationRange = 4f;

    public GameObject followerPrefab;
    public int minFollowersToDrop = 10;
    public int maxFollowersToDrop = 20;

    public float dashInterval = 3f;
    public float dashForce = 5f;
    private float dashTimer;

    public ParticleSystem deathEffect;
    public ParticleSystem damageEffect;   
    public ParticleSystem teleportEffect; 

    public Vector2 teleportAreaMin = new Vector2(-8, -4);
    public Vector2 teleportAreaMax = new Vector2(8, 4);
    public bool allowTeleport = false;

    private Transform player;
    private SpriteRenderer sr;
    private bool hasTeleportedWhenLowHP = false;

    void Start()
    {
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, player.position);
            float speedMultiplier = 1f;

            if (distance <= accelerationRange)
            {
                speedMultiplier = Mathf.Lerp(1f, maxSpeed / baseSpeed, 1 - (distance / accelerationRange));
            }

            transform.position += (Vector3)direction * baseSpeed * speedMultiplier * Time.deltaTime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        dashTimer += Time.deltaTime;

        if (dashTimer >= dashInterval)
        {
            DashBurst();
            dashTimer = 0f;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (damageEffect != null)
        {
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        }

        StartCoroutine(FlashRed());

        
        if (currentHP <= 0)
        {
            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }

            DropFollowers();
            Destroy(gameObject);
        }
        else
        {
            // HP < 50% 
            if (allowTeleport && !hasTeleportedWhenLowHP && currentHP < maxHP * 0.5f)
            {
                TeleportToRandomPosition();
                hasTeleportedWhenLowHP = true;
            }
        }
    }

    IEnumerator FlashRed()
    {
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
        }
    }

    private void DropFollowers()
    {
        if (followerPrefab == null || player == null) return;

        int count = Random.Range(minFollowersToDrop, maxFollowersToDrop + 1);
        List<GameObject> droppedFollowers = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 0.5f;
            GameObject followerGO = Instantiate(followerPrefab, transform.position + (Vector3)offset, Quaternion.identity);
            droppedFollowers.Add(followerGO);
        }

        for (int i = 0; i < droppedFollowers.Count; i++)
        {
            Follower followerScript = droppedFollowers[i].GetComponent<Follower>();
            if (followerScript != null)
            {
                followerScript.enabled = true;
                followerScript.SendMessage("ForceFollow", i == 0 ? player : droppedFollowers[i - 1].transform);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Meteor"))
        {
            TakeDamage(1);
        }
    }

    void DashBurst()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * dashForce;
    }

    void TeleportToRandomPosition()
    {
        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, transform.position, Quaternion.identity);
        }

        float x = Random.Range(teleportAreaMin.x, teleportAreaMax.x);
        float y = Random.Range(teleportAreaMin.y, teleportAreaMax.y);
        transform.position = new Vector3(x, y, transform.position.z);

        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, transform.position, Quaternion.identity);
        }
    }
}