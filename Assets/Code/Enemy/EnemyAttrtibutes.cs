using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "EnemyAttrtibutes", menuName = "Scriptable Objects/EnemyAttrtibutes")]
public class EnemyAttrtibutes : ScriptableObject
{
    [System.Serializable]
    
    public struct HardClueEntry
    {
        public HardClues hardClue;
        public bool isActive;
    }


    public string EnemyName;
    public AttackPatternDeckData deckData;
    public HardClueEntry[] clues;
}