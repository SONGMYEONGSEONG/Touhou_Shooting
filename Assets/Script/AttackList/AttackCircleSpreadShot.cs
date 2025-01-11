using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttackCircleSpreadShot", menuName = "PatternScriptableObject/Attack/CircleSpreadShot", order = int.MaxValue)]
public class AttackCircleSpreadShot : EnemyAttackPattern
{
    [SerializeField] string[] BulletPrefabs;
    [SerializeField] int RedBulletCount = 2;
    [SerializeField] int BlueBulletCount = 1;

    enum CircleSpreadBulletType { Red =0, Blue = 1 }
    public override bool UsePattern(EnemyBase enemy, EnemyData data)
    {
        Vector2 Bullet_Pos = enemy.transform.position;

        Bullet bulletPoolObj1 = StageMgr.Instance.BulletMgr.Instance(BulletPrefabs[(int)CircleSpreadBulletType.Red]);
        Bullet bulletPoolObj2 = StageMgr.Instance.BulletMgr.Instance(BulletPrefabs[(int)CircleSpreadBulletType.Blue]);

        for (int i = 0; i < BulletPrefabs.Length; i++)
        {
            float angle = 0f, x = 0f , y = 0f;
            Vector2 dir;

            switch (i)
            {
                case (int)CircleSpreadBulletType.Red:
                    for (int j = 0; j < RedBulletCount; j++)
                    {
                        angle = Random.Range(0, 360);

                        x = Mathf.Cos(angle);
                        y = Mathf.Sin(angle);
                        dir = new Vector2(x, y);

                        bulletPoolObj1.SetPosition(Bullet_Pos + data.Bulletoffset);
                        bulletPoolObj1.OnFire(dir, data.Bulletoffset);
                    }
                    break;

                case (int)CircleSpreadBulletType.Blue:
                    for (int j = 0; j < BlueBulletCount; j++)
                    {
                        angle = Random.Range(0, 360);

                        x = Mathf.Cos(angle);
                        y = Mathf.Sin(angle);
                        dir = new Vector2(x, y);

                        bulletPoolObj2.SetPosition(Bullet_Pos + data.Bulletoffset);
                        bulletPoolObj2.OnFire(dir, data.Bulletoffset);
                    }
                    break;

            }
        }


        SoundMgr.Instance.PlaySFX("SFX_Enemy_Boss_Attack");
        return true;
    }
}
