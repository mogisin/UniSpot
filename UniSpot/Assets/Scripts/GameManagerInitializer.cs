using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManagerInitializer : MonoBehaviour
{
    // `GameManager` 프리팹을 에디터에서 할당해야 합니다.
    public GameManager gameManagerPrefab;

    void Awake()
    {
        // `GameManager` 인스턴스가 이미 존재하는지 확인
        if (GameManager.Instance == null)
        {
            // `GameManager` 프리팹 인스턴스 생성 및 `DontDestroyOnLoad`로 설정
            Instantiate(gameManagerPrefab);
        }

        SceneManager.LoadScene("Main Scene");
    }
}
