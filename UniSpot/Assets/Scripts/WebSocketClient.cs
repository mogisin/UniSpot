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

            // �޽��� ���� ��� ����
            ReceiveWebSocketMessages();

            // WebSocket ���� �� ���������� �Լ� ����
            await ExecuteSequence();
        }
        catch (Exception e)
        {
            LogMessage($"WebSocket connection failed: {e.Message}");
        }
    }

    // WebSocket �޽��� ���� �Լ�
    private async void SendWebSocketMessage(object messageObject)
    {
        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            LogMessage("WebSocket is not connected.");
            return;
        }

        // JsonUtility�� ����Ͽ� JSON ����ȭ
        string jsonMessage = JsonUtility.ToJson(messageObject);
        ArraySegment<byte> messageBytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage));
        await webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        LogMessage($"Sent: {jsonMessage}");
    }

    // ���� ��� ����
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

    // ���� �޽��� ó�� �Լ�
    private void HandleReceivedMessage(string message)
    {
        LogMessage($"Received from server: {message}");
        var data = JsonUtility.FromJson<MessageData>(message);

        if (data.type == "get_all_monsters")
        {
            // ��Ÿ ó��
        }
    }

    // ���������� ������ �Լ�
    private async Task ExecuteSequence()
    {
        LogMessage("Sequence started");

        // 1. Ư�� �а��� ��ġ ����
        SendLocation("����Ʈ�������մ���");
        await Task.Delay(1000);  // 1�� ���

        // 2. ���� ���� ���� ��û
        GenerateMonsters();
        await Task.Delay(1000);  // 1�� ���

        // 3. ��� ������ ��ġ ��û
        GetAllMonsters();
        await Task.Delay(1000);  // 1�� ���

        // 4. ��� ���� ���� ��û
        DeleteAllMonsters();
        await Task.Delay(1000);  // 1�� ���

        // 5. ���� ���� ��ȹ ��û
        CaptureAnyMonster();
        await Task.Delay(1000);  // 1�� ���

        LogMessage("Sequence finished");
    }

    // Ư�� �а��� ��ġ ���� �Լ�
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

    // ���� ���� ���� ��û �Լ�
    public void GenerateMonsters()
    {
        var message = new GenerateMonstersMessage
        {
            type = "generate_monsters",
            count = 1
        };
        SendWebSocketMessage(message);
    }

    // ��� ������ ��ġ ��û �Լ�
    public void GetAllMonsters()
    {
        var message = new GetAllMonstersMessage
        {
            type = "get_all_monsters"
        };
        SendWebSocketMessage(message);
    }

    // ��� ���� ���� ��û �Լ�
    public void DeleteAllMonsters()
    {
        var message = new DeleteAllMonstersMessage
        {
            type = "delete_all"
        };
        SendWebSocketMessage(message);
    }

    // ���� ���� ��ȹ ��û �Լ�
    public void CaptureAnyMonster()
    {
        LogMessage("Capture any monster button clicked.");
        // TODO: ���� ���� ��ȹ ���� �߰�
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

    // �޽��� ������ ����
    [Serializable]
    public class MessageData
    {
        public string type;
        public MonsterData[] monsters;
    }

    // �� ��û �޽��� ����
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

    // ���� ������ ����
    [Serializable]
    public class MonsterData
    {
        public string _id;
        public float latitude;
        public float longitude;
    }
}
