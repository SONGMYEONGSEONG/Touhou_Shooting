using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//Enemy의 부모클래스(추상클래스)
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public abstract class EnemyBase : MonoBehaviour,IPoolingObject
{
    [Header("EnemyBase")]
    [SerializeField] protected Vector2 Start_pos; //객체가 처음시작하는 위치
    [SerializeField] protected Vector2 End_pos; //객체가 최종도착하는 위치
    [SerializeField] protected EnemyData data = null; //enemydata를 스크립타블오브젝트로 활용할 예정 

    [Header("Enemy Move Pattern List")]
    [SerializeField] protected EnemyMovePattern[] MovePattern = null; //이동종류 배열

    [Header("Enemy OnHit Display")]//적이 공격에 받았을경우 나타내는 표시
    [SerializeField] protected float HitDisplayTime = 0.3f;//공격 받았다는 시간을 알려주는 시간  
    protected Coroutine Hitcoroutine = null; //Player에게 공격 받았을경우 표시 

    [Header("Particle")]
    [SerializeField] protected ParticleSystem Particle_OnHit = null;

    //Enemy 객체가 오브젝트 풀링에 돌려주는 이벤트함수
    public event System.Action<EnemyBase> DieEnemyEvent = null;

    //Enemy 객체가 죽으면 Item을 떨구는 함수
    public event System.Action<EnemyBase,Vector2> ItemDropEvent = null;

    string gainItem;//가지고있는 아이템 종류
    int gaintItemCount;//가지고있는 아이템 갯수

    public string GaintItem => gainItem; //죽을때 떨어트리는 아이템 종류
    public int GaintItemCount => gaintItemCount; //죽을때 떨어트리는 아이템의 갯수

    ////Enemy 객체가 죽으면 Item을 떨구는 함수
    //public event System.Action<Vector2> ItemDropEvent = null; 

    protected PatternBase pattern; //해당 몹이 사용할 패턴->ex)HP 상태에 따른 공격종류 변화

    protected float HelathPoint; // 객체의 체력 포인트
    protected Rigidbody2D rigid;
    protected CircleCollider2D coll;
    protected SpriteRenderer render;

    Color EnemyColor;
      
    protected virtual void Awake()
    {
        //오브젝트 풀링 수정중
        //플레이어 총알 (오브젝트 풀링) 세팅
        //if (!bulletController) { bulletController = GetComponent<BulletController>(); }

        if (!rigid)
        {
            rigid = GetComponent<Rigidbody2D>();
            rigid.freezeRotation = true;
            gameObject.transform.position = Start_pos;
        }
        if (!coll)
        {
            coll = GetComponent<CircleCollider2D>();
            coll.isTrigger = true; //콜리더가 키네마틱이라 충돌처리를 만들기위해 트리거체크
        }
        if (!render)
        {
            render = GetComponent<SpriteRenderer>();
        }

        HelathPoint = data.MaxHealth;
        EnemyColor = render.color;

        //해당 Enemy객체의 패턴이 <Pattern_Normal>인경우 Init시작
        //아닌경우 BossBase에서 Boss 패턴 Init 함
        if (pattern = GetComponentInChildren<Pattern_Normal>()) { pattern.Initialize(HelathPoint); }
       else { Debug.Log("EnemyBase : 패턴 컴포넌트 미적용"); }

        gameObject.SetActive(false);
    }
    //!interface
    public virtual bool Initialize()
    {
        if (MovePattern == null) return false;

        gameObject.SetActive(true);
        SetPosition(Start_pos);
        //bulletController.Initialize();
        return true;
    }
    public void SetPosition(Vector2 pos)
    {
        //rigid.transform.position = pos;
        transform.position = pos;
    }
    public void OffActiveObject() { }
    //!interface
    //떨어지는 아이템 Init 세팅
    public virtual void DropItemInit(string Item, int ItemCount)
    {
        gainItem = Item;
        gaintItemCount = ItemCount;
    }


    public void SetStartPosition(Vector2 pos) { Start_pos = pos; }
    public void SetEndPosition(Vector2 pos) { End_pos = pos; }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        string collisionTag = collision.tag;

        switch(collisionTag)
        {
            case "BulletBoundary": //맵의 경계선에 닿은경우
                BulletBoundaryTouch();
                break;

            case "PlayerBullet":  //플레이어 탄막에 닿은경우 
                PlayerBulletOnHIt();
              
                break;

            case "PlayerBomb":  //플레이어의 폭탄에 닿은경우 
                PlayerBombOnHIt();
                break;
        }

    }

    protected void BulletBoundaryTouch()
    {
        Debug.Log("Enemy -BulletBoundary 터치");

        DieEnemyEvent?.Invoke(this);
        //오브젝트 off
        //gameObject.SetActive(false);
    }
    protected void UseDieEnemyEvent() { DieEnemyEvent?.Invoke(this); }
    protected void UseItemDropEvent() { ItemDropEvent?.Invoke(this,rigid.position); }
    protected void EventAllClear()
    {
        ItemDropEvent = null;
        DieEnemyEvent = null;
    }

    protected void UseDieParticle()
    { 
        ParticleBase particle = StageMgr.Instance.PartcleMgr.Instance("Particle_Enemy_Die_explosion");
        particle.ParticlePlay(rigid.position);
    }

    protected void PlayerBulletOnHIt()
    {
        //Debug.Log("Enemy - Player_Bullet 터치");
        HelathPoint--;

        if (Particle_OnHit)
        {
            Particle_OnHit.Play();
        }

        if (HelathPoint <= 0)   //HP가 0이되는경우 적오브젝트 사망
        {
            UseDieParticle();

            //점수 중복방지를 위한 오브젝트 생존체크후 점수추가
            if (gameObject.activeSelf) { StageMgr.Instance.AddScore(data.GetScore); }
            ItemDropEvent?.Invoke(this, transform.position);
            DieEnemyEvent?.Invoke(this);
            //모든 이벤트함수 사용후 이벤트 비우기
            EventAllClear();
        }
        else // 적 HP 감소
        {
            Hitcoroutine = StartCoroutine(OnHit());
            StageMgr.Instance.UIMgr.UpdateHpBarSlider(HelathPoint, data.MaxHealth);
        }
    }

    protected void PlayerBombOnHIt()
    {
        UseDieParticle();
        StageMgr.Instance.AddScore(data.GetScore);
        ItemDropEvent?.Invoke(this, transform.position);
        DieEnemyEvent?.Invoke(this);
        EventAllClear();
    }

    protected IEnumerator OnHit()
    {
        float curTimer = 0;//히트시간 타이머
        

        while (curTimer <= HitDisplayTime)
        {
            //해당 타이머 홀,짝수에 따라 무적시간 투명도 적용 
            switch (curTimer % 2 == 0)
            {
                case true:
                    //render.color = new Color32(255, 255, 255, 90);
                    EnemyColor.a = 0.3f;
                    render.color = EnemyColor;
                    break;

                case false:
                    //render.color = new Color32(255, 255, 255, 180);
                    EnemyColor.a = 0.85f;
                    render.color = EnemyColor;
                    break;
            }
            yield return new WaitForSeconds(0.1f);
            curTimer += 0.1f;
        }


        //render.color = new Color32(255, 255, 255, 255);
        EnemyColor.a = 1.0f;
        render.color = EnemyColor;
        yield return null;
    }

}
