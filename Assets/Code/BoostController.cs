using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostController : MonoBehaviour
{
    public float normalSpeed = 5f;
    public float boostSpeed = 10f;
    public float boostDuration = 1.5f;
    public GameObject boostUI;

    private float currentSpeed;
    private float boostTimer;
    private Vector2 targetDirection;
    private float lastTapTime;
    private float tapThreshold = 0.3f;
    private bool isBoosting;

    void Start()
    {
        currentSpeed = normalSpeed;
        targetDirection = Vector2.up;
        boostUI.SetActive(false);
    }

    void Update()
    {
        HandleInput();
        Move();
        HandleBoost();
    }

    void HandleInput()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    if (Time.time - lastTapTime < tapThreshold)
                    {
                        StartBoost();
                    }
                    lastTapTime = Time.time;
                }
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                targetDirection = (touchPosition - (Vector2)transform.position).normalized;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                StartBoost();
            }
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetDirection = (mousePosition - (Vector2)transform.position).normalized;
        }
    }

    void Move()
    {
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), 200f * Time.deltaTime);
        transform.position += transform.up * currentSpeed * Time.deltaTime;
    }

    void StartBoost()
    {
        if (!isBoosting)
        {
            isBoosting = true;
            currentSpeed = boostSpeed;
            boostTimer = boostDuration;
            boostUI.SetActive(true);
        }
    }

    void HandleBoost()
    {
        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                currentSpeed = normalSpeed;
                isBoosting = false;
                boostUI.SetActive(false);
            }
        }
    }
}
