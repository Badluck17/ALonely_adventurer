using UnityEngine;
using UnityEngine.AddressableAssets;
using Zycalipse;

[CreateAssetMenu(menuName = "PlayerAttackArea")]
public class PlayerAttackArea : ScriptableObject
{
    public WeaponName Name;
    public WeaponType Type;
    public WeaponAttackType AttackType;
    public WeaponSkills Skill;
    public WeaponKnockbackDirection KnockBackDirection;
    public int KnockBackPower;
    public int Damage;
    public int WeaponDelay;
    public AssetReferenceGameObject AttackAreaObject;
    public string Description;
}
