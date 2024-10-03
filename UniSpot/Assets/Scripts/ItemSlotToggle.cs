using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlotToggle : MonoBehaviour
{
    public Animator itemSlotAnimator; // ItemSlot의 Animator
    private bool isExpanded = false; // 현재 확장 상태를 저장할 변수

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Bt_Inventory 버튼 클릭 시 호출되는 함수
    public void ToggleItemSlot()
    {
        // isExpanded 값을 토글
        isExpanded = !isExpanded;

        // 애니메이터의 파라미터를 설정
        itemSlotAnimator.SetBool("isExpanded", isExpanded);
    }

    public void UI_Anim_ItemSlot_Expanded()
    {

    }
}
