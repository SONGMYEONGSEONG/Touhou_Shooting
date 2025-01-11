using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "ScriptableObject/Enemy Data Asset",order =2)]
public class EnemyData : ScriptableObject
{
    [Header("Move")]
    [SerializeField] float speed = 1.0f;
    [Header("HP")]
    [SerializeField] int maxHealth = 1;
    [Header("Bullet")]
    [SerializeField] Vector2 bullet_offset = new Vector2(0, 0.5f); //탄막 생성위치 offset
    [SerializeField] float duration = 0.05f;
    [Header("Score")]
    [SerializeField] int getScore = 100;

    public float Speed => speed;
    public int MaxHealth => maxHealth;
    public int GetScore => getScore;
    public float Duration => duration;
    public Vector2 Bulletoffset => bullet_offset;
}
