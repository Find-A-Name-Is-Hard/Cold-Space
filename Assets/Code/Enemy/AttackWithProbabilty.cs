using UnityEngine;

[System.Serializable]
public class AttackWithProbability
{
    public AttackPatternData attackPattern;
    //[Range(0f, 1f)]
    //public float attackChance;
    public AnimationCurve attackChanceAtDifficulty = AnimationCurve.Constant(0f, 1f, .5f);
}