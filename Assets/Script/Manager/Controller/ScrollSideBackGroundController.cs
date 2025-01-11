using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollSideBackGroundController : MonoBehaviour
{
    [SerializeField] Vector2 ScrollDir; //스크롤될 방향을 정함 
    [SerializeField] float ScrollSpeed = 0.1f;
    [SerializeField] float ScrollPoint = -11.32f;//백그라운드 이미지의 pivot이 해당 스크롤 포인트에 도달시 동작 조건문
    [SerializeField] float ScrollInitPoint_X = -2.78f;

    Transform m_Transform;

    private void Awake()
    {
        m_Transform = gameObject.transform;
    }

    private void FixedUpdate()
    {
        Vector2 CurPos = m_Transform.position;
        Vector2 NextPos = ScrollDir * ScrollSpeed * Time.fixedDeltaTime;
        m_Transform.position = CurPos + NextPos;

        if (ScrollDir.x * m_Transform.position.x >= ScrollPoint)
        {
            m_Transform.position = new Vector2(ScrollInitPoint_X, transform.position.y);
        }
       
    }
}
