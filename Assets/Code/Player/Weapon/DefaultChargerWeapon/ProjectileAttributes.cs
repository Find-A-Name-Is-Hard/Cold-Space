using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileAttributes", menuName = "Scriptable Objects/ProjectileAttributes")]
public class ProjectileAttributes : ScriptableObject
{
    public GameObject m_BulletPrefab;
    public string m_EnemyTag = "Enemy";
    public int ChargeValue = 1;
    /// <summary>
    /// Time gap between each fire
    /// </summary>
    [Tooltip("Time gap between each fire")]
    public float m_FireGap;
    /// <summary>
    /// How far should the bullet be generated in front of player
    /// </summary>
    [Tooltip("How far should the bullet be generated in front of player")]
    public float m_SpawnOffset;
}
