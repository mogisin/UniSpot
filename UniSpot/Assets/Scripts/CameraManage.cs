using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Unity.XR.CoreUtils;

public class CameraManage : MonoBehaviour
{
    public GameObject monsterPrefab; // ������ ���� ������
    public float spawnDistance = 2.0f; // ī�޶� �������� �󸶳� ������ ���� ��������

    private XROrigin xrOrigin;

    void Start()
    {
        // XROrigin ã��
        xrOrigin = FindObjectOfType<XROrigin>();

        // ���� �����ϱ�
        SpawnMonsterInFrontOfCamera();
    }

    // ī�޶� ���ʿ� ���� ����
    void SpawnMonsterInFrontOfCamera()
    {
        // ī�޶� ��ġ�� ���� ��������
        Camera arCamera = xrOrigin.Camera;
        Vector3 cameraPosition = arCamera.transform.position;
        Vector3 cameraForward = arCamera.transform.forward;

        // ī�޶� �������� spawnDistance ��ŭ ������ ��ġ ���
        Vector3 spawnPosition = cameraPosition + cameraForward * spawnDistance;

        // ī�޶��� ȸ������ �����ϰ� ����
        Quaternion spawnRotation = arCamera.transform.rotation;

        // ���� ����
        Instantiate(monsterPrefab, spawnPosition, spawnRotation);
    }
}
