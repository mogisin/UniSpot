using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlotToggle : MonoBehaviour
{
    public Animator itemSlotAnimator; // ItemSlot�� Animator
    private bool isExpanded = false; // ���� Ȯ�� ���¸� ������ ����

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Bt_Inventory ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public void ToggleItemSlot()
    {
        // isExpanded ���� ���
        isExpanded = !isExpanded;

        // �ִϸ������� �Ķ���͸� ����
        itemSlotAnimator.SetBool("isExpanded", isExpanded);
    }

    public void UI_Anim_ItemSlot_Expanded()
    {

    }
}
