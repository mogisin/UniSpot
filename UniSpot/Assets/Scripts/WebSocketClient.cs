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

                // JSON 메시지 파싱
                MoneyResponse response = JsonUtility.FromJson<MoneyResponse>(message);

                // 수신된 money 값을 ServerData에 저장
                ServerData.userMoney = response.money; // 수정된 부분: 수신된 값을 저장

                // "money" 값을 TextMeshProUGUI에 표시
                if (moneyText != null)
                {   
                    moneyText.text = $"{ServerData.userMoney}";
                }

                // "money" 값을 두 번째 TextMeshProUGUI에도 표시
                if (moneyText2 != null)
                {
                    moneyText2.text = $"{ServerData.userMoney}";
                }

                // GameObject에 특정 동작을 할당
                if (someGameObject != null)
                {
                    // 예시로 GameObject를 활성화 또는 비활성화
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
