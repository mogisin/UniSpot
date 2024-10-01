using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // �� ���� ����� ����ϱ� ���� �ʿ�


public class SceneManage : MonoBehaviour
{
    

    public void LoadCameraScene(string sceneName)
    {
        SceneManager.LoadScene("Camera Scene");
    }
    public void LoadMainScene(string sceneName)
    {
        SceneManager.LoadScene("Main Scene");
    }

    public void LoadSampleScene(string sceneName)
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Update is called once per frame
    private void OnSceneUnloaded(Scene scene)
    {
        // ���� ��ε�Ǳ� ���� �����͸� ����
        //SaveManager.Instance.SaveGame();
    }
}
