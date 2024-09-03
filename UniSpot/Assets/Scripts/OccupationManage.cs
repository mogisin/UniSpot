using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using UnityEngine.UI;
using TMPro;

public class OccupationManage : MonoBehaviour
{
    public static OccupationManage Instance { get; private set; }

    public Button Bt_Occupation;
    public Text Info;
    public Text Txt_Occupation;
    private bool isOccupation = false;

    public TMP_InputField inputField;
    public TMP_Text UserName;
    private string userNameText = "";

    void Start()
    {
        Bt_Occupation.onClick.AddListener(OnOcuupationBt);
        inputField.onValueChanged.AddListener(OnInputFieldChange);
    }

    void OnInputFieldChange(string text)
    {
        if (UserName != null) 
        {
            UserName.text = text;       
        }

    }
    void OnOcuupationBt()
    {
        if (!isOccupation) 
        {
            // ���� ����
            isOccupation = true;
            Txt_Occupation.text = "���� ��";
            Bt_Occupation.interactable = false;
            Info.text = $"���� {UserName.text} ������ �������Դϴ�";
        }
    }

    public void StopOcuupation()
    {
        if (isOccupation)
        {
            // ���� ����
            isOccupation = false;
            Txt_Occupation.text = "�����ϱ�";
            Bt_Occupation.interactable = true; // ��ư Ȱ��ȭ
            Info.text = "������ �Ϸ��߽��ϴ�";
        }
    }
}
