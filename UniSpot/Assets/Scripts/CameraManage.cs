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
    // MonsterGaugeInteraction ��ũ��Ʈ ����
    public MonsterGaugeInteraction monsterGaugeInteraction;

    public GameObject[] monsterPrefabs; // ���� ���� ���� ������
    public GameObject[] monsterPrefabs1; // ���� ������ �迭 1
    public GameObject[] monsterPrefabs2; // ���� ������ �迭 2
    private GameObject spawnedMonster; // ���� ������ ���� �ν��Ͻ�
    public float spawnDistance = 2.0f; // ī�޶� �������� �󸶳� ������ ���� ��������

    private XROrigin xrOrigin;
    private ARAnchorManager anchorManager;

    // �� �迭�� ���� Ȯ�� (0~100 ������ ��)
    [Range(0, 100)]
    public int monsterPrefabs1Chance = 75; // monsterPrefabs1�� ���� Ȯ�� (��: 75%)
    [Range(0, 100)]
    public int monsterPrefabs2Chance = 25; // monsterPrefabs2�� ���� Ȯ�� (��: 25%)

    public TextMeshProUGUI moneyIncreaseText; // ������ų money ���� ��� TextMeshProUGUI ������Ʈ

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

    // ����ġ�� ���� �������� ���� ������ �迭 ���� �� �� �迭���� ���� ������ ����
    GameObject GetRandomMonsterPrefab()
    {
        // Ȯ���� ���� � �迭�� �������� ����
        int randomArrayChoice = Random.Range(0, 100); // 0���� 100 ������ ���� ��

        GameObject selectedPrefab = null;

        if (randomArrayChoice < monsterPrefabs1Chance)
        {
            // monsterPrefabs1�� ������ Ȯ�� (��: 75%)
            selectedPrefab = monsterPrefabs1[Random.Range(0, monsterPrefabs1.Length)];
            Debug.Log("Selected from monsterPrefabs1: " + selectedPrefab.name);

            // moneyIncreaseText ���� 50���� ����
            if (moneyIncreaseText != null)
            {
                moneyIncreaseText.text = "50";
            }

            // fillDecreaseSpeed ���� 0.1�� ����
            if (monsterGaugeInteraction != null)
            {
                monsterGaugeInteraction.fillDecreaseSpeed = 0.1f;
            }
        }
        else
        {
            // monsterPrefabs2�� ������ Ȯ�� (��: 25%)
            selectedPrefab = monsterPrefabs2[Random.Range(0, monsterPrefabs2.Length)];
            Debug.Log("Selected from monsterPrefabs2: " + selectedPrefab.name);

            // moneyIncreaseText ���� 100���� ����
            if (moneyIncreaseText != null)
            {
                moneyIncreaseText.text = "100";
            }

            // fillDecreaseSpeed ���� 0.2�� ����
            if (monsterGaugeInteraction != null)
            {
                monsterGaugeInteraction.fillDecreaseSpeed = 0.2f;
                Debug.Log("�ӵ��� ����");
            }

        }

        return selectedPrefab;
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
            // Ȯ���� ���� �迭 ���� �� ������ ����
            GameObject selectedPrefab = GetRandomMonsterPrefab();

            // ���� �ν��Ͻ� ���� �� ����
            spawnedMonster = Instantiate(selectedPrefab, anchor.transform.position, anchor.transform.rotation, anchor.transform);

            // �������� �̸��� �α׷� ���
            Debug.Log("Spawned Monster Prefab Name: " + selectedPrefab.name);
        }
    }


}
