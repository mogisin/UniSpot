using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

public class MonsterGaugeInteraction : MonoBehaviour
{
    public Image gaugeImage; // Gauge_Frame�� Image ������Ʈ
    public float fixedIncreaseAmount = 0.1f; // ��ġ �� ������ ������
    public float fillDecreaseSpeed = 0.1f; // ������ ���� �ӵ�
    public float delayBeforeDecrease = 1f; // ������ ���� �� ��� �ð�
    public GameObject resultSuccess; // Result_Success ������Ʈ
    public GameObject resultFailed; // Result_Failed ������Ʈ

    private bool isGaugeDecreasing = true; // ������ ���� ���� �÷���
    private bool isCaptureFailed = false; // ��ȹ ���� ���� �÷���
    private bool resultFailedActive = false; // Result_Failed Ȱ��ȭ ���� �÷���
    private bool resultSuccessActive = false; // Result_Success Ȱ��ȭ ���� �÷���

    public RectTransform frameCenter; // Frame_Center�� RectTransform
    private Rect frameRect;

    public GameObject slot1; // ���� 1 ������Ʈ
    public GameObject slot2; // ���� 2 ������Ʈ

    public WebSocketClient webSocketClient; // WebSocketClient�� �ν��Ͻ��� ������ ����
    public static event Action OnCaptureSuccess; // ��ȹ ���� �̺�Ʈ

    public TextMeshProUGUI moneyIncreaseText; // ������ų money ���� ��� TextMeshProUGUI ������Ʈ


    // ��ȹ ���� ���θ� ��Ÿ���� ����
    public bool IsCaptureSuccessful { get; private set; }

    void Start()
    {
        // Frame_Center�� RectTransform�� ��ũ�� ��ǥ�� ��ȯ
        Vector3[] frameCorners = new Vector3[4];
        frameCenter.GetWorldCorners(frameCorners);
        frameRect = new Rect(frameCorners[0].x, frameCorners[0].y,
                             frameCorners[2].x - frameCorners[0].x,
                             frameCorners[2].y - frameCorners[0].y);

        // ������ ���Ҹ� ������ �Ŀ� ����
        StartCoroutine(StartDecreasingGaugeWithDelay());
    }

    void Update()
    {
        // ��ġ �Է� ���� (��ȹ ���� ���°� �ƴ� ����)
        if (Input.GetMouseButtonDown(0) && !isCaptureFailed && !resultSuccessActive)
        {
            Vector3 touchPosition = Input.mousePosition;

            // ��ġ �̺�Ʈ �߻�
            HandleTouch(touchPosition);
        }
    }

    // ��ġ �̺�Ʈ ó�� �޼���
    void HandleTouch(Vector3 touchPosition)
    {
        // ��ġ ��ġ�� Frame_Center �ȿ� �ִ��� Ȯ��
        if (frameRect.Contains(touchPosition))
        {
            IncreaseGaugeFixedAmount();
        }
        // Frame ���� �ۿ����� ��ġ �̺�Ʈ�� �߻������� �������� ������ ���� ����
    }

    // ������ ���� ���� (������ ����)
    System.Collections.IEnumerator StartDecreasingGaugeWithDelay()
    {
        Debug.Log("������ ���Ҹ� ���� ��� �ð� ����.");
        // ������ ������ �ð� ���� ���
        yield return new WaitForSeconds(delayBeforeDecrease);

        Debug.Log("������ ���� ����!");
        // �� �����Ӹ��� ������ ����
        StartCoroutine(DecreaseGauge());
    }

