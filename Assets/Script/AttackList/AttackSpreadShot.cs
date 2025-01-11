using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttackSpreadShot", menuName = "PatternScriptableObject/Attack/Spread", order = int.MaxValue)]

public class AttackSpreadShot : EnemyAttackPattern
{
    [SerializeField] int BulletSpreadCount = 5; //한번 공격시 확산되는 총알의 갯수
    [SerializeField] float interval_angle = 30.0f;//탄막과 탄막 사이의 각도

    private Queue<Vector2> Fire_Dir_Set_Spread(int extra_bullet_count = 0)
    {
        int BulletCount = BulletSpreadCount - extra_bullet_count;

        Queue<Vector2> bullet_dir_queue = new Queue<Vector2>(); //환산 - 탄막 방향을 저장하는 큐

        if (BulletCount % 2 != 0) { bullet_dir_queue.Enqueue(Vector2.down); } //홀수 인경우 직선으로 발사하는 방향 세팅  

        float x, y; // 탄막의 좌표
        int angle_magnification = BulletCount - 1; //각도의 배율(탄막갯수에 의해 각도의 크기가 변경됨)
        int shot_bullet__count = Mathf.FloorToInt(BulletCount * 0.5f);//탄막이 발사되는 갯수(탄막갯수의 올림에의해 결정)

        while (shot_bullet__count > 0)
        {
            float angle = (interval_angle * 0.5f) * angle_magnification;

            y = -Mathf.Cos(angle * Mathf.PI / 180.0f); // radian = degree * pi/180
            x = Mathf.Sin(angle * Mathf.PI / 180.0f);

            bullet_dir_queue.Enqueue(new Vector2(-x, y));
            bullet_dir_queue.Enqueue(new Vector2(x, y));
            shot_bullet__count--;

            angle_magnification -= 2; // 짝수,홀수에 따라 발사 방향,각도가 2단계씩 바껴야해서 -2 감소를함
        }

        return bullet_dir_queue;
    }

    public override bool UsePattern(EnemyBase enemy, EnemyData data)
    {
        Queue<Vector2> bullet_dir_queue;
        bullet_dir_queue = Fire_Dir_Set_Spread();

        for (int i = 0; i < BulletSpreadCount; i++)
        {
            Bullet bulletPoolObj = StageMgr.Instance.BulletMgr.Instance("Enemy3_Bullet_Straight");

            if (bulletPoolObj)
            {
                bulletPoolObj.Initialize();
                Vector2 bullet_pos = enemy.transform.position;
                bulletPoolObj.SetPosition(bullet_pos + data.Bulletoffset);
                bulletPoolObj.OnFire(bullet_dir_queue.Dequeue(), data.Bulletoffset);
            }
        }

        SoundMgr.Instance.PlaySFX("SFX_Enemy_Boss_Attack");
        return true;
    }
}
