using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackAroundSpinStar", menuName = "PatternScriptableObject/Attack/AroundSpinStar", order = int.MaxValue)]

public class AttackAroundSpinStar : EnemyAttackPattern
{
    [Header("Boss2_Spin_Star_Prefab")]
    [SerializeField] string bullet_prefab_name;
    [SerializeField] float circleR = 3.0f; //원의 반지름
    [SerializeField] float cur_deg = 0f; //현재 발사 각도 
    [SerializeField] float start_deg = 0f; //초기 시작 각도
    [SerializeField] float deg = 10.0f; //한발씩 발사할때마다 늘어나는 각도
    [SerializeField] int shotCount = 2;//한번에 발사하는 bullet의 갯수

    public override bool UsePattern(EnemyBase enemy, EnemyData data)
    {
        Vector2 bullet_pos = enemy.transform.position;
        Vector2 dir = Vector2.zero;

        if (cur_deg - start_deg <= 360) { cur_deg += deg; } // 각도가 360도(원)을 안돌은경우 발사 각도를 n도씩 증가
        else 
        { 
            cur_deg = start_deg;
        } //각도가 원(360)을 넘긴경우 360도를 빼고 원래 시작각도로 초기화 ( cur_deh -360 *2)

        float interval_angle = 360 / shotCount; //발사하는 기준의 각도

        for (int i = 0; i < shotCount; i++)
        {
            Bullet bulletPoolObj = StageMgr.Instance.BulletMgr.Instance(bullet_prefab_name);

            float rad;
            // i==0 과 i ==1 일때의 발사 방향 각도를 조절 (i가 0과 1일떄 정반대의 각도를 나타냄)
            if (i == 0) { rad = Mathf.Deg2Rad * (cur_deg); }
            else { rad = Mathf.Deg2Rad * (cur_deg + (interval_angle * i)); }
            
            float x = circleR * Mathf.Sin(rad);
            float y = circleR * Mathf.Cos(rad);
            dir = new Vector2(x, y);
            dir.Normalize();
            bulletPoolObj.SetPosition(bullet_pos + data.Bulletoffset);
            bulletPoolObj.OnFire(dir, data.Bulletoffset);
        }

        SoundMgr.Instance.PlaySFX("SFX_Enemy_Boss_Attack");
        return true;
    }
}