    // �������� ���������� ���ҽ�Ű�� �ڷ�ƾ
    System.Collections.IEnumerator DecreaseGauge()
    {
        while (gaugeImage.fillAmount > 0f && isGaugeDecreasing)
        {
            gaugeImage.fillAmount -= fillDecreaseSpeed * Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        // �������� ��� �Ҹ�Ǿ��� ��
        if (gaugeImage.fillAmount <= 0f)
        {
            Debug.Log("�������� ��� �Ҹ�Ǿ����ϴ�.");

            // ��ȹ ���� ���·� ����
            isCaptureFailed = true;

            // Result_Failed Ȱ��ȭ �� ���� �÷��� ����
            resultFailed.SetActive(true);
            resultFailedActive = true;

            // 5�� �Ŀ� ���� ȭ������ �̵�
            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene("Main Scene");
        }
    }

    // ��ġ �� ������ �縸ŭ ������ ����
    void IncreaseGaugeFixedAmount()
    {
        // ������ ����
        gaugeImage.fillAmount += fixedIncreaseAmount;

        // �������� �ִ�ġ�� �����ϸ� ���� ó��
        if (gaugeImage.fillAmount >= 1f)
        {
            gaugeImage.fillAmount = 1f; // ������ �ִ�ġ ����
            Debug.Log("������ �ִ�ġ�� ����!");

            // ��ȹ ���� ó��
            IsCaptureSuccessful = true; // ��ȹ ������ ���

            // ��ȹ ���� �̺�Ʈ �߻�
            OnCaptureSuccess?.Invoke();

            // ������ ���� ����
            isGaugeDecreasing = false;

            // Result_Success Ȱ��ȭ �� ���� �÷��� ����
            resultSuccess.SetActive(true);
            resultSuccessActive = true;

            // TextMeshProUGUI�� �ؽ�Ʈ�� ������ ��ȯ�Ͽ� ������ų ������ ���
            int increaseAmount = int.Parse(moneyIncreaseText.text);
            ServerData.userMoney += increaseAmount;
            Debug.Log("userMoney increased by " + increaseAmount + ". New value: " + ServerData.userMoney);

            // WebSocketClient�� ���� ������ ������Ʈ ����
            if (webSocketClient != null)
            {
                webSocketClient.SendUpdateUserMoneyMessage(ServerData.userMoney);
            }
            else
            {
                Debug.LogWarning("WebSocketClient reference is not set.");
            }

            // 5�� �Ŀ� ���� ȭ������ �̵�
            StartCoroutine(GoToMainSceneAfterDelay(5f));
        }
    }

    // 5�� �� ���� ȭ������ �̵�
    System.Collections.IEnumerator GoToMainSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main Scene");
    }

    // ��ġ�� Result_Failed �Ǵ� Result_Success â�� ��Ȱ��ȭ�Ǵ� ���� ����
    void LateUpdate()
    {
        if (resultFailedActive && !resultFailed.activeSelf)
        {
            resultFailed.SetActive(true);
        }

        if (resultSuccessActive && !resultSuccess.activeSelf)
        {
            resultSuccess.SetActive(true);
        }
    }

    // ���� 1 Ŭ�� �� ������ ���� ��ġ ����
    public void OnSlot1Clicked()
    {
        // ���� money�� 100 �̻��� ��
        if (ServerData.userMoney >= 100)
        {
            // 100 �Ҹ�
            ServerData.userMoney -= 100;

            // WebSocketClient�� ���� ������ ������Ʈ ����
            if (webSocketClient != null)
            {
                webSocketClient.SendUpdateUserMoneyMessage(ServerData.userMoney);
            }
            else
            {
                Debug.LogWarning("WebSocketClient reference is not set.");
            }

            Debug.Log("Slot 2 clicked: 100 money consumed. New money: " + ServerData.userMoney);

            // ������ ������ 10% ����
            fixedIncreaseAmount *= 1.1f;
            Debug.Log("Gauge increase amount increased by 10%.");
        }
        else
        {
            Debug.LogWarning("Not enough money to use Slot 2.");
        }
    }

    // ���� 2 Ŭ�� �� ������ ���� ��ġ ����
    public void OnSlot2Clicked()
    {
        // ���� money�� 150 �̻��� ��
        if (ServerData.userMoney >= 150)
        {
            // 150 �Ҹ�
            ServerData.userMoney -= 150;

            // WebSocketClient�� ���� ������ ������Ʈ ����
            if (webSocketClient != null)
            {
                webSocketClient.SendUpdateUserMoneyMessage(ServerData.userMoney);
            }
            else
            {
                Debug.LogWarning("WebSocketClient reference is not set.");
            }

            Debug.Log("Slot 2 clicked: 150 money consumed. New money: " + ServerData.userMoney);

            // ������ ������ 10% ����
            fixedIncreaseAmount *= 1.1f;
            Debug.Log("Gauge increase amount increased by 10%.");
        }
        else
        {
            Debug.LogWarning("Not enough money to use Slot 2.");
        }
    }

}
