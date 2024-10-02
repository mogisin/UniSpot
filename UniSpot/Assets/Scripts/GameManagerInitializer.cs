using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManagerInitializer : MonoBehaviour
{
    // `GameManager` �������� �����Ϳ��� �Ҵ��ؾ� �մϴ�.
    public GameManager gameManagerPrefab;

    void Awake()
    {
        // `GameManager` �ν��Ͻ��� �̹� �����ϴ��� Ȯ��
        if (GameManager.Instance == null)
        {
            // `GameManager` ������ �ν��Ͻ� ���� �� `DontDestroyOnLoad`�� ����
            Instantiate(gameManagerPrefab);
        }

        SceneManager.LoadScene("Main Scene");
    }
}
