using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Unity.XR.CoreUtils;

public class CameraManage : MonoBehaviour
{
    public GameObject monsterPrefab; // 생성할 몬스터 프리팹
    public float spawnDistance = 2.0f; // 카메라 앞쪽으로 얼마나 떨어진 곳에 생성할지

    private XROrigin xrOrigin;

    void Start()
    {
        // XROrigin 찾기
        xrOrigin = FindObjectOfType<XROrigin>();

        // 몬스터 생성하기
        SpawnMonsterInFrontOfCamera();
    }

    // 카메라 앞쪽에 몬스터 생성
    void SpawnMonsterInFrontOfCamera()
    {
        // 카메라 위치와 방향 가져오기
        Camera arCamera = xrOrigin.Camera;
        Vector3 cameraPosition = arCamera.transform.position;
        Vector3 cameraForward = arCamera.transform.forward;

        // 카메라 앞쪽으로 spawnDistance 만큼 떨어진 위치 계산
        Vector3 spawnPosition = cameraPosition + cameraForward * spawnDistance;

        // 카메라의 회전값과 동일하게 생성
        Quaternion spawnRotation = arCamera.transform.rotation;

        // 몬스터 생성
        Instantiate(monsterPrefab, spawnPosition, spawnRotation);
    }
}
