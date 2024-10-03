using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMonsterData : MonoBehaviour
{
    private WebSocketClient webSocketClient;

    void OnEnable()
    {
        // �̺�Ʈ ����
        MonsterGaugeInteraction.OnCaptureSuccess += OnCaptureSuccess;
    }

    void OnDisable()
    {
        // �̺�Ʈ ���� ����
        MonsterGaugeInteraction.OnCaptureSuccess -= OnCaptureSuccess;
    }

    void Start()
    {
        // WebSocketClient �ν��Ͻ� ã��
        webSocketClient = FindObjectOfType<WebSocketClient>();

        if (webSocketClient == null)
        {
            Debug.LogWarning("WebSocketClient�� ã�� �� �����ϴ�.");
        }
    }

    // ��ȹ ���� �� ȣ��Ǵ� �޼���
    private void OnCaptureSuccess()
    {
        string monsterName = gameObject.name; // ���� ���� ������Ʈ�� �̸�
        SendMonsterNameToServer(monsterName);
    }

    // ������ �̸��� ������ �����ϴ� �޼���
    private void SendMonsterNameToServer(string monsterName)
    {
        if (webSocketClient != null)
        {
            webSocketClient.SendMonsterNameMessage(monsterName);
            Debug.Log("Server�� ���� �̸��� ����: " + monsterName);
        }
        else
        {
            Debug.LogWarning("WebSocketClient reference is not set.");
        }
    }
}
