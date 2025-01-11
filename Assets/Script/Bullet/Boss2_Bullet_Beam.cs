using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_Bullet_Beam :Bullet
{
    [SerializeField] BoxCollider2D BoxColl = null;

    Coroutine RotateCoorutine;
    public override void OnFire(Vector2 dir, Vector2 offset_pos)
    {
        BoxColl.enabled = true;

        this.dir = dir;
        this.offset_pos = offset_pos;

        gameObject.SetActive(true);//총알이 움직이기 시작될때 살아남

        if (RotateCoorutine == null)
        {
            RotateCoorutine = StartCoroutine(Rotate(this.dir));
        }

    }
    public override void OffFire() { DieBullet(); }

    [SerializeField] float Beam_Scale_Speed = 5.0f;
    [SerializeField] float Beam_Scale_y_Max = 6.0f;
    [SerializeField] float Beam_Rotate_Speed = -5.0f;
    [SerializeField] float Beam_Angle_Max = 110.0f;

    private void ScaleBeam() // 빔 크기 키우기
    {
        Vector3 new_scale = transform.localScale;
        new_scale.y += Beam_Scale_Speed * Time.deltaTime;
        transform.localScale = new_scale;
    }
    private void RotateBeam(float angle,float rotationAmount) // 빔 방향 회전
    {
        angle = Beam_Rotate_Speed * dir.x * Time.deltaTime;

        Quaternion new_Rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation *= new_Rotation;

        rotationAmount += Mathf.Abs(angle);  // 변경된 부분: 회전된 각도를 누적
    }

    IEnumerator Rotate(Vector2 dir)
    {
        float rotationAmount = 0f;//회전한 각도를 누적하는 변수 
        float angle = 0f; //한 루프당 회전한 각도

        switch (dir.x)
        {
            case 1://Vector2.right
                while (transform.localScale.y < Beam_Scale_y_Max)
                {
                    ScaleBeam();
                }
                while (rotationAmount < Beam_Angle_Max)
                {
                    RotateBeam(angle, rotationAmount);
                }
                break;

            case -1://Vector2.left
                while (transform.localScale.y > Beam_Scale_y_Max * dir.x )
                {
                    ScaleBeam();
                }
                while (rotationAmount < Beam_Angle_Max)
                {
                   RotateBeam(angle, rotationAmount);
                }
                break;
        }
        yield return new WaitForSeconds(1.0f);
        OffFire();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        string collisionTag = collision.tag;

        switch (collisionTag)
        {   
            case "PlayerBomb":
                if (gameObject.tag == "EnemyBullet")
                {
                    if (collisionTag.Equals("PlayerBomb")) { BulletDIeParticle(); }
                    OffFire();
                };
                break;
        };
    }


}
