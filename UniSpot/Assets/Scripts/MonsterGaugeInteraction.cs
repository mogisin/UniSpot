using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MonsterGaugeInteraction : MonoBehaviour
{
    public Image gaugeImage; // Gauge_Frame�� Image ������Ʈ
    public float fillSpeed = 0.5f; // ������ ���� �ӵ�
    public float delayBeforeIncrease = 1f; // ������ ���� �� ��� �ð�
    public GameObject resultSuccess; // Result_Success ������Ʈ

    void Start()
    {
        // ������ ������ ������ �Ŀ� ����
        StartCoroutine(StartIncreasingGaugeWithDelay());
    }

    // ������ ���� ���� (������ ����)
    System.Collections.IEnumerator StartIncreasingGaugeWithDelay()
    {
        Debug.Log("������ ������ ���� ��� �ð� ����.");
        // ������ ������ �ð� ���� ���
        yield return new WaitForSeconds(delayBeforeIncrease);

        Debug.Log("������ ���� ����!");
        // �� �����Ӹ��� ������ ����
        StartCoroutine(IncreaseGauge());
    }

    // �������� ���������� ������Ű�� �ڷ�ƾ
    System.Collections.IEnumerator IncreaseGauge()
    {
        while (gaugeImage.fillAmount < 1f)
        {
            gaugeImage.fillAmount += fillSpeed * Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        Debug.Log("������ �ִ�ġ�� ����!");

        // �������� �ִ�ġ�� �������� �� Result_Success�� Ȱ��ȭ
        resultSuccess.SetActive(true);

        // 5�� �Ŀ� ���� ȭ������ �̵�
        yield return new WaitForSeconds(5f);

        // ���� ȭ������ �̵�
        SceneManager.LoadScene("Main Scene");
    }
}
