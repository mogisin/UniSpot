using UnityEngine;

public class Software_1 : MonoBehaviour
{
    private WebSocketClient webSocketClient;

    void OnEnable()
    {
        // 이벤트 구독
        MonsterGaugeInteraction.OnCaptureSuccess += OnCaptureSuccess;
    }

    void OnDisable()
    {
        // 이벤트 구독 해제
        MonsterGaugeInteraction.OnCaptureSuccess -= OnCaptureSuccess;
    }

    void Start()
    {
        // WebSocketClient 인스턴스 찾기
        webSocketClient = FindObjectOfType<WebSocketClient>();

        if (webSocketClient == null)
        {
            Debug.LogWarning("WebSocketClient를 찾을 수 없습니다.");
        }
    }

    // 포획 성공 시 호출되는 메서드
    private void OnCaptureSuccess()
    {
        string monsterName = gameObject.name; // 현재 몬스터 오브젝트의 이름
        SendMonsterNameToServer(monsterName);
    }

    // 몬스터의 이름을 서버로 전송하는 메서드
    private void SendMonsterNameToServer(string monsterName)
    {
        if (webSocketClient != null)
        {
            webSocketClient.SendMonsterNameMessage(monsterName);
            Debug.Log("Server에 몬스터 이름을 전송: " + monsterName);
        }
        else
        {
            Debug.LogWarning("WebSocketClient reference is not set.");
        }
    }
    public void Software_1_anim()
    {
        // 애니메이션 이벤트에서 실행하려는 로직
        Debug.Log("Software_1_anim 이벤트가 호출되었습니다.");
    }
}
