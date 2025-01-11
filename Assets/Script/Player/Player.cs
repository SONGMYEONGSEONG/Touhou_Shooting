using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    [SerializeField] PlayerData data = null;
    [SerializeField] Reimou_Bullet_Spread SpreadPrefab = null; // 확산 총알 프리팹
    [SerializeField] Reimou_Bullet_Straight StraightPrefab = null; // 직선 총알 프리팹
    [SerializeField] Bomb BombPrefab = null; // 폭탄 프리팹

    [SerializeField] float interval_angle = 30.0f; // 확산공격시 발사 사이 간격
    [SerializeField] float interval_distance = 0.3f; // 직선공격시 발사 사이 간격
    [SerializeField] Vector2 player_startPos; //플레이어 시작위치
    [SerializeField] float invincibleTime = 0.5f; //플레이어 무적시간
    [SerializeField] float RebornPosY = -4.5f; //플레이어 부활y좌표   

    float power; //플레이어 파워 단계      

    bool isSlow; //저속이동 bool 변수
    float AttackTimer;// 공격쿨타임(속도) 체크 변수
    float BombTimer;// 폭탄 쿨타임 체크 변수
    int BombCount; //가지고있는 폭탄의 갯수 
    int CurLife; // 플레이어 현재 목숨 
    bool isinvincible; //플레이어 무적임을 나타내는 변수
    bool isboundaryPass; //맵의 경계선을 무시 할수 있는 변수 
    public bool IsInvincible
    {
        get { return isinvincible; }
        set { isinvincible = value; }
    }

    public int Bombcount => BombCount;
    public float Power => power;
    public int life
    {
        get { return CurLife; }
        set { CurLife = value; }
    }
    public int MaxLife
    {
        get { return data.MaxLife; }
    }

    public bool Isinvincible { set { isinvincible = value; } }
    Vector2 velocity; // 플레이어의 현재 속도 

    // 경계선 좌표를 나타냄 
    Vector2 left_bottom;
    Vector2 right_top;

    Animator anim;
    Rigidbody2D rigid;
    CircleCollider2D coll;
    SpriteRenderer render;
    Transform playerHitArea; //저속이동시 화면에 출력되는 피격포인트 

    Coroutine Firecoroutine = null; //총알 발사 코루틴 
    Coroutine InvincibleCoroutine = null; //무적시간 적용 코루틴

    private void OnDestroy()
    {
        Debug.Log("Player Object 파괴");
    }

    private void Start()
    {
        //탄막 프리팹 세팅
        if (!SpreadPrefab) { SpreadPrefab = Resources.Load<Reimou_Bullet_Spread>("Prefabs/Reimou_Bullet_Spread"); }
        if (!StraightPrefab) { StraightPrefab = Resources.Load<Reimou_Bullet_Straight>("Prefabs/Reimou_Bullet_Straight"); }
        //폭탄 프리팹 세팅
        if(!BombPrefab) { BombPrefab = Resources.Load<Bomb>("Prefabs/Reimou_Bomb"); }

        //플레이어 저속이동시 나오는 피격지점 출력 오브젝트 세팅
        if (!playerHitArea) { playerHitArea = transform.GetChild(0); }
    }

    public void Set_Position(Vector2 pos) {   transform.position = pos; }


    public void Initialize(Vector2 bound_min, Vector2 bound_max, GameData curGameData)
    {
        if (!rigid)
        {
            rigid = GetComponent<Rigidbody2D>();
            rigid.freezeRotation = true;
        }
        if (!anim)
        {
            anim = GetComponent<Animator>();
        }
        if (!coll)
        {
            coll = GetComponent<CircleCollider2D>();
            coll.isTrigger = true;
        }
        if(!render)
        {
            render = GetComponent<SpriteRenderer>();
        }

        //플레이어 경계선 적용
        left_bottom = bound_min;
        right_top = bound_max;

        //플레이어 등장(무적 적용)
        //StartCoroutine(PlayerStartMove(InvisibleInputDisable()));
        StartCoroutine(OnInvincible());

        //플레이어 데이터 세팅
        isSlow = false; // 저속이동 사용x
        isinvincible = false; //무적 미적용 
        AttackTimer = data.Duration; //공격속도 설정
        power = curGameData.Playerpower; //파워 단계
        CurLife = curGameData.Playerlife; // 플레이어 목숨 세팅
        BombCount = curGameData.Playerbomb; //플레이어 폭탄 세팅

        //플레이어 데이터 UI 전달
        StageMgr.Instance.AddPlayerLife(CurLife);//플레이어 목숨 UI 전달
        StageMgr.Instance.AddPlayerBomb(BombCount);//플레이어 폭탄갯수 UI 전달 
        StageMgr.Instance.AddPlayerPower(power);//플레이어 파워레벨 UI 전달 
    }
    public void Move(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();

        velocity = dir * data.Speed;

        MoveAnimation(dir);
    }

    private void MoveAnimation(Vector2 dir)
    {
        if (dir.x > 0) { anim.SetBool("Right", true); return; }
        else if (dir.x < 0) { anim.SetBool("Left", true); return; }

        //오른쪽,왼쪽이 다 안눌린경우 false
        anim.SetBool("Right", false);
        anim.SetBool("Left", false);
        
    }

    public void SlowOn(InputAction.CallbackContext context) 
    {
        isSlow = true;

        playerHitArea.gameObject.SetActive(true);

    }
    public void SlowOff(InputAction.CallbackContext context) 
    { 
        isSlow = false;

        playerHitArea.gameObject.SetActive(false);

    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.started && AttackTimer >= data.Duration)
        {
            AttackTimer = 0f;
            Firecoroutine = StartCoroutine(Fire());

            //Player_Bullet.Run(0,Vector2.up,data.Bulletoffset, data.Duration, data.Duration,Power);
        }
        else if (context.canceled)
        {
            StopCoroutine(Firecoroutine);
        }
    }

    public void Bomb(InputAction.CallbackContext context)
    {
        if(context.started && BombCount > 0 && BombTimer >= data.BombDuration)
        {

            //StageMgr.Instance.SoundMgr.PlaySFX("SFX_Player_Bomb");
            SoundMgr.Instance.PlaySFX("SFX_Player_Bomb");

            BombTimer = 0f;
            BombCount--;
            StageMgr.Instance.AddPlayerBomb(BombCount);

            Bomb bomb = Instantiate(BombPrefab);
            //bomb.UseBomb(rigid.transform.position);
            bomb.UseBomb(transform.position);
        }
    }

  
    private void Fire_Dir_Set_Spread(ref Queue<Vector2> bullet_dir_queue, ref Queue<float> bullet_angle_queue, int OnPower) //발사 방향 각도 세팅 - 확산발사
    {
        if (OnPower % 2 != 0 )
        {
            bullet_dir_queue.Enqueue(Vector2.up); //파워가 홀수 인경우 위로 발사하는 방향 세팅
            bullet_angle_queue.Enqueue(0); //파워가 홀 수인경우 플레이어와 수평으로 발사하는 각도 세팅

            if (OnPower == 1) return; //파워가 1단계인경우 함수탈출
        }

        float x, y; // 탄막의 좌표
        int angle_magnification = OnPower - 1; //각도의 배율(파워에 의해 각도의 크기가 변경됨)
        int shot_bullet__count = Mathf.FloorToInt(OnPower * 0.5f);//탄막이 발사되는 갯수(파워의 내림에의해 결정)

        while (shot_bullet__count > 0)
        {
            float angle = (interval_angle * 0.5f) * angle_magnification;

            //상수화 해놓고 쓸것 =  Mathf.PI / 180.0f
            y = Mathf.Cos(angle *  Mathf.PI / 180.0f);
            x = Mathf.Sin(angle * Mathf.PI / 180.0f);

            bullet_dir_queue.Enqueue(new Vector2(-x, y));
            bullet_angle_queue.Enqueue(angle * 1);

            bullet_dir_queue.Enqueue(new Vector2(x, y));
            bullet_angle_queue.Enqueue(angle * -1);
            shot_bullet__count--;

            //power 올림 (한발 사이의 간격 30 도)
            //올림 2/2 - > 1(올림) -> 1 * 30 = 15*1
            //올림 3/2 -> 1.5(올림) -> 2 * 30 = 15*2
            //올림 4/2 -> 2(올림) -> 2 *30 = 15+30 = 15*3
            //올림 5/2 -> 2.5(올림) ->3 * 30 + 30 = 15*4
            angle_magnification -= 2; // 짝수,홀수에 따라 발사 방향,각도가 2단계씩 바껴야해서 -2 감소를함
        }

    }

    private void Fire_Pos_Set_Straight(ref Queue<float> bullet_distance_queue, int OnPower) //발사 시작 위치 세팅 - 직선발사
    {
        //power가 홀수인 경우 중앙발사 위치를 queue에 저장
        if (OnPower % 2 != 0) { bullet_distance_queue.Enqueue(0); }

        //발사 왼쪽(-1),오른쪽(+1) 위치를 잡아줌
        for (int i = 0; i < (int)(power * 0.5f) ; i++)
        {
            bullet_distance_queue.Enqueue(-interval_distance * (i + 1));
            bullet_distance_queue.Enqueue(interval_distance * (i + 1));
        }
    }

    private void FixedUpdate()
    {
        rigid.velocity = velocity;
        if (isSlow) { rigid.velocity *= data.Slowspeed; }

        if (!isboundaryPass)
        {
            //플레이어 캐릭터의 경계선 제어
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, left_bottom.x, right_top.x),
            Mathf.Clamp(transform.position.y, left_bottom.y, right_top.y));
        }
    }

    private void Update()
    {
        AttackTimer += Time.deltaTime;
        BombTimer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string collisionTag = collision.tag;

        switch(collisionTag)
        {
            //적과 충돌하거나 적의 총알에 충돌했을때
            case "Enemy":
            case "EnemyBullet":
                if(!isinvincible) // 무적 상태가 아닐때
                {
                    isinvincible = true; //죽음과 동시에 플레리어 캐릭터블 무적 적용 
                    SoundMgr.Instance.PlaySFX("SFX_Player_Dead");//플레이어 SFX Dead사운드 적용

                    ParticleBase DeadParticle = StageMgr.Instance.PartcleMgr.Instance("Particle_Player_Die_explosion");  //플레이어 이펙트 Dead 적용
                    DeadParticle.ParticlePlay(transform.position);

                    StageMgr.Instance.ItemMgr.PlayerDeadItemSummon(transform.position, power); // 플레이어가 죽은경우 아이템을 떨구는 함수
                    StageMgr.Instance.AddPlayerLife(--CurLife); // 플레이어 목숨을 UI에 적용 

                    //플레이어가 죽은경우 처리하는 함수
                    OnDie();

                    if (CurLife < 0) // 목숨을 다 쓴경우
                    {
                        /*캐릭터 초기화 및 게임오버 UI 팝업창 띄워야함 */
                        CurLife = data.Life;
                    }   
                }
                break;

            case "Item": // 아이템과 충돌 했을때
                //Debug.Log("Player - Item 터치");

                // 모든 Item(GameObject)들은 ItemBase 클래스를 상속 받기 때문에,
                // GetComponenet를 이용하여 ItemBase를 찾아 Use() 함수를 호출하여 사용 가능.
                if (collision.TryGetComponent(out ItemBase item))
                {
                    item.OnUse(gameObject);
                }

                break;

        }

    }

    public void PowerUp(float value)
    {
        switch (power >= data.PowerMax)
        {
            case true:
                StageMgr.Instance.AddScore((int)(value * 100)/*점수 배율*/);
                //Debug.Log("Power 초과, 점수로 변경 " + (value * 100));
                break;

            case false:
                power += value;
                StageMgr.Instance.AddPlayerPower(power); // UIPower Up
                if(power >= data.PowerMax) { SoundMgr.Instance.PlaySFX("SFX_Player_Full_Power_Up"); }
                //Debug.Log("Power 적용 " + power);
                break;
        }
    }

    public void BombCountUp(int value)
    {
        switch (BombCount >= data.BombMaxCount)
        {
            case true:
                StageMgr.Instance.AddScore((value * 100)/*점수 배율*/);
                Debug.Log("폭탄 갯수 초과, 점수로 변경 " + (value * 100));
                break;

            case false:
                BombCount += value;
                StageMgr.Instance.AddPlayerBomb(BombCount); // UI 폭탄 갯수 출력
                Debug.Log("폭탄 갯수 증가 적용 " + BombCount);
                break;
        }
    }

    IEnumerator Fire()
    {
        while(true)
        {
            //StageMgr.Instance.SoundMgr.PlaySFX("SFX_Player_Fire");
            SoundMgr.Instance.PlaySFX("SFX_Player_Fire");

            //총알에 사용되는 파워 정수 값 
            int OnPower = Mathf.FloorToInt(power);

            Queue<Vector2> bullet_dir_queue = new Queue<Vector2>(); //확산 - 탄막 방향을 저장하는 큐
            Queue<float> bullet_angle_queue = new Queue<float>(); //확산 - 탄막 각도을 저장하는 큐
            Queue<float> bullet_distance_queue = new Queue<float>(); //직선 - 탄막 사이 거리을 저장하는 큐

            //캐릭터의 발사되는 탄막의 객체 리스트
            List<Bullet> bullet_spread_list = new List<Bullet>();
            //Vector2 Player_pos = rigid.transform.position; // 탄막 발사 시작 위치 초기화
            Vector2 Player_pos = transform.position; // 탄막 발사 시작 위치 초기화

            switch (isSlow)
            {
                // 총알 발사 간격 사이 설정
                case true:  Fire_Pos_Set_Straight(ref bullet_distance_queue, OnPower); break;
                //총알 발사 방향 설정 
                case false:  Fire_Dir_Set_Spread(ref bullet_dir_queue, ref bullet_angle_queue , OnPower); break;
            }

            for (int i = 0; i < OnPower; i++)
            {
                if (isSlow)
                {
                    bullet_spread_list.Add(StageMgr.Instance.BulletMgr.Instance(SpreadPrefab.name));
                }
                else
                {
                    bullet_spread_list.Add(StageMgr.Instance.BulletMgr.Instance(StraightPrefab.name));
                }

                bullet_spread_list[i].Initialize();
                Vector2 Fire_pos = Player_pos;

                if (isSlow) { Fire_pos.x += bullet_distance_queue.Dequeue(); }
                bullet_spread_list[i].SetPosition(Fire_pos +data.Bulletoffset);

                if (isSlow)
                {
                    bullet_spread_list[i].SetRotaion(0);
                    bullet_spread_list[i].OnFire(Vector2.up, data.Bulletoffset);
                }
                else
                {
                    bullet_spread_list[i].SetRotaion(bullet_angle_queue.Dequeue());
                    bullet_spread_list[i].OnFire(bullet_dir_queue.Dequeue(), data.Bulletoffset);
                }
            }

            yield return new WaitForSeconds(data.Duration);
        }
    }
    public void OnDie() // 플레이어가 죽었을경우 처리되는 함수
    {
        InvincibleCoroutine = StartCoroutine(OnInvincible());//무적이 적용되며 플레이어input 및 부활 애니메이션 코루틴
    
        //죽은경우 파워와 폭탄갯수 초기값 설정 
        BombCount = data.BombCount;
        StageMgr.Instance.UIMgr.UpdatePlayerBomb(BombCount);
        power = data.PowerMin;
        StageMgr.Instance.UIMgr.UpdatePlayerPower(power);
    }

    IEnumerator InvincibleAnimation()
    {
        float curTimer = 0;//무적시간 타이머
        bool InvincibleCheck = false;
        while (curTimer <= invincibleTime)
        {
            //해당 타이머 홀,짝수에 따라 무적시간 투명도 적용 
            switch (InvincibleCheck/*curTimer % 2 == 0*/)
            {
                case true:
                    render.color = new Color32(255, 255, 255, 210);
                    break;

                case false:
                    render.color = new Color32(255, 255, 255, 40);
                    break;
            }

            InvincibleCheck = !InvincibleCheck;

            yield return new WaitForSeconds(0.1f);
            curTimer += 0.1f;
        }
    }

    IEnumerator PlayerStartMove(InputActionMap PlayerActionMap) // 플레이어가 시작지점까지 자동으로 이동해주는 코루틴
    {
        PlayerActionMap.Disable();
        isboundaryPass = true; // 경계선 무시
        //해당 플레이어의 시작위치로 이동후 캐릭터가 무적시간동안 위로 올라옴 
        Set_Position(player_startPos); //플레이어 위치 설정
        velocity = Vector2.up * (data.Speed * 0.5f);
        //

        while (true)
        {
            //플레이어 Input이 활성화상태면 비활성화상태로 바꾼다 .
            if(PlayerActionMap.enabled)
            { 
                PlayerActionMap.Disable(); 
            }
            
            //버그수정 (메모) 
            //rigid.transform.position = 로컬좌표
            //rigid.position = 월드좌표 
            //해당 부활 경계선까지 도착시 정지 및 플레이어 input 활성화 
            //if (rigid.position.y >= RebornPosY)
            if (transform.position.y >= RebornPosY)
            {
                PlayerActionMap.Enable();
                velocity = Vector2.zero;
                isboundaryPass = false;
                yield break; 
            }
            yield return null;
        }
       
    }

    IEnumerator OnInvincible() // 무적 적용시간과 연출을 따로 구분해야됨 
    {
        InputActionMap PlayerActionMap = InvisibleInputDisable();

        //해당 플레이어의 시작위치로 이동후 캐릭터가 무적시간동안 위로 올라옴 
        StartCoroutine(PlayerStartMove(PlayerActionMap));
  
        yield return StartCoroutine(InvincibleAnimation()); // 다른 코르틴이 끝날 때까지 대기

        //무적시간을 다 사용한 후 무적을 해제하는 코드 
        render.color = new Color32(255, 255, 255, 255);
        isinvincible = false;

        yield return null;
    }



    //Player의 Input Action map을 Return 하는 함수 
    private InputActionMap InvisibleInputDisable()
    {
        PlayerInput input;
        GameMgr.Instance.TryGetComponent<PlayerInput>(out input);

        InputActionAsset actionAsset = input.actions;

        InputActionMap actionmap = actionAsset.FindActionMap("Player");

        return actionmap;
    }
}
