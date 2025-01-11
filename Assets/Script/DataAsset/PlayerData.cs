using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "ScriptableObject/Reimou Data Asset", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Move")]
    [SerializeField] float speed = 10.0f; //기본 이동속도
    [SerializeField] float slowspeed = 0.5f; //저속이동시 배율
    [Header("Bullet")]
    [SerializeField] Vector2 bullet_offset = new Vector2(0, 0.5f); //탄막 생성위치 offset
    [SerializeField] float attack_duration = 0.1f; //한발 발사후 다음공격까지의 대기시간 
    [SerializeField] float bomb_duration = 2.0f; //폭탄 발사후 다음공격까지의 대기시간 
    [Header("Life")]
    [SerializeField] int life = 3; //플레이어의 초기목숨 갯수 
    [SerializeField] int maxLife = 5; //플레이어의 최대목숨 갯수 
    [Header("Power")]
    [SerializeField] float powermin = 1.0f; //파워 단계 최소치(새게임-파워 초기)
    [SerializeField] float powermax = 4.0f; //파워 단계 최대치
    [Header("Bomb")]
    [SerializeField] int bombcount = 2; //가지고있는 폭탄의 갯수(초기 폭탄갯수)
    [SerializeField] int bombMaxcount = 5; //최대가질수있는 폭탄의 갯수

    public float Speed => speed;
    public float Slowspeed => slowspeed;
    public Vector2 Bulletoffset => bullet_offset;
    public float Duration => attack_duration;
    public float BombDuration => bomb_duration;
    public int Life => life;
    public int MaxLife => maxLife;
    public float PowerMin => powermin;
    public float PowerMax => powermax;
    public int BombCount => bombcount;
    public int BombMaxCount => bombMaxcount;
   
}
