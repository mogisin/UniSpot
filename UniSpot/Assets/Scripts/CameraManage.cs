using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CameraManage : MonoBehaviour
{
    public GameObject[] monsterPrefabs; // 여러 개의 몬스터 프리팹
    private GameObject spawnedMonster; // 실제 생성된 몬스터 인스턴스
    public float spawnDistance = 2.0f; // 카메라 앞쪽으로 얼마나 떨어진 곳에 생성할지

    private XROrigin xrOrigin;
    private ARAnchorManager anchorManager;

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
            // 랜덤한 몬스터 프리팹 선택
            int randomIndex = Random.Range(0, monsterPrefabs.Length);
            GameObject selectedPrefab = monsterPrefabs[randomIndex];

            // 몬스터 인스턴스 생성 및 저장
            spawnedMonster = Instantiate(selectedPrefab, anchor.transform.position, anchor.transform.rotation, anchor.transform);
        }
    }


}
