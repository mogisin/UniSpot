using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro ����� ���� ���ӽ����̽�

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // �÷��̾� ���� �� ���� ���� ���� ������
    public string playerID;
    public bool isZoneCaptured;
    public float timer; // Ÿ�̸Ӵ� �� ������ ����
    public TextMeshProUGUI playerIDTextMesh; // TextMeshPro�� ���� �÷��̾� ID�� �Է� ����
    public TextMeshProUGUI timerTextMesh;    // TextMeshPro�� ���� Ÿ�̸� UI ǥ��
    public GameObject occupationOff; // ���� ���� Ȯ���� Bt_Occupation ���� Off ���

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // �� ��ȯ �ÿ��� �ı����� ����
        }
        else
        {
            Destroy(gameObject);  // �ٸ� �ν��Ͻ��� �ִٸ� ���� �������� ����
        }
    }
    
    private void Start()
    {
        // �÷��̾� ID �⺻ �� ���� (�� ���� ��� "User Name")
        playerID = playerIDTextMesh.text;
        if (string.IsNullOrEmpty(playerID))
        {
            playerID = "User Name";
            playerIDTextMesh.text = playerID;  // TextMeshPro�� �⺻ �� �ݿ�
        }

        // ���� ���� �ʱ�ȭ
        CheckZoneCaptured();

        // Ÿ�̸� �ʱ�ȭ (00:00)
        ResetTimer();
    }

    // ���� ���� üũ
    public void CheckZoneCaptured()
    {
        // occupationOff�� ��Ȱ��ȭ�Ǿ� ������ ���ɵ��� ���� ����, Ȱ��ȭ�Ǿ� ������ ���ɵ� ����
        isZoneCaptured = occupationOff.activeSelf;
    }

    // Ÿ�̸Ӹ� Ư�� ���ǿ��� 05:00���� ���� �� ����
    public void StartCooldownTimer()
    {
        timer = 5 * 60; // 5��(�� ������ ����)
        UpdateTimerUI();
    }

    // Ÿ�̸Ӹ� �⺻��(00:00)���� �ʱ�ȭ
    public void ResetTimer()
    {
        timer = 0f;
        UpdateTimerUI();
    }

    // Ÿ�̸� UI ������Ʈ (MM:SS ����)
    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerTextMesh.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Ÿ�̸� ������Ʈ
    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerUI();

            // Ÿ�̸Ӱ� ������ 00:00���� �ʱ�ȭ
            if (timer <= 0)
            {
                ResetTimer();
            }
        }

        if (timerActive && timer > 0)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                TimerEnded();
            }
        }
    }

    // �ʿ� �� �����͸� �ʱ�ȭ�ϴ� �޼���
    public void ResetGameData()
    {
        isZoneCaptured = false;
        timer = 0f;
    }

    // �÷��̾� ������ ���� �� �ҷ�����
    public void SaveGameData()
    {
        PlayerPrefs.SetString("PlayerID", playerID);
        PlayerPrefs.SetInt("IsZoneCaptured", isZoneCaptured ? 1 : 0);
        PlayerPrefs.SetFloat("Timer", timer);

        // �÷��̾� ID ����
        PlayerPrefs.SetString("PlayerID", playerIDTextMesh.text);

        PlayerPrefs.Save();
    }

    public void LoadGameData()
    {
        // �÷��̾� ID �ҷ����� (�⺻��: "User Name")
        playerID = PlayerPrefs.GetString("PlayerID", "User Name");
        playerIDTextMesh.text = playerID;

        // ���� ���� �ҷ����� (�⺻��: ���ɵ��� ���� ����)
        isZoneCaptured = PlayerPrefs.GetInt("IsZoneCaptured", 0) == 1;
        occupationOff.SetActive(!isZoneCaptured);  // ���� ���ο� ���� Off ����� ���� ����

        // Ÿ�̸� �� �ҷ����� (�⺻��: 0��)
        timer = PlayerPrefs.GetFloat("Timer", 0f);
        UpdateTimerUI();
    }

    // Ÿ�̸� ��� ��üȭ
    private bool timerActive = false;

    public void StartTimer(float duration)
    {
        timer = duration;
        timerActive = true;
    }

    private void TimerEnded()
    {
        timerActive = false;
        // Ÿ�̸Ӱ� ������ �� ó���� ����
        Debug.Log("Timer has ended!");
    }
}
