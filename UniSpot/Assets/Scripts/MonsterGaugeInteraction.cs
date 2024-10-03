using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

            // ������ ���� ����
            isGaugeDecreasing = false;

            // Result_Success Ȱ��ȭ �� ���� �÷��� ����
            resultSuccess.SetActive(true);
            resultSuccessActive = true;

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
        // ������ ���� �ӵ� 10% ����
        fillDecreaseSpeed *= 0.9f;
        Debug.Log("Slot 1 clicked: Gauge decrease speed reduced by 10%.");
    }

    // ���� 2 Ŭ�� �� ������ ���� ��ġ ����
    public void OnSlot2Clicked()
    {
        // ������ ������ 10% ����
        fixedIncreaseAmount *= 1.1f;
        Debug.Log("Slot 2 clicked: Gauge increase amount increased by 10%.");
    }

}
