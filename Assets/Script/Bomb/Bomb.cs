using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//폭탄 범위에 닿은 총알들은 오브젝트가 사라져야함
//폭탄 범위에 닿은 적들은 폭탄의 데미지 만큼 hp가 감소해야함
//반지름이 코루틴 업데이트 주기마다 커지면 그 코루틴 주기마다 원이 커진다.
//방법 1. 보간(Lerp)를 사용하여 원을 연출한다.
//방법 2. 폭탄의 리소스 범위 스케일를 키워서 커지게 연출한다. 
//tip. 폭탄의 pivot을 플레이어 중앙이 아닌 밑으로 잡고 위쪽으로 늘리는 방식으로 하는게 좋다.
//(기획서 - 폭탄 참고 필요 ) 

//회전 - > 애니메이션 사용할것 (현재 Bomb 애니메이션으 러프적용시켜서 회전시키는중)

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Bomb : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] float Sclae_Speed = 0.2f; //폭탄의 범위가 커지는 속도
    [SerializeField] float Max_Scale = 30f; //폭탄이 최대 커지는 크기

    Rigidbody2D rigid;
    Animator anim;

    Coroutine Bombcoroutine = null;

    public void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        gameObject.SetActive(true);
    }

    public void UseBomb(Vector2 Start)
    {
        if (!gameObject.activeSelf) return;

        gameObject.SetActive(true);

        transform.position = new Vector2(Start.x, Start.y);//폭탄의 시작 위치 초기화
        anim.Play("Reimou_Rotation_Boom"); // 폭탄 회전 애니메이터 플레이
        Bombcoroutine = StartCoroutine(MoveBomb());
    }

    private void OffBomb()
    {
        StopCoroutine(Bombcoroutine);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    IEnumerator MoveBomb()
    {
        while (true)
        {
            //크기 최대치에 도달시 코루틴 정지 및 오브젝트 off
            if (transform.localScale.x >= Max_Scale) { OffBomb(); }

            //폭탄의 크기 증가
            float Up_Scale = Sclae_Speed * Time.deltaTime;
            transform.localScale += new Vector3(Up_Scale, Up_Scale, Up_Scale);

            yield return null;
        }
    }

}
