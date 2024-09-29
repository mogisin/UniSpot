using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // 씬 관련 기능을 사용하기 위해 필요


public class SceneManage : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        var eventSystems = FindObjectsOfType<EventSystem>();
        if (eventSystems.Length > 1)
        {
            for (int i = 1; i < eventSystems.Length; i++)
            {
                Destroy(eventSystems[i].gameObject);
            }
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

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
        // 씬이 언로드되기 전에 데이터를 저장
        //SaveManager.Instance.SaveGame();
    }
}
