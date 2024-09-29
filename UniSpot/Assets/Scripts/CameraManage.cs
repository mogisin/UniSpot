using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

public class CameraManage : MonoBehaviour
{
    public GameObject monsterPrefab; // 생성할 몬스터 프리팹
    public float spawnDistance = 2.0f; // 카메라 앞쪽으로 얼마나 떨어진 곳에 생성할지

    private XROrigin xrOrigin;
    private ARAnchorManager anchorManager;

    void Start()
    {
        // XROrigin 및 ARAnchorManager 찾기
        xrOrigin = FindObjectOfType<XROrigin>();
        anchorManager = FindObjectOfType<ARAnchorManager>();

        // 몬스터 생성하기
        SpawnMonsterWithAnchor();
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

        // 카메라의 회전값과 동일하게 생성
        Quaternion spawnRotation = arCamera.transform.rotation;

        // 앵커 생성 및 위치 고정
        GameObject anchorObject = new GameObject("Anchor");
        anchorObject.transform.position = spawnPosition;
        anchorObject.transform.rotation = spawnRotation;

        // ARAnchor 컴포넌트를 추가하여 앵커로 사용
        ARAnchor anchor = anchorObject.AddComponent<ARAnchor>();

        if (anchor != null)
        {
            Instantiate(monsterPrefab, anchor.transform.position, anchor.transform.rotation, anchor.transform);
        }
    }
}
