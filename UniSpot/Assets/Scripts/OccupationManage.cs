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
            // 점령 시작
            isOccupation = true;
            Txt_Occupation.text = "점령 중";
            Bt_Occupation.interactable = false;
            Info.text = $"현재 {UserName.text} 유저가 점령중입니다";
        }
    }

    public void StopOcuupation()
    {
        if (isOccupation)
        {
            // 점령 중지
            isOccupation = false;
            Txt_Occupation.text = "점령하기";
            Bt_Occupation.interactable = true; // 버튼 활성화
            Info.text = "점령을 완료했습니다";
        }
    }
}
