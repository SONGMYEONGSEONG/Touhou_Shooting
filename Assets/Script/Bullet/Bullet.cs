using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public abstract class Bullet : MonoBehaviour, IPoolingObject
{
    [Header("Speed")]
    [SerializeField]    protected float speed = 1.0f;//총알의 현재 속도
    [SerializeField] protected float accelation = 0.0f; //가속도
    [SerializeField] protected float LimitSpeed = 0.0f; //최저속도
    [SerializeField] int score = 10;//적에게 공격이 성공했을경우 점수가 오르는 비율

    public event System.Action<Bullet> DieBulletObject = null; //총알이 죽었을때 발생되는 event함수
    
    protected Rigidbody2D rigid;
    protected CircleCollider2D coll;

    Coroutine MoveCoroutine = null;

    protected Vector2 dir;//총알이 진행하는 방향
    protected Vector2 offset_pos;//총알의 offset 위치


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
        coll.isTrigger = true;
    }



    //Override
    public virtual void OnFire(Vector2 dir, Vector2 offset_pos)
    {
        gameObject.SetActive(true);//총알이 움직이기 시작될때 살아남

        this.dir = dir;
        this.offset_pos = offset_pos;

        if (MoveCoroutine == null) { MoveCoroutine = StartCoroutine(BulletMove()); }
    }



    public virtual void OffFire()
    {
        if (MoveCoroutine != null)
        {
            StopCoroutine(MoveCoroutine);
            MoveCoroutine = null;

            DieBullet();
        }
    }
    //!Override

    protected void DieBullet()
    {
        DieBulletObject?.Invoke(this); //Bullet을 objectpool에 집어넣는 event함수 동작 
        DieBulletObject = null; // 다사용한 event함수는 null로 초기화(낫둘경우 오브젝트로 남아잇기에 계속 함수가 추가됨)
    }

    //Interface
    public bool Initialize()
    {
        return true; 
    }
    public void SetPosition(Vector2 pos) { transform.position = pos; }
    public void OffActiveObject()
    {
        OffFire();
    }
    //!Interface

    public void SetRotaion(float angle)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    }

    protected void BulletScoreAdd()
    {
        StageMgr.Instance.AddScore(score);
    }

    //Bullet <> Object 충돌처리 동작
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            //게임 화면 밖으로 총알이 나가 경계선과 충돌
            case "BulletBoundary": 
                OffFire(); //총알 오브젝트 비활성화
                break;

            //Bullet <> Player,PlayerBomb 충돌
            case "Player":
            case "PlayerBomb":
                //해당 오브젝트 태그가 "EnemyBullet" 인경우 동작
                if (gameObject.tag == "EnemyBullet")
                {
                    //"PlayerBomb"태그의 오브젝트와 충돌시 이펙트 발생
                    if (collision.tag.Equals("PlayerBomb")) { BulletDIeParticle(); }
                    OffFire(); //총알 오브젝트 비활성화
                };
                break;

            //Bullet <> Enemy,Boss 충돌
            case "Enemy":
            case "Boss":
                //해당 오브젝트 태그가 "PlayerBullet" 인경우 동작
                if (gameObject.tag == "PlayerBullet")
                {
                    BulletDIeParticle(); //총알이 비활성화시 이펙트 발생
                    OffFire(); //총알 오브젝트 비활성화
                    BulletScoreAdd(); //총알을 맞췄으므로 점수 획득   
                };
                break;     
        };
    }

    protected void BulletDIeParticle()
    {
        ParticleBase BulletDiePaticle = StageMgr.Instance.PartcleMgr.Instance("Particle_Bullet_Die_explosion");
        BulletDiePaticle.ParticlePlay(this.transform.position);
    }

    IEnumerator BulletMove() // 총알 객체의 이동 코루틴 
    {
        float accel = accelation;
        float result_speed = speed;

        while (true)
        {
            switch (accel > 0) //가속도의 값이 +,-에 따라 적용이 다르게 되야됨
            {
                case true: //가속
                    if (result_speed < LimitSpeed) { result_speed += accel; }
                    break;

                case false: //감속
                    //Limit 속도에 걸리지 않는한 가속도가 계속 적용
                    if (result_speed > LimitSpeed) { result_speed += accel; }
                    break;
            }
          

            rigid.velocity = dir * result_speed;
            yield return null;
        }
    }
}
