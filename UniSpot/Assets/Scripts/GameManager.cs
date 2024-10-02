using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // 씬 관련 기능을 사용하기 위해 필요

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //public GameData gameData;

    // 플레이어 정보 및 게임 상태 관리 변수들
    public string playerID;
    public bool isZoneCaptured;
    public float timer; // 타이머는 초 단위로 관리
    public TextMeshProUGUI playerIDTextMesh; // TextMeshPro를 통해 플레이어 ID를 입력 받음
    public TextMeshProUGUI timerTextMesh;    // TextMeshPro를 통해 타이머 UI 표시
    public GameObject occupationOff; // 점령 여부 확인할 Bt_Occupation 내의 Off 요소
    public Text textSpotInfo; // 스팟의 상태
    public TextMeshProUGUI resource; // 자원량 표시를 위한 요소
    public Text OccupationResource; // 스팟 점령에 필요한 자원량
    public GameObject OccupationResourceInfo; // 스팟 점령 가능 여부 정보

    public int resourceValue; // 씬 간에 공유되는 현재 자원량

    private void Awake()
    {
        // 싱글톤 인스턴스 생성
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시에도 파괴되지 않음


        }
        else
        {
            Destroy(gameObject);  // 다른 인스턴스가 있다면 새로 생성하지 않음
        }
    }
    
    private void Start()
    {
        LoadGameData();

        // 플레이어 ID 기본 값 설정 (빈 값일 경우 "User Name")
        playerID = PlayerPrefs.GetString("PlayerID", "User Name");
        playerIDTextMesh.text = playerID;

        // 자원량 초기화
        UpdateResourceUI();

        // 점령 여부 초기화
        isZoneCaptured = PlayerPrefs.GetInt("IsZoneCaptured", 0) == 1;
        occupationOff.SetActive(!isZoneCaptured);  // 점령 여부에 따라 Off 요소의 상태 조정

        // 타이머 초기화 (00:00)
        timer = PlayerPrefs.GetFloat("Timer", 0f);
        UpdateTimerUI();
    }

    // 자원량 UI 업데이트
    public void UpdateResourceUI()
    {
        resource.text = resourceValue.ToString();
    }

    // 씬 이동 함수
    public void LoadSelectScene(string Scenename)
    {
        SaveGameData();
        SceneManager.LoadScene(Scenename);
    }
    
    public void LoadCameraScene()
    {
        SaveGameData();
        SceneManager.LoadScene("Camera Scene");
    }
    public void LoadMainScene()
    {
        SaveGameData();
        SceneManager.LoadScene("Main Scene");
    }

    public void LoadSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // 자원량 업데이트
    public void UpdateResource(int amount)
    {
        resourceValue += amount; 
        UpdateResourceUI();
    }

    // PlayerPrefs에 자원량 저장
    public void SaveGameData()
    {
        PlayerPrefs.SetString("PlayerID", playerID);
        PlayerPrefs.SetInt("IsZoneCaptured", isZoneCaptured ? 1 : 0);
        PlayerPrefs.SetFloat("Timer", timer);
        PlayerPrefs.SetInt("ResourceValue", resourceValue);

        PlayerPrefs.Save();
    }

    // 점령 여부 체크
    public void CheckZoneCaptured()
    {
        // occupationOff가 비활성화되어 있으면 점령되지 않은 상태, 활성화되어 있으면 점령된 상태
        isZoneCaptured = occupationOff.activeSelf;
    }

    public void CaptureZone()
    {
        /*// resource와 occupation resource의 텍스트 컴포넌트에서 값을 파싱
        int currentResourceValue = int.Parse(resource.text); // Assuming the resource text is purely numeric
        int occupationResourceValue = int.Parse(OccupationResource.text); // Assuming the occupation resource text is purely numeric
        */
        int occupationResourceValue = int.Parse(OccupationResource.text);

        // 현재 자원 값이 필요한 occupation resource 값보다 크거나 같은지 확인
        if (resourceValue >= occupationResourceValue)
        {
            // 자원량 감소를 일관성 있게 처리
            UpdateResource(-occupationResourceValue);

            // occupation resource 정보를 비활성화
            OccupationResourceInfo.SetActive(false);

            // 점령 확인을 위한 occupation off GameObject 활성화
            occupationOff.SetActive(true);

            // 점령 상태 변경
            isZoneCaptured = true;

            // 로그
            Debug.Log("Zone has been successfully captured.");
        }
        else
        {
            Debug.Log("Not enough resources to capture the zone.");
        }
    }

    // 타이머를 특정 조건에서 05:00으로 설정 후 시작
    public void StartCooldownTimer()
    {
        timer = 5 * 60; // 5분(초 단위로 설정)
        UpdateTimerUI();
    }

    // 타이머를 기본값(00:00)으로 초기화
    public void ResetTimer()
    {
        timer = 0f;
        UpdateTimerUI();
    }

    // 타이머 UI 업데이트 (MM:SS 형식)
    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerTextMesh.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // 타이머 업데이트
    private void Update()
    {
        if (timerActive && timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerUI();

            if (timer <= 0)
            {
                timerActive = false;
                TimerEnded();
            }
        }
    }

    // 필요 시 데이터를 초기화하는 메서드
    public void ResetGameData()
    {
        isZoneCaptured = false;
        timer = 0f;
    }


    public void LoadGameData()
    {
        // 플레이어 ID 불러오기 (기본값: "User Name")
        playerID = PlayerPrefs.GetString("PlayerID", "User Name");
        playerIDTextMesh.text = playerID;

        // 점령 여부 불러오기 (기본값: 점령되지 않은 상태)
        isZoneCaptured = PlayerPrefs.GetInt("IsZoneCaptured", 0) == 1;
        occupationOff.SetActive(!isZoneCaptured);  // 점령 여부에 따라 Off 요소의 상태 조정

        // 타이머 값 불러오기 (기본값: 0초)
        timer = PlayerPrefs.GetFloat("Timer", 0f);
        UpdateTimerUI();

        // 자원 값 불러오기 (기본값: 1000)
        resourceValue = PlayerPrefs.GetInt("ResourceValue", 1000);
        UpdateResourceUI(); // 자원 UI 업데이트
    }

    // 타이머 기능 구체화
    private bool timerActive = false;

    public void StartTimer(float duration)
    {
        timer = duration;
        timerActive = true;
    }

    private void TimerEnded()
    {
        timerActive = false;
        // 타이머가 끝났을 때 처리할 로직
        Debug.Log("Timer has ended!");
    }
}
