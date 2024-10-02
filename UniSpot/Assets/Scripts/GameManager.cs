using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro ����� ���� ���ӽ����̽�
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // �� ���� ����� ����ϱ� ���� �ʿ�

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //public GameData gameData;

    // �÷��̾� ���� �� ���� ���� ���� ������
    public string playerID;
    public bool isZoneCaptured;
    public float timer; // Ÿ�̸Ӵ� �� ������ ����
    public TextMeshProUGUI playerIDTextMesh; // TextMeshPro�� ���� �÷��̾� ID�� �Է� ����
    public TextMeshProUGUI timerTextMesh;    // TextMeshPro�� ���� Ÿ�̸� UI ǥ��
    public GameObject occupationOff; // ���� ���� Ȯ���� Bt_Occupation ���� Off ���
    public Text textSpotInfo; // ������ ����
    public TextMeshProUGUI resource; // �ڿ��� ǥ�ø� ���� ���
    public Text OccupationResource; // ���� ���ɿ� �ʿ��� �ڿ���
    public GameObject OccupationResourceInfo; // ���� ���� ���� ���� ����

    public int resourceValue; // �� ���� �����Ǵ� ���� �ڿ���

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
        LoadGameData();

        // �÷��̾� ID �⺻ �� ���� (�� ���� ��� "User Name")
        playerID = PlayerPrefs.GetString("PlayerID", "User Name");
        playerIDTextMesh.text = playerID;

        // �ڿ��� �ʱ�ȭ
        UpdateResourceUI();

        // ���� ���� �ʱ�ȭ
        isZoneCaptured = PlayerPrefs.GetInt("IsZoneCaptured", 0) == 1;
        occupationOff.SetActive(!isZoneCaptured);  // ���� ���ο� ���� Off ����� ���� ����

        // Ÿ�̸� �ʱ�ȭ (00:00)
        timer = PlayerPrefs.GetFloat("Timer", 0f);
        UpdateTimerUI();
    }

    // �ڿ��� UI ������Ʈ
    public void UpdateResourceUI()
    {
        resource.text = resourceValue.ToString();
    }

    // �� �̵� �Լ�
    public void LoadSelectScene(string Scenename)
    {
        SaveGameData();
        SceneManager.LoadScene(Scenename);
    }
    
    public void LoadCameraScene()
    {
        SaveGameData();
        SceneManager.LoadScene("Camera Scene");
    }
    public void LoadMainScene()
    {
        SaveGameData();
        SceneManager.LoadScene("Main Scene");
    }

    public void LoadSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // �ڿ��� ������Ʈ
    public void UpdateResource(int amount)
    {
        resourceValue += amount; 
        UpdateResourceUI();
    }

    // PlayerPrefs�� �ڿ��� ����
    public void SaveGameData()
    {
        PlayerPrefs.SetString("PlayerID", playerID);
        PlayerPrefs.SetInt("IsZoneCaptured", isZoneCaptured ? 1 : 0);
        PlayerPrefs.SetFloat("Timer", timer);
        PlayerPrefs.SetInt("ResourceValue", resourceValue);

        PlayerPrefs.Save();
    }

    // ���� ���� üũ
    public void CheckZoneCaptured()
    {
        // occupationOff�� ��Ȱ��ȭ�Ǿ� ������ ���ɵ��� ���� ����, Ȱ��ȭ�Ǿ� ������ ���ɵ� ����
        isZoneCaptured = occupationOff.activeSelf;
    }

    public void CaptureZone()
    {
        /*// resource�� occupation resource�� �ؽ�Ʈ ������Ʈ���� ���� �Ľ�
        int currentResourceValue = int.Parse(resource.text); // Assuming the resource text is purely numeric
        int occupationResourceValue = int.Parse(OccupationResource.text); // Assuming the occupation resource text is purely numeric
        */
        int occupationResourceValue = int.Parse(OccupationResource.text);

        // ���� �ڿ� ���� �ʿ��� occupation resource ������ ũ�ų� ������ Ȯ��
        if (resourceValue >= occupationResourceValue)
        {
            // �ڿ��� ���Ҹ� �ϰ��� �ְ� ó��
            UpdateResource(-occupationResourceValue);

            // occupation resource ������ ��Ȱ��ȭ
            OccupationResourceInfo.SetActive(false);

            // ���� Ȯ���� ���� occupation off GameObject Ȱ��ȭ
            occupationOff.SetActive(true);

            // ���� ���� ����
            isZoneCaptured = true;

            // �α�
            Debug.Log("Zone has been successfully captured.");
        }
        else
        {
            Debug.Log("Not enough resources to capture the zone.");
        }
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
        if (timerActive && timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerUI();

            if (timer <= 0)
            {
                timerActive = false;
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

        // �ڿ� �� �ҷ����� (�⺻��: 1000)
        resourceValue = PlayerPrefs.GetInt("ResourceValue", 1000);
        UpdateResourceUI(); // �ڿ� UI ������Ʈ
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
