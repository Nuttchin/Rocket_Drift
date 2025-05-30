using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
{
    [SerializeField] private SceneController _sceneController;

    public void PlayGame()
    {
        _sceneController.LoadScene("1"); // ใช้ชื่อซีนที่ต้องการโหลด เช่น "1"
    }

    public void Exit()
    {
        Application.Quit();
    }
}


