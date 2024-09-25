using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // �� ���� ����� ����ϱ� ���� �ʿ�


public class SceneManage : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Start is called before the first frame update
    public void LoadCameraScene(string sceneName)
    {
        SceneManager.LoadScene("Camera Scene");
    }
    public void LoadMainScene(string sceneName)
    {
        SceneManager.LoadScene("Main Scene");
    }

    // Update is called once per frame
    private void OnSceneUnloaded(Scene scene)
    {
        // ���� ��ε�Ǳ� ���� �����͸� ����
        //SaveManager.Instance.SaveGame();
    }
}
