using System;
using System.Collections;
using System.Text;
using UnityEngine;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketClient : MonoBehaviour
{
    private ClientWebSocket webSocket;

    async void Start()
    {
        // WebSocket 연결 시작
        webSocket = new ClientWebSocket();
        Uri serverUri = new Uri("ws://172.20.10.7:8000");
        await webSocket.ConnectAsync(serverUri, CancellationToken.None);

        Debug.Log("WebSocket connected!");

        // 메시지 전송
        SendGetUserMoneyMessage("testUser");

        // 서버 응답을 대기하는 비동기 함수 호출
        await ReceiveMessagesAsync();
    }

    // 서버로 get_user_money 메시지 보내기
    private async void SendGetUserMoneyMessage(string username)
    {
        // JSON 형식으로 메시지 작성 (이스케이프 문자 없이)
        string message = $@"
        {{
            ""type"": ""get_user_money"",
            ""username"": ""{username}""
        }}";

        byte[] bytesToSend = Encoding.UTF8.GetBytes(message);

        // 서버로 메시지 전송
        await webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);

        Debug.Log("Sent: " + message);
    }

    // 서버로부터 메시지 수신 (비동기)
    private async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[1024];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Debug.Log("Received: " + message);
            }
        }
    }

    // WebSocket 연결 닫기
    private async void OnApplicationQuit()
    {
        if (webSocket != null)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            webSocket.Dispose();
            Debug.Log("WebSocket closed.");
        }
    }
}
