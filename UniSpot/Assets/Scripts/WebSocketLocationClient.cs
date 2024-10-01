using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;


public class WebSocketLocationClient : MonoBehaviour
{
    private ClientWebSocket webSocket;

    [Serializable]
    public class LocationMessage
    {
        public string type = "location";  // 고정된 메시지 타입
        public string username;
        public float latitude;
        public float longitude;
    }

    // 서버 연결 및 위치 메시지 전송
    private async void Start()
    {
        webSocket = new ClientWebSocket();
        Uri serverUri = new Uri("ws://172.20.10.7:8000");  // 서버의 IP 주소와 포트 설정

        try
        {
            // WebSocket 서버에 연결
            await webSocket.ConnectAsync(serverUri, CancellationToken.None);
            Debug.Log("Connected to WebSocket server");

            // 위치 메시지 전송 (테스트용으로 고정된 위치 값)
            await SendLocationMessage("player1", 37.7749f, -122.4194f);
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket connection failed: {e.Message}");
        }
    }

    // 위치 메시지 생성 및 서버로 전송
    private async System.Threading.Tasks.Task SendLocationMessage(string username, float latitude, float longitude)
    {
        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            Debug.LogError("WebSocket is not connected");
            return;
        }

        // 위치 메시지 생성
        LocationMessage locationMessage = new LocationMessage
        {
            username = username,
            latitude = latitude,
            longitude = longitude
        };

        // JSON으로 변환
        string jsonMessage = JsonUtility.ToJson(locationMessage);
        ArraySegment<byte> messageBytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage));

        // 서버로 메시지 전송
        await webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        Debug.Log($"Sent location message: {jsonMessage}");
    }

    // WebSocket 연결 종료
    private async void OnDestroy()
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by Unity", CancellationToken.None);
            Debug.Log("WebSocket connection closed");
        }
    }

}
