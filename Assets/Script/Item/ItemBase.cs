using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
//Enemy의 부모클래스(추상클래스)
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]

public abstract class ItemBase : MonoBehaviour, IPoolingObject
{
    [SerializeField] CircleCollider2D ItemColl = null;
    [SerializeField] CircleCollider2D ItemSearchColl = null;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float speed = 1.0f;
    public float Speed { set { speed = value; } }

    ItemPlayerSearch itemPlayerSearch = null;
    float InitSpeed;//초기 스피드
    //Item이 플레이어와 충돌한뒤 동작하는 이벤트 함수 
    public event System.Action<ItemBase> DieItemEvent = null;

    protected string itemName;
    public string ItemName => itemName;
    bool Up_Finish; //아이템이 위로 튀어오르는 연출의 상태 체크
    //[SerializeField] float itemDropPercent = 100.0f; //해당 아이템이 드랍될 확률 
    //public float ItemDropPercent => itemDropPercent;

    Vector2 moveDir;
    public Vector2 MoveDir
    {
        get { return moveDir; }
        set { moveDir = value; }
    }

    Coroutine Movecoroutine = null;
    Rigidbody2D rigid;


    public virtual void Awake()
    {
        if(!rigid) { rigid = GetComponent<Rigidbody2D>(); }
        if(!itemPlayerSearch)
        {
            
            itemPlayerSearch = GetComponentInChildren<ItemPlayerSearch>();
        }
        InitSpeed = speed;
    }
    //interface
    public virtual bool Initialize()
    {
        gameObject.SetActive(true);
        speed = InitSpeed;
        Up_Finish = false;
        MoveDir = Vector2.zero;
        itemPlayerSearch.GetBoderLineCheck = false;

        if (Movecoroutine == null) 
        { 
            Movecoroutine = StartCoroutine(MoveItem()); 
        } //아이템이 아래로 떨어지는 코드 구현

        return false;
    }

    public virtual void SetPosition(Vector2 pos)
    {
        transform.position = pos;
        return;
    }

    public void OffActiveObject() // 플레이어가 특정 y지점을 지나갈시 동작하는 함수
    {
        itemPlayerSearch.GetBoderLineCheck = true; //플레이어가 특정 y지점을 넘어감을 체크하는 변수 
        itemPlayerSearch.PlayerChaseItem();
    }
    //!interface
    public virtual bool OnUse(GameObject target)
    {
        if(target.CompareTag("Player"))
        {
            return true;
        }

        return true;
    }

    protected void UsedItem() // Item을 다 사용한경우 DIeItem 이벤트 함수를 호출하는 함수(자식용)
    {
        if (Movecoroutine != null) //코루틴 초기화
        {
            rigid.velocity = Vector2.zero;//속도 0 초기화
            StopCoroutine(Movecoroutine);
            Movecoroutine = null;
        }
        DieItemEvent?.Invoke(this);
    }

    private void  OnTriggerEnter2D(Collider2D collision)
    {
        if (!Up_Finish) { return; } // 아이템이 위로 올라가는 연출이 끝난경우 시작 

        switch (collision.tag)
        {
            case "BulletBoundary": //맵의 경계선에 닿은경우(플레이어가 아이템을 못먹은경우)

                if (Movecoroutine != null) //코루틴 초기화
                {
                    rigid.velocity = Vector2.zero;//속도 0 초기화
                    StopCoroutine(Movecoroutine);
                    Movecoroutine = null;
                }

                DieItemEvent?.Invoke(this);

                break;
        }
        
    }


    IEnumerator MoveItem()
    {
        while(true)
        {
            switch(Up_Finish)
            {
                case true:
                    rigid.velocity = MoveDir * speed;
                    break;
                case false:

                    int angle = Random.Range(45, 135); // 45 ~ 135 도의 사이중 랜덤으로 방향이 정해짐 

                    float x = Mathf.Cos(angle * Mathf.PI / 180.0f);
                    float y = Mathf.Sin(angle * Mathf.PI / 180.0f);

                    MoveDir = new Vector2(x, y);

                    rigid.velocity = MoveDir * (speed * 2); //아이템이 위로 튀어오르는 연출
                    yield return new WaitForSeconds(0.5f);
                    MoveDir = Vector2.down;
                    Up_Finish = true;
                    break;
            }
         
            yield return null;
        }
        
    }
}
