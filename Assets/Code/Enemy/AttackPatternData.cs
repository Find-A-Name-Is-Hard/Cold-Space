using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
[CreateAssetMenu(fileName = "AttackPatternData", menuName = "Scriptable Objects/AttackPatternData")]
public class AttackPatternData : ScriptableObject
{
    public fromEnemyOriginBulletData[] enemyOriginBulletDatas;
    public fromLineOriginBulletData[] lineOriginBulletDatas;
    public fromEnemyAimedBulletData[] enemyAimedBulletDatas;
    public SplineContainer[] splineMovementData;
    
    public float attackLength = 5f;
    public float TimeBetweenAttack = 10f;
}
