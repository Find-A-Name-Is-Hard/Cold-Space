using UnityEngine;

[CreateAssetMenu(fileName = "fromLineOriginBulletData", menuName = "Scriptable Objects/fromLineOriginBulletData")]
public class fromLineOriginBulletData : ScriptableObject
{
    public GameObject BulletPrefab;
    public Vector2 startPos;
    public Vector2 endPos;

    [Tooltip("How fast the line drifts over time")]
    public Vector2 lineDrift;

    [Tooltip("How much randomness effects line placement")]
    public Vector2 lineNoise = new Vector2(0,0);

    [Tooltip("How much randomness effects bullet placement within line")]
    public float placementNoise = 0;

    [Tooltip("How fast the line moves to track the player")]
    public float aimSpeed = 0;

    //public Vector2 projectileSpeed;
    //public float projectileAngle;
    public Vector2 projectileAngleVector = new Vector2 (0,-1);

    /// <summary>
    /// The number of bullets that are going to spawn per row
    /// </summary>
    [Tooltip("The number of bullets that are going to spawn per row ")]
    public int Count = 0;

    /// <summary>
    /// The spawning time interval among different wave 
    /// </summary>
    [Tooltip("The spawning time interval among different wave")]
    public float SpawnInterval = 0;
}
