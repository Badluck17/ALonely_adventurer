using UnityEngine;
using Zycalipse;
using Zycalipse.Enemys;

[CreateAssetMenu(menuName = "Data's/EnemyData")]
public class EnemyBaseData : CharacterData
{
    public EnemyType Type;
    public EnemyAttackRange[] Range;
    public int ExpDrop;
    public int GoldDrop;
    public EnemyDropType HealthDropType;
    public int HealthDropPrecentage;
    public EnemyBehavior DefaultBehavior;
    public EnemyBehavior[] EnemyBehaviors;
}
