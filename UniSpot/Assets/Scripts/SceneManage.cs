using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    // Start is called before the first frame update
    public void MoveToMain()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void MoveToSpot1()
    {
        SceneManager.LoadScene("CameraScene01");
    }
}
