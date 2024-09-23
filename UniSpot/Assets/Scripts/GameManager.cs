using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // 플레이어 정보 및 게임 상태 관리 변수들
    public string playerID;
    public bool isZoneCaptured;
    public float timer; // 타이머는 초 단위로 관리
    public TextMeshProUGUI playerIDTextMesh; // TextMeshPro를 통해 플레이어 ID를 입력 받음
    public TextMeshProUGUI timerTextMesh;    // TextMeshPro를 통해 타이머 UI 표시
    public GameObject occupationOff; // 점령 여부 확인할 Bt_Occupation 내의 Off 요소

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
        // 플레이어 ID 기본 값 설정 (빈 값일 경우 "User Name")
        playerID = playerIDTextMesh.text;
        if (string.IsNullOrEmpty(playerID))
        {
            playerID = "User Name";
            playerIDTextMesh.text = playerID;  // TextMeshPro에 기본 값 반영
        }

        // 점령 여부 초기화
        CheckZoneCaptured();

        // 타이머 초기화 (00:00)
        ResetTimer();
    }

    // 점령 여부 체크
    public void CheckZoneCaptured()
    {
        // occupationOff가 비활성화되어 있으면 점령되지 않은 상태, 활성화되어 있으면 점령된 상태
        isZoneCaptured = occupationOff.activeSelf;
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
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerUI();

            // 타이머가 끝나면 00:00으로 초기화
            if (timer <= 0)
            {
                ResetTimer();
            }
        }

        if (timerActive && timer > 0)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
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

    // 플레이어 데이터 저장 및 불러오기
    public void SaveGameData()
    {
        PlayerPrefs.SetString("PlayerID", playerID);
        PlayerPrefs.SetInt("IsZoneCaptured", isZoneCaptured ? 1 : 0);
        PlayerPrefs.SetFloat("Timer", timer);

        // 플레이어 ID 저장
        PlayerPrefs.SetString("PlayerID", playerIDTextMesh.text);

        PlayerPrefs.Save();
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
