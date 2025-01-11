using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Boss1_Bullet1 : Bullet
{
    [SerializeField] int SpreadBulletCount = 4;//퍼지는 총알의 갯수
    [SerializeField] float[] BulletEndPointY;//탄막의 터지는 위치y 좌표 리스트

    Coroutine PatternCoroutine = null;
    Queue<Boss1_SubBullet1_Spread> boss1_Bullet1_Spread;

    Queue<Vector2> bullet_dir_queue = new Queue<Vector2>(); //환산 - 탄막 방향을 저장하는 큐

    private void SpreadeBulletInitilalize()
    {
        boss1_Bullet1_Spread = new Queue<Boss1_SubBullet1_Spread>();
        float angle = 360 / SpreadBulletCount;

        //설정한 탄환을 미리 이 탄환 리스트에 넣어놓음
        for(int i =0; i < SpreadBulletCount; i++)
        {
            //부모객체(bullet)에 오브젝트 풀에서 꺼낸 자식객체(Boss1_SubBullet_Spread)를 업캐스팅
            Bullet bullet = StageMgr.Instance.BulletMgr.Instance("Boss1_SubBullet_Spread");

            //업캐스팅 된 부모(bullet)을 다운캐스팅하여 Boss1_SubBullet_Spread로 복구하기
            Boss1_SubBullet1_Spread bulletPoolObj = (Boss1_SubBullet1_Spread)bullet;

            //제대로 다운캐스팅 되었는지 확인하기 
            if (bulletPoolObj)
            {
                bulletPoolObj.SetPosition(rigid.transform.position);
                boss1_Bullet1_Spread.Enqueue(bulletPoolObj);

                //각도에 따른 방향 벡터 만들기
                float x = Mathf.Cos((angle * i) * Mathf.PI / 180.0f);
                float y = Mathf.Sin((angle * i) * Mathf.PI / 180.0f);

                bullet_dir_queue.Enqueue(new Vector2(x, y));
            }
        }
    }

    public override void OnFire(Vector2 dir, Vector2 offset_pos)
    {
        base.OnFire(dir, offset_pos);

        if (PatternCoroutine == null) { PatternCoroutine = StartCoroutine(Spread()); }
        else
        {
            StopCoroutine(PatternCoroutine);
            PatternCoroutine = null;

            PatternCoroutine = StartCoroutine(Spread());
        }
    }
    public override void OffFire()
    {
        if (PatternCoroutine != null)
        {
            StopCoroutine(PatternCoroutine);
            PatternCoroutine = null;
        }
        base.OffFire();
    }
    private void SpreadShot() // 연두색 총알이 산탄되는 코드
    {
        SpreadeBulletInitilalize(); // 연두색 총알 오브젝트 풀링 및 방향값 세팅

        while (boss1_Bullet1_Spread.Count > 0) //총알 갯수가 존재할경우 큐의 갯수만큼 발사
        {
            Vector2 dir = bullet_dir_queue.Dequeue();
            boss1_Bullet1_Spread.Dequeue().OnFire(dir, new Vector2(0, 0));
        }
    }

    IEnumerator Spread() //주황색 총알이 터지면서 산탄되는 코루틴
    {
        //주황색 총알이 터지는 위치 배열을 랜덤으로 하나 가져온다.
        int Random_index = Random.RandomRange(0, BulletEndPointY.Length);
        while (true)
        {
            //가져온 위치에 도달할시 주황색총알이 터지고 연두색 총알이 발사된다.
            if (rigid.transform.position.y <= BulletEndPointY[Random_index])
            {
                SpreadShot(); //연두색 총알을 360도 원으로 발사한다.
                base.OffFire();
                yield break;
            }

            yield return null;
        }
    }


}
