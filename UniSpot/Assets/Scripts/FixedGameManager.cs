using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // 씬 관련 기능을 사용하기 위해 필요

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
