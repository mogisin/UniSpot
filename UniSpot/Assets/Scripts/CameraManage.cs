using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

public class CameraManage : MonoBehaviour
{
    public GameObject monsterPrefab; // ������ ���� ������
    public float spawnDistance = 2.0f; // ī�޶� �������� �󸶳� ������ ���� ��������

    private XROrigin xrOrigin;
    private ARAnchorManager anchorManager;

    void Start()
    {
        // XROrigin �� ARAnchorManager ã��
        xrOrigin = FindObjectOfType<XROrigin>();
        anchorManager = FindObjectOfType<ARAnchorManager>();

        // ���� �����ϱ�
        SpawnMonsterWithAnchor();
    }

    // ��Ŀ�� ���� ���� ����
    void SpawnMonsterWithAnchor()
    {
        // ī�޶� ��ġ�� ���� ��������
        Camera arCamera = xrOrigin.Camera;
        Vector3 cameraPosition = arCamera.transform.position;
        Vector3 cameraForward = arCamera.transform.forward;

        // ī�޶� �������� spawnDistance ��ŭ ������ ��ġ ���
        Vector3 spawnPosition = cameraPosition + cameraForward * spawnDistance;

        // ī�޶��� ȸ������ �����ϰ� ����
        Quaternion spawnRotation = arCamera.transform.rotation;

        // ��Ŀ ���� �� ��ġ ����
        GameObject anchorObject = new GameObject("Anchor");
        anchorObject.transform.position = spawnPosition;
        anchorObject.transform.rotation = spawnRotation;

        // ARAnchor ������Ʈ�� �߰��Ͽ� ��Ŀ�� ���
        ARAnchor anchor = anchorObject.AddComponent<ARAnchor>();

        if (anchor != null)
        {
            Instantiate(monsterPrefab, anchor.transform.position, anchor.transform.rotation, anchor.transform);
        }
    }
}
