using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CameraManage : MonoBehaviour
{
    public GameObject[] monsterPrefabs; // ���� ���� ���� ������
    private GameObject spawnedMonster; // ���� ������ ���� �ν��Ͻ�
    public float spawnDistance = 2.0f; // ī�޶� �������� �󸶳� ������ ���� ��������

    private XROrigin xrOrigin;
    private ARAnchorManager anchorManager;

    void Start()
    {
        // ���� �õ� �ʱ�ȭ
        Random.InitState(System.DateTime.Now.Millisecond);

        // XROrigin �� ARAnchorManager ã��
        xrOrigin = FindObjectOfType<XROrigin>();
        anchorManager = FindObjectOfType<ARAnchorManager>();

        // ���� �����ϱ�
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


    // ��Ŀ�� ���� ���� ����
    void SpawnMonsterWithAnchor()
    {
        // ī�޶� ��ġ�� ���� ��������
        Camera arCamera = xrOrigin.Camera;
        Vector3 cameraPosition = arCamera.transform.position;
        Vector3 cameraForward = arCamera.transform.forward;

        // ī�޶� �������� spawnDistance ��ŭ ������ ��ġ ���
        Vector3 spawnPosition = cameraPosition + cameraForward * spawnDistance;

        // ī�޶��� ȸ������ Y������ 180�� ȸ�� �߰�
        Quaternion spawnRotation = arCamera.transform.rotation * Quaternion.Euler(0, 180, 0);

        // ��Ŀ ���� �� ��ġ ����
        GameObject anchorObject = new GameObject("Anchor");
        anchorObject.transform.position = spawnPosition;
        anchorObject.transform.rotation = spawnRotation;

        // ARAnchor ������Ʈ�� �߰��Ͽ� ��Ŀ�� ���
        ARAnchor anchor = anchorObject.AddComponent<ARAnchor>();

        if (anchor != null)
        {
            // ������ ���� ������ ����
            int randomIndex = Random.Range(0, monsterPrefabs.Length);
            GameObject selectedPrefab = monsterPrefabs[randomIndex];

            // ���� �ν��Ͻ� ���� �� ����
            spawnedMonster = Instantiate(selectedPrefab, anchor.transform.position, anchor.transform.rotation, anchor.transform);
        }
    }


}
