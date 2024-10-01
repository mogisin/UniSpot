using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WebSocketClient : MonoBehaviour
{
    private ClientWebSocket webSocket;

    private async void Start()
    {
        webSocket = new ClientWebSocket();

        try
        {
            await webSocket.ConnectAsync(new Uri("ws://172.20.10.7:8000"), CancellationToken.None);
            LogMessage("WebSocket connection opened.");

            // 메시지 수신 대기 시작
            ReceiveWebSocketMessages();

            // WebSocket 연결 후 순차적으로 함수 실행
            await ExecuteSequence();
        }
        catch (Exception e)
        {
            LogMessage($"WebSocket connection failed: {e.Message}");
        }
    }

    // WebSocket 메시지 전송 함수
    private async void SendWebSocketMessage(object messageObject)
    {
        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            LogMessage("WebSocket is not connected.");
            return;
        }

        // JsonUtility를 사용하여 JSON 직렬화
        string jsonMessage = JsonUtility.ToJson(messageObject);
        ArraySegment<byte> messageBytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage));
        await webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        LogMessage($"Sent: {jsonMessage}");
    }

    // 수신 대기 시작
    private async void ReceiveWebSocketMessages()
    {
        byte[] buffer = new byte[1024];

        while (webSocket.State == WebSocketState.Open)
        {
            ArraySegment<byte> receivedData = new ArraySegment<byte>(buffer);
            WebSocketReceiveResult result = null;

            try
            {
                result = await webSocket.ReceiveAsync(receivedData, CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                HandleReceivedMessage(message);
            }
            catch (Exception e)
            {
                LogMessage($"Error receiving WebSocket message: {e.Message}");
                break;
            }
        }
    }

    // 수신 메시지 처리 함수
    private void HandleReceivedMessage(string message)
    {
        LogMessage($"Received from server: {message}");
        var data = JsonUtility.FromJson<MessageData>(message);

        if (data.type == "get_all_monsters")
        {
            // 기타 처리
        }
    }

    // 순차적으로 실행할 함수
    private async Task ExecuteSequence()
    {
        LogMessage("Sequence started");

        // 1. 특정 학과의 위치 전송
        SendLocation("소프트웨어융합대학");
        await Task.Delay(1000);  // 1초 대기

        // 2. 랜덤 몬스터 생성 요청
        GenerateMonsters();
        await Task.Delay(1000);  // 1초 대기

        // 3. 모든 몬스터의 위치 요청
        GetAllMonsters();
        await Task.Delay(1000);  // 1초 대기

        // 4. 모든 몬스터 삭제 요청
        DeleteAllMonsters();
        await Task.Delay(1000);  // 1초 대기

        // 5. 랜덤 몬스터 포획 요청
        CaptureAnyMonster();
        await Task.Delay(1000);  // 1초 대기

        LogMessage("Sequence finished");
    }

    // 특정 학과의 위치 전송 함수
    public void SendLocation(string department)
    {
        var message = new LocationMessage
        {
            type = "location",
            username = "testUser",
            department = department
        };
        SendWebSocketMessage(message);
    }

    // 랜덤 몬스터 생성 요청 함수
    public void GenerateMonsters()
    {
        var message = new GenerateMonstersMessage
        {
            type = "generate_monsters",
            count = 1
        };
        SendWebSocketMessage(message);
    }

    // 모든 몬스터의 위치 요청 함수
    public void GetAllMonsters()
    {
        var message = new GetAllMonstersMessage
        {
            type = "get_all_monsters"
        };
        SendWebSocketMessage(message);
    }

    // 모든 몬스터 삭제 요청 함수
    public void DeleteAllMonsters()
    {
        var message = new DeleteAllMonstersMessage
        {
            type = "delete_all"
        };
        SendWebSocketMessage(message);
    }

    // 랜덤 몬스터 포획 요청 함수
    public void CaptureAnyMonster()
    {
        LogMessage("Capture any monster button clicked.");
        // TODO: 실제 몬스터 포획 로직 추가
        GetAllMonsters();
    }

    private void LogMessage(string message)
    {
        Debug.Log(message);
    }

    private async void OnDestroy()
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by Unity", CancellationToken.None);
            LogMessage("WebSocket connection closed.");
        }
    }

    // 메시지 데이터 형식
    [Serializable]
    public class MessageData
    {
        public string type;
        public MonsterData[] monsters;
    }

    // 각 요청 메시지 형식
    [Serializable]
    public class LocationMessage
    {
        public string type;
        public string username;
        public string department;
    }

    [Serializable]
    public class GenerateMonstersMessage
    {
        public string type;
        public int count;
    }

    [Serializable]
    public class GetAllMonstersMessage
    {
        public string type;
    }

    [Serializable]
    public class DeleteAllMonstersMessage
    {
        public string type;
    }

    // 몬스터 데이터 형식
    [Serializable]
    public class MonsterData
    {
        public string _id;
        public float latitude;
        public float longitude;
    }
}
