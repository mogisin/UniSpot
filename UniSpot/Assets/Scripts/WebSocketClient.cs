using System;
using System.Collections;
using System.Text;
using UnityEngine;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI; 
using TMPro;
using UnityEngine.SceneManagement;

public class WebSocketClient : MonoBehaviour
{
    private ClientWebSocket webSocket;
    public TextMeshProUGUI useridname;

    async void Start()
    {
        // WebSocket ���� ����
        webSocket = new ClientWebSocket();
        Uri serverUri = new Uri("ws://172.20.10.7:8000");
        await webSocket.ConnectAsync(serverUri, CancellationToken.None);

        Debug.Log("WebSocket connected!");

        // �޽��� ����
        SendGetUserMoneyMessage($"testUser");

        // ���� ������ ����ϴ� �񵿱� �Լ� ȣ��
        await ReceiveMessagesAsync();
    }


    // ������ get_user_money �޽��� ������
    private async void SendGetUserMoneyMessage(string username)
    {
        // JSON �������� �޽��� �ۼ� (�̽������� ���� ����)
        string message = $@"
        {{
            ""type"": ""get_user_money"",
            ""username"": ""{username}""
        }}";

        byte[] bytesToSend = Encoding.UTF8.GetBytes(message);

        // ������ �޽��� ����
        await webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);

        Debug.Log("Sent: " + message);
    }

    // �����κ��� �޽��� ���� (�񵿱�)
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

                // JSON �޽��� �Ľ�
                MessageResponse response = JsonUtility.FromJson<MessageResponse>(message);

                // "type" �ʵ带 Ȯ���Ͽ� �޽��� Ÿ���� ����
                if (response.type == "res_user_money")
                {
                    // ���ŵ� money ���� ServerData�� ����
                    ServerData.userMoney = response.money;  // ���ŵ� ���� ����
                    Debug.Log("User money: " + ServerData.userMoney);

                    // "money" ���� TextMeshProUGUI�� ǥ��
                    if (moneyText != null)
                    {
                        moneyText.text = $"{ServerData.userMoney}";
                    }

                    if (moneyText2 != null)
                    {
                        moneyText2.text = $"{ServerData.userMoney}";
                    }

                    // GameObject�� Ư�� ������ �Ҵ�
                    if (someGameObject != null)
                    {
                        if (response.money > 1000)
                        {
                            someGameObject.SetActive(true);
                        }
                        else
                        {
                            someGameObject.SetActive(false);
                        }
                    }
                }
                else if (response.type == "res_duplicate_money")
                {
                    Debug.Log("Duplicate money response received.");
                    // �ߺ��� money ���信 ���� �ٸ� ó��
                }
                else
                {
                    Debug.Log("Unknown message type: " + response.type);
                }
            }
        }
    }

    // �޽����� ���� Ŭ���� ����
    [System.Serializable]
    public class MessageResponse
    {
        public string type;  // �޽����� Ÿ�� �ʵ�
        public int money;    // �޽����� money �ʵ�
    }

    // WebSocket ���� �ݱ�
    private async void OnApplicationQuit()
    {
        if (webSocket != null)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            webSocket.Dispose();
            Debug.Log("WebSocket closed.");
        }
    }

    // ������ money �� ������Ʈ�ϱ�
    public void SendUpdateUserMoneyMessage(int newMoneyValue)
    {
        // JSON �������� �޽��� �ۼ�
        string message = $@"
    {{
        ""type"": ""update_user_money"",
        ""username"": ""testUser"",
        ""money"": {newMoneyValue}
    }}";

        byte[] bytesToSend = Encoding.UTF8.GetBytes(message);

        // ������ �޽��� ����
        webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);

        Debug.Log("Sent money update to server: " + message);
    }

    // ������ ���� ���� ������
    public async void SendMonsterNameMessage(string monsterName)
    {
        string message = $@"
    {{
        ""type"": ""capture_monster"",
        ""username"": ""testUser"",
        ""monsterName"": ""{monsterName}""
    }}";

        byte[] bytesToSend = Encoding.UTF8.GetBytes(message);

        // ������ �޽��� ����
        await webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);

        Debug.Log("Sent monster name to server: " + message);
    }

    // ������ ���� �̸� ������Ʈ �޽��� ������
    public async void SendUsernameUpdateMessage(string newUsername)
    {
        // JSON �������� �޽��� �ۼ�
        string message = $@"
    {{
        ""type"": ""update_username"",
        ""username"": ""{newUsername}""
    }}";

        byte[] bytesToSend = Encoding.UTF8.GetBytes(message);

        // ������ �޽��� ����
        try
        {
            await webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);
            Debug.Log("Sent username update to server: " + message);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to send username update: " + e.Message);
        }
    }

    // ������ ���� ���� ���� ������
    public void SendMonsterInfoMessage(int newMoneyValue)
    {
        // JSON �������� �޽��� �ۼ�
        string message = $@"
    {{
        ""type"": ""get_duplicate_name"",
        ""username"": ""testUser""
    }}";

        byte[] bytesToSend = Encoding.UTF8.GetBytes(message);

        // ������ �޽��� ����
        webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);

        Debug.Log("Sent money update to server: " + message);
    }


    // JSON ������ ������ ǥ���� Ŭ���� ����
    [System.Serializable]
    public class MoneyResponse
    {
        public string type;
        public string username;
        public int money;
    }

    // �ʵ� �߰�
    public TextMeshProUGUI moneyText; // TextMeshProUGUI�� ���� ǥ���ϱ� ���� �ʵ�
    public TextMeshProUGUI moneyText2;
    public GameObject someGameObject; // money ���� ���� ������ GameObject �ʵ�

    // �� �̵� �� ����ȣ�� ����
    private async void OnDisable()
    {
        // WebSocket ������ �����ϰ� ���� �ִ��� Ȯ��
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing due to scene change", CancellationToken.None);
            webSocket.Dispose();
            Debug.Log("WebSocket closed due to scene change.");
        }
    }



}
