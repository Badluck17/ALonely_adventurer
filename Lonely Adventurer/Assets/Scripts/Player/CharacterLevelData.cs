using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatData
{
    public int MaxHP;
    public int Defense;
    public int EvasionRate;
    public int GoldToUpgrade;
}

[CreateAssetMenu(menuName = "Data's/CharacterLevelData")]
public class CharacterLevelData : ScriptableObject
{
    public StatData[] statData;
}