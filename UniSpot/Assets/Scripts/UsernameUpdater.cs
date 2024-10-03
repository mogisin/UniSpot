using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UsernameUpdater : MonoBehaviour
{
    public TMP_InputField userNameInput; // Username 입력 필드
    private WebSocketClient webSocketClient; // WebSocketClient 인스턴스 참조

    void Start()
    {
        // WebSocketClient 찾기
        webSocketClient = FindObjectOfType<WebSocketClient>();

        if (userNameInput != null)
        {
            // InputField 값이 변경될 때마다 메서드 호출
            userNameInput.onValueChanged.AddListener(OnUserNameChanged);
        }
    }

    // 유저 이름이 변경될 때 호출되는 메서드
    private void OnUserNameChanged(string newUsername)
    {
        Debug.Log("Username changed: " + newUsername);

        // 서버로 새 유저 이름 전송
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
