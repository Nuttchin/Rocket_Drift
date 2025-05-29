using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float boostedSpeed = 10f;
    public float turnSpeed = 200f;

    private Vector2 targetDirection;
    private bool isBoosting = false;
    private float boostTimer = 0f;
    public float boostDuration = 2f;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    public float boostCooldown = 5f;

    private float lastTapTime = 0f;
    private float doubleTapThreshold = 0.3f;

    [Header("Boost UI")]
    public GameObject boostUI;

    [Header("Particle Effects")]
    public ParticleSystem boostEffectPrefab;
    private ParticleSystem activeBoostEffect;

    public ParticleSystem deathEffect;

    [Header("Thruster (Normal Flying)")]
    public ParticleSystem thrusterEffectPrefab;
    private ParticleSystem activeThrusterEffect;
    public Transform thrusterPoint; 

    void Start()
    {
        targetDirection = Vector2.up;
        if (boostUI != null) boostUI.SetActive(false);

        
        if (thrusterEffectPrefab != null && thrusterPoint != null)
        {
            activeThrusterEffect = Instantiate(thrusterEffectPrefab, thrusterPoint.position, thrusterPoint.rotation, thrusterPoint);
            activeThrusterEffect.Play();
        }
    }

    void Update()
    {
        HandleInput();
        MoveForward();
        HandleBoostTimers();
        HandleBoostUI();
        UpdateBoostEffectRotation();
        UpdateThrusterEffect(); // ไอพ่น
    }

    void HandleInput()
    {
        if (Time.timeScale == 0f || Camera.main == null) return;

        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.position.x < 0 || touch.position.y < 0 || touch.position.x > Screen.width || touch.position.y > Screen.height)
                    return;

                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                targetDirection = (touchPosition - (Vector2)transform.position).normalized;

                if (touch.phase == TouchPhase.Began)
                {
                    if (Time.time - lastTapTime < doubleTapThreshold)
                    {
                        TryActivateBoost();
                    }
                    lastTapTime = Time.time;
                }
            }
        }
        else
        {
            if (Input.mousePosition.x < 0 || Input.mousePosition.y < 0 ||
                Input.mousePosition.x > Screen.width || Input.mousePosition.y > Screen.height)
                return;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetDirection = (mousePosition - (Vector2)transform.position).normalized;

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                TryActivateBoost();
            }
        }
    }

    void MoveForward()
    {
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        float currentSpeed = isBoosting ? boostedSpeed : moveSpeed;
        transform.position += transform.up * currentSpeed * Time.deltaTime;
    }

    void TryActivateBoost()
    {
        if (!isBoosting && !isOnCooldown)
        {
            isBoosting = true;
            boostTimer = boostDuration;
            isOnCooldown = true;
            cooldownTimer = boostCooldown;

            if (boostEffectPrefab != null)
            {
                activeBoostEffect = Instantiate(boostEffectPrefab, transform.position, transform.rotation, transform);
                activeBoostEffect.Play();
            }
        }
    }

    void HandleBoostTimers()
    {
        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                isBoosting = false;

                if (activeBoostEffect != null)
                {
                    activeBoostEffect.Stop();
                    Destroy(activeBoostEffect.gameObject, 1f);
                }
            }
        }

        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
            }
        }
    }

    void HandleBoostUI()
    {
        if (boostUI != null)
        {
            boostUI.SetActive(!isBoosting && !isOnCooldown);
        }
    }

    void UpdateBoostEffectRotation()
    {
        if (activeBoostEffect != null)
        {
            activeBoostEffect.transform.rotation = transform.rotation;
        }
    }

    void UpdateThrusterEffect()
    {
        if (activeThrusterEffect != null)
        {
            
            activeThrusterEffect.transform.rotation = transform.rotation;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Meteor") || collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            Debug.Log("Player Died");

            if (deathEffect != null)
            {
                ParticleSystem effectInstance = Instantiate(deathEffect, transform.position, Quaternion.identity);
                effectInstance.Play();
                Destroy(effectInstance.gameObject, 2f);
            }

            // StopThruster
            if (activeThrusterEffect != null)
            {
                activeThrusterEffect.Stop();
                Destroy(activeThrusterEffect.gameObject, 1f);
            }

            // Stop Boost Effect
            if (activeBoostEffect != null)
            {
                activeBoostEffect.Stop();
                Destroy(activeBoostEffect.gameObject, 1f);
            }

            gameObject.SetActive(false);
            GameManager.instance.OnPlayerDied();
        }
    }
}