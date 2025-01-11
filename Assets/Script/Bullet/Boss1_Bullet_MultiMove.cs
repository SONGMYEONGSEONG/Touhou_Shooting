using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1_Bullet_MultiMove : Bullet
{
    [SerializeField] float MultiMoveDistance = 1.0f; // 해당 거리만큼 이동후 턴이동
    Coroutine MoveBulletCoroutine = null;

    private Vector2[] dir_arr;//여러번 이동하는 총알인 경우 이동방향을 보관하는 배열
    enum BulletDir { Negative = 0, Positive = 1 };
    BulletDir bulletindex;
    //여러번 이동이 필요한 총알의 방향값을 저장하는 함수 
    public void Dir_Arr_Set(Vector2[] dir_arr) { this.dir_arr = dir_arr; }

    public override void OnFire(Vector2 dir, Vector2 offset_pos)
    {
        gameObject.SetActive(true);//총알이 움직이기 시작될때 살아남
        bulletindex = BulletDir.Negative;

        if (MoveBulletCoroutine != null)
        {
            StopCoroutine(MoveBulletCoroutine);
            MoveBulletCoroutine = null;
        }
        MoveBulletCoroutine = StartCoroutine(MultiMove(offset_pos));
    }

    public override void OffFire()
    {
        if (MoveBulletCoroutine != null)
        {
            StopCoroutine(MoveBulletCoroutine);
            MoveBulletCoroutine = null;

            DieBullet();
        }        
    }

    IEnumerator MultiMove(Vector2 offset_pos) //탄막의 방향전화 코루틴
    {
        Vector2 Old_Pos = transform.position;
        float accel = accelation;
        float result_speed = speed;
        dir = dir_arr[(int)bulletindex];

        while (true)
        {
            float Cur_Distance = Vector2.Distance(transform.position, Old_Pos);

            //총알 방향 변경 코드
            if (MultiMoveDistance/*이동한거리 체크*/ <= Cur_Distance)
            {
                switch (bulletindex)
                {
                    case BulletDir.Negative:   bulletindex = BulletDir.Positive; break;
                    case BulletDir.Positive:   bulletindex = BulletDir.Negative; break;    
                }
                Old_Pos = transform.position;
                dir = dir_arr[(int)bulletindex];
            }

      
            this.offset_pos = offset_pos;

            //총알 가속도 조절 코드
            switch (accel > 0) //가속도의 값이 +,-에 따라 적용이 다르게 되야됨
            {
                case true: //가속
                    if (result_speed < LimitSpeed) { result_speed += accel; }
                    else if(result_speed >= LimitSpeed ) { result_speed = LimitSpeed; accel = 0; }
                    break;

                case false: //감속
                    //Limit 속도에 걸리지 않는한 가속도가 계속 적용
                    if (result_speed > LimitSpeed) { result_speed += accel; }
                    else if (result_speed <= LimitSpeed) { result_speed = LimitSpeed; accel = 0; }
                    break;
            }
            rigid.velocity = dir * result_speed;
            yield return null;
        }
    }

}


