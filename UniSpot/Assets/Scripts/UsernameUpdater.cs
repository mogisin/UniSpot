using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UsernameUpdater : MonoBehaviour
{
    public TMP_InputField userNameInput; // Username �Է� �ʵ�
    private WebSocketClient webSocketClient; // WebSocketClient �ν��Ͻ� ����

    void Start()
    {
        // WebSocketClient ã��
        webSocketClient = FindObjectOfType<WebSocketClient>();

        if (userNameInput != null)
        {
            // InputField ���� ����� ������ �޼��� ȣ��
            userNameInput.onValueChanged.AddListener(OnUserNameChanged);
        }
    }

    // ���� �̸��� ����� �� ȣ��Ǵ� �޼���
    private void OnUserNameChanged(string newUsername)
    {
        Debug.Log("Username changed: " + newUsername);

        // ������ �� ���� �̸� ����
        if (webSocketClient != null)
        {
            webSocketClient.SendUsernameUpdateMessage(newUsername);
        }
        else
        {
            Debug.LogWarning("WebSocketClient reference is not set.");
        }
    }
}
