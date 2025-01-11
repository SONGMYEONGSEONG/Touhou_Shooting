using System; //Serialzeable 사용 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//아이템이 플레이어의 위치를 탐색해서 따라오게하는 컴포넌트
public class ItemPlayerSearch : MonoBehaviour
{
    ItemBase IsItem = null; // 현재 적용되고있는 아이템 
    [SerializeField] float Chasespeed = 8.0f;
    [SerializeField] float GetBoderLinespeed = 20.0f;
    bool getBoderLineCheck;

    public bool GetBoderLineCheck
    {
        get { return getBoderLineCheck; }
        set { getBoderLineCheck = value; }
    }


    private void Awake()
    {
        IsItem = GetComponentInParent<ItemBase>();
        getBoderLineCheck = false;
    }

    public void PlayerChaseItem()
    {
        //플레이어벡터 - 아이템벡터 = 아이템->플레이어 방향벡터
        Vector2 dir = (StageMgr.Instance.Player.transform.position - gameObject.transform.position).normalized;
        IsItem.MoveDir = dir;
      
        switch(getBoderLineCheck)
        {
            case true:    IsItem.Speed = GetBoderLinespeed; break;
            case false:   IsItem.Speed = Chasespeed; break;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (GetBoderLineCheck) return;

        switch (collision.tag)
        {
            case "Player": //Player인경우
                PlayerChaseItem();
                break;
        }

    } 

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    switch (collision.tag)
    //    {
    //        case "Player": //Player인경우

    //            IsItem.MoveDir = Vector2.down;
    //            IsItem.Speed = 1.0f;
    //            break;
    //    }

    //}
}
