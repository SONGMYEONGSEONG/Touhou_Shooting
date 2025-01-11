using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ScrollBackGroundController : MonoBehaviour
{
    [SerializeField] float ScrollSpeed = 0.1f;
    [SerializeField] Transform[] BackGroundGroup = null;
    [SerializeField] int Index;
    [SerializeField] float ScrollPoint = -11.32f;//백그라운드 이미지의 pivot이 해당 스크롤 포인트에 도달시 동작 조건문
    //[SerializeField] float ScrollInitPoint_X = -2.78f;
    [SerializeField] float ScrollInitPoint_Y = 12.4f;

    private void FixedUpdate()
    {
        Vector2 CurPos = transform.position;
        Vector2 NextPos = Vector2.down * ScrollSpeed * Time.fixedDeltaTime;
        transform.position = CurPos + NextPos;

        if (BackGroundGroup[Index].transform.position.y < ScrollPoint)
        {
            //BackGroundGroup[Index].transform.position = new Vector2(ScrollInitPoint_X, ScrollInitPoint_Y);
            BackGroundGroup[Index].transform.position = new Vector2(BackGroundGroup[Index].transform.position.x, ScrollInitPoint_Y);
            Index++;

            if(Index >= BackGroundGroup.Length)
            {
                Index = 0;
            }
        }
    }


}
