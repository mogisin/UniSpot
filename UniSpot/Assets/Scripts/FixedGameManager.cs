using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro ����� ���� ���ӽ����̽�
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // �� ���� ����� ����ϱ� ���� �ʿ�

public class FixedGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSelectScene(string Scenename)
    {
        SceneManager.LoadScene(Scenename);
    }
}
