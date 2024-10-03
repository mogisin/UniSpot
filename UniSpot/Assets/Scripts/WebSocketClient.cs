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
        // WebSocket 연결 시작
        webSocket = new ClientWebSocket();
        Uri serverUri = new Uri("ws://172.20.10.7:8000");
        await webSocket.ConnectAsync(serverUri, CancellationToken.None);

        Debug.Log("WebSocket connected!");

        // 메시지 전송
        SendGetUserMoneyMessage($"testUser");

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

                // JSON 메시지 파싱
                MessageResponse response = JsonUtility.FromJson<MessageResponse>(message);

                // "type" 필드를 확인하여 메시지 타입을 구분
                if (response.type == "res_user_money")
                {
                    // 수신된 money 값을 ServerData에 저장
                    ServerData.userMoney = response.money;  // 수신된 값을 저장
                    Debug.Log("User money: " + ServerData.userMoney);

                    // "money" 값을 TextMeshProUGUI에 표시
                    if (moneyText != null)
                    {
                        moneyText.text = $"{ServerData.userMoney}";
                    }

                    if (moneyText2 != null)
                    {
                        moneyText2.text = $"{ServerData.userMoney}";
                    }

                    // GameObject에 특정 동작을 할당
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
                    // 중복된 money 응답에 대해 다른 처리
                }
                else
                {
                    Debug.Log("Unknown message type: " + response.type);
                }
            }
        }
    }

    // 메시지에 대한 클래스 정의
    [System.Serializable]
    public class MessageResponse
    {
        public string type;  // 메시지의 타입 필드
        public int money;    // 메시지의 money 필드
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

    // 서버에 money 값 업데이트하기
    public void SendUpdateUserMoneyMessage(int newMoneyValue)
    {
        // JSON 형식으로 메시지 작성
        string message = $@"
    {{
        ""type"": ""update_user_money"",
        ""username"": ""testUser"",
        ""money"": {newMoneyValue}
    }}";

        byte[] bytesToSend = Encoding.UTF8.GetBytes(message);

        // 서버로 메시지 전송
        webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);

        Debug.Log("Sent money update to server: " + message);
    }

    // 서버에 몬스터 정보 보내기
    public async void SendMonsterNameMessage(string monsterName)
    {
        string message = $@"
    {{
        ""type"": ""capture_monster"",
        ""username"": ""testUser"",
        ""monsterName"": ""{monsterName}""
    }}";

        byte[] bytesToSend = Encoding.UTF8.GetBytes(message);

        // 서버로 메시지 전송
        await webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);

        Debug.Log("Sent monster name to server: " + message);
    }

    // 서버에 유저 이름 업데이트 메시지 보내기
    public async void SendUsernameUpdateMessage(string newUsername)
    {
        // JSON 형식으로 메시지 작성
        string message = $@"
    {{
        ""type"": ""update_username"",
        ""username"": ""{newUsername}""
    }}";

        byte[] bytesToSend = Encoding.UTF8.GetBytes(message);

        // 서버로 메시지 전송
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

    // 서버에 몬스터 보유 정보 보내기
    public void SendMonsterInfoMessage(int newMoneyValue)
    {
        // JSON 형식으로 메시지 작성
        string message = $@"
    {{
        ""type"": ""get_duplicate_name"",
        ""username"": ""testUser""
    }}";

        byte[] bytesToSend = Encoding.UTF8.GetBytes(message);

        // 서버로 메시지 전송
        webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);

        Debug.Log("Sent money update to server: " + message);
    }


    // JSON 데이터 구조를 표현할 클래스 정의
    [System.Serializable]
    public class MoneyResponse
    {
        public string type;
        public string username;
        public int money;
    }

    // 필드 추가
    public TextMeshProUGUI moneyText; // TextMeshProUGUI에 값을 표시하기 위한 필드
    public TextMeshProUGUI moneyText2;
    public GameObject someGameObject; // money 값에 따라 동작할 GameObject 필드

    // 씬 이동 시 서버호출 관리
    private async void OnDisable()
    {
        // WebSocket 연결이 존재하고 열려 있는지 확인
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing due to scene change", CancellationToken.None);
            webSocket.Dispose();
            Debug.Log("WebSocket closed due to scene change.");
        }
    }



}
