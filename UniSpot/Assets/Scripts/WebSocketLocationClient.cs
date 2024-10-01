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
        public string type = "location";  // ������ �޽��� Ÿ��
        public string username;
        public float latitude;
        public float longitude;
    }

    // ���� ���� �� ��ġ �޽��� ����
    private async void Start()
    {
        webSocket = new ClientWebSocket();
        Uri serverUri = new Uri("ws://172.20.10.7:8000");  // ������ IP �ּҿ� ��Ʈ ����

        try
        {
            // WebSocket ������ ����
            await webSocket.ConnectAsync(serverUri, CancellationToken.None);
            Debug.Log("Connected to WebSocket server");

            // ��ġ �޽��� ���� (�׽�Ʈ������ ������ ��ġ ��)
            await SendLocationMessage("player1", 37.7749f, -122.4194f);
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket connection failed: {e.Message}");
        }
    }

    // ��ġ �޽��� ���� �� ������ ����
    private async System.Threading.Tasks.Task SendLocationMessage(string username, float latitude, float longitude)
    {
        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            Debug.LogError("WebSocket is not connected");
            return;
        }

        // ��ġ �޽��� ����
        LocationMessage locationMessage = new LocationMessage
        {
            username = username,
            latitude = latitude,
            longitude = longitude
        };

        // JSON���� ��ȯ
        string jsonMessage = JsonUtility.ToJson(locationMessage);
        ArraySegment<byte> messageBytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage));

        // ������ �޽��� ����
        await webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        Debug.Log($"Sent location message: {jsonMessage}");
    }

    // WebSocket ���� ����
    private async void OnDestroy()
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by Unity", CancellationToken.None);
            Debug.Log("WebSocket connection closed");
        }
    }

}
