using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopgame : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Meteor") || collision.CompareTag("Enemy"))
        {
            Debug.Log("Game Over!"); 
            Time.timeScale = 0; 
        }
    }
}
