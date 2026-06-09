using UnityEngine;

[System.Serializable]
public struct SPlayerAttributes
{
    public int MaxHP;
    public int CurrentHP;
    public float HPDangerousLine;
    public int MaxEnergy;
    public int HitEnergy;
    public int DodgeEnergy;
    public int CurrentEnergy;
    public float Speed;
    public float SlowDownRatio;
}
