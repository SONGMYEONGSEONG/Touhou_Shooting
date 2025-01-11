using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossBase : EnemyBase
{
    protected bool isInvincible = true; //보스 첫등장시 무적임을 나타내는 변수
    public event System.Action DieBossEvent = null; //Boss객체가 죽을때 스테이지를 클리어해주는  이벤트함수 

    Coroutine BossFinishcoroutine = null;

    private void OnDestroy()
    {
        Debug.Log("Boss를 처리하였습니다.");    
    }

    protected override void Awake() 
    {
        base.Awake();//EnemyBase로 접근 


        if (pattern = GetComponentInChildren<Pattern_HP_Decrease>())
        {
            pattern.Initialize(base.HelathPoint);
        }
        else { Debug.Log("BossBase : 패턴 컴포넌트 미적용"); }

    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        string collisionTag = collision.tag;

        switch (collisionTag)
        {
            case "PlayerBullet":  //플레이어 탄막에 닿은경우 

                if(!isInvincible)
                {
                    //Debug.Log("Enemy - Player_Bullet 터치");
                    HelathPoint--;

                    if (Particle_OnHit) { Particle_OnHit.Play(); }

                    if (HelathPoint <= 0)   //HP가 0이되는경우 적오브젝트 사망
                    {
                        StageMgr.Instance.UIMgr.BossPatternChange();//패턴 변경 

                        isInvincible = true; //HP가 0이하인경우 때려도 처리되지않게 무적처리
                        StageMgr.Instance.Player.Isinvincible = true; //플레이어 체력이 0이되서 스테이지 클리어 되게 무조건처리

                        //보스죽음 연출 코루틴이 null일때만 재생 
                        if (BossFinishcoroutine == null)
                        {
                            BossFinishcoroutine = StartCoroutine(BossFinish());
                        }

                    }
                    else // 적 HP 감소
                    {
                        Hitcoroutine = StartCoroutine(OnHit());
                        StageMgr.Instance.UIMgr.UpdateHpBarSlider(HelathPoint, data.MaxHealth);
                    }
                }

                break;

        }

    }

    //보스가 죽은뒤 처리되는 event함수들 모음 
    private void BossFinishEvent()
    {
        //보스 죽음처리시 클리어창 띄우는 event
        DieBossEvent?.Invoke();
        DieBossEvent = null;

        //Enemy 죽은후 오브젝트 풀에 다시 돌리는 함수 
        base.UseDieEnemyEvent();

        //모든 이벤트함수 사용후 이벤트 비우기
        base.EventAllClear();
    }


    IEnumerator BossFinish()
    {
        //파티클 애니메이션 재생 
        ParticleBase Sub_Explosion = StageMgr.Instance.PartcleMgr.Instance("Particle_Boss_Die_sub_explosion"); //서브 폭발 파티클
        ParticleBase Main_Explosion = StageMgr.Instance.PartcleMgr.Instance("Particle_Boss_Die_main_explosion"); //메인 폭발 파티클 

        StageMgr.Instance.PartcleMgr.cameraShakeOn();

        for (int i = 0; i < 5; i++)
        {
            SoundMgr.Instance.PlaySFX("SFX_Boss_Die_Sub_Explosion");
            //Vector2 pos = new Vector2(rigid.position.x + Random.Range(-1f, 1f), rigid.position.y + Random.Range(-1f, 1f));
            Vector2 pos = new Vector2(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f));
            Sub_Explosion.ParticlePlay(pos);
            yield return new WaitForSeconds(0.2f);
        }

        SoundMgr.Instance.PlaySFX("SFX_Boss_Die_Main_Explosion");
        //Main_Explosion.ParticlePlay(rigid.position);
        Main_Explosion.ParticlePlay(transform.position);

        //보스 폭파 연출 이후 보스 오브젝트 이미지 투명화
        //해당 SpriteRender의 color 변수를 호출
        Color currentColor = base.render.color;
        // 알파 값을 변경합니다.
        currentColor.a = 0; //투명도 = 0 설정
        //변경된 색상을 적용합니다.
        base.render.color = currentColor;

        //해당 조건이 false가 될떄까지 대기 (폭파 파티클이 끝날때까지)
        yield return new WaitWhile(() => Main_Explosion.isParticle);

        //보스가 죽는 연출과 점수를 얻음 
        StageMgr.Instance.AddScore(data.GetScore);

        BossFinishEvent();
    }



}
