using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class CameraManage : MonoBehaviour
{
    // MonsterGaugeInteraction 스크립트 참조
    public MonsterGaugeInteraction monsterGaugeInteraction;

    public GameObject[] monsterPrefabs; // 여러 개의 몬스터 프리팹
    public GameObject[] monsterPrefabs1; // 몬스터 프리팹 배열 1
    public GameObject[] monsterPrefabs2; // 몬스터 프리팹 배열 2
    private GameObject spawnedMonster; // 실제 생성된 몬스터 인스턴스
    public float spawnDistance = 2.0f; // 카메라 앞쪽으로 얼마나 떨어진 곳에 생성할지

    private XROrigin xrOrigin;
    private ARAnchorManager anchorManager;

    // 각 배열의 선택 확률 (0~100 사이의 값)
    [Range(0, 100)]
    public int monsterPrefabs1Chance = 75; // monsterPrefabs1의 선택 확률 (예: 75%)
    [Range(0, 100)]
    public int monsterPrefabs2Chance = 25; // monsterPrefabs2의 선택 확률 (예: 25%)

    public TextMeshProUGUI moneyIncreaseText; // 증가시킬 money 값을 담는 TextMeshProUGUI 컴포넌트

    void Start()
    {
        // 랜덤 시드 초기화
        Random.InitState(System.DateTime.Now.Millisecond);

        // XROrigin 및 ARAnchorManager 찾기
        xrOrigin = FindObjectOfType<XROrigin>();
        anchorManager = FindObjectOfType<ARAnchorManager>();

        // 몬스터 생성하기
        SpawnMonsterWithAnchor();

        if (spawnedMonster != null)
        {
            Vector3 monsterScreenPosition = Camera.main.WorldToScreenPoint(spawnedMonster.transform.position);
            Debug.Log(monsterScreenPosition);
        }
        else
        {
            Debug.LogWarning("Monster has not been spawned yet.");
        }
    }

    // 가중치에 따라 랜덤으로 몬스터 프리팹 배열 선택 후 그 배열에서 랜덤 프리팹 선택
    GameObject GetRandomMonsterPrefab()
    {
        // 확률에 따라 어떤 배열을 선택할지 결정
        int randomArrayChoice = Random.Range(0, 100); // 0부터 100 사이의 랜덤 값

        GameObject selectedPrefab = null;

        if (randomArrayChoice < monsterPrefabs1Chance)
        {
            // monsterPrefabs1을 선택할 확률 (예: 75%)
            selectedPrefab = monsterPrefabs1[Random.Range(0, monsterPrefabs1.Length)];
            Debug.Log("Selected from monsterPrefabs1: " + selectedPrefab.name);

            // moneyIncreaseText 값을 50으로 설정
            if (moneyIncreaseText != null)
            {
                moneyIncreaseText.text = "50";
            }

            // fillDecreaseSpeed 값을 0.1로 설정
            if (monsterGaugeInteraction != null)
            {
                monsterGaugeInteraction.fillDecreaseSpeed = 0.1f;
            }
        }
        else
        {
            // monsterPrefabs2를 선택할 확률 (예: 25%)
            selectedPrefab = monsterPrefabs2[Random.Range(0, monsterPrefabs2.Length)];
            Debug.Log("Selected from monsterPrefabs2: " + selectedPrefab.name);

            // moneyIncreaseText 값을 100으로 설정
            if (moneyIncreaseText != null)
            {
                moneyIncreaseText.text = "100";
            }

            // fillDecreaseSpeed 값을 0.2로 설정
            if (monsterGaugeInteraction != null)
            {
                monsterGaugeInteraction.fillDecreaseSpeed = 0.2f;
                Debug.Log("속도가 변경");
            }

        }

        return selectedPrefab;
    }


    // 앵커를 통한 몬스터 생성
    void SpawnMonsterWithAnchor()
    {
        // 카메라 위치와 방향 가져오기
        Camera arCamera = xrOrigin.Camera;
        Vector3 cameraPosition = arCamera.transform.position;
        Vector3 cameraForward = arCamera.transform.forward;

        // 카메라 앞쪽으로 spawnDistance 만큼 떨어진 위치 계산
        Vector3 spawnPosition = cameraPosition + cameraForward * spawnDistance;

        // 카메라의 회전값에 Y축으로 180도 회전 추가
        Quaternion spawnRotation = arCamera.transform.rotation * Quaternion.Euler(0, 180, 0);

        // 앵커 생성 및 위치 고정
        GameObject anchorObject = new GameObject("Anchor");
        anchorObject.transform.position = spawnPosition;
        anchorObject.transform.rotation = spawnRotation;

        // ARAnchor 컴포넌트를 추가하여 앵커로 사용
        ARAnchor anchor = anchorObject.AddComponent<ARAnchor>();

        if (anchor != null)
        {
            // 확률에 따라 배열 선택 및 프리팹 선택
            GameObject selectedPrefab = GetRandomMonsterPrefab();

            // 몬스터 인스턴스 생성 및 저장
            spawnedMonster = Instantiate(selectedPrefab, anchor.transform.position, anchor.transform.rotation, anchor.transform);

            // 프리팹의 이름을 로그로 출력
            Debug.Log("Spawned Monster Prefab Name: " + selectedPrefab.name);
        }
    }


}
