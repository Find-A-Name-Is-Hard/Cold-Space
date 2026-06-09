using UnityEngine;

[System.Serializable]
public class SBulletsSpawnerAttributes
{
    public GameObject BulletPrefab;
    public float InitRotation = 0;
    public float InitAngularVelocity = 0;
    public float MaxAngularVelocity = 0;
    public float AngularAcceleration = 0;
    /// <summary>
    /// The number of bullets that are going to spawn per row
    /// </summary>
    [Tooltip("The number of bullets that are going to spawn per row ")]
    public int Count = 0;

    /// <summary>
    /// The angle between the bullets when they are generated.
    /// </summary>
    [Tooltip("The angle between the bullets when they are generated.")]
    public float AngleInterval = 0;

    /// <summary>
    /// The offset distance between spawner center and bullets
    /// </summary>
    [Tooltip("The offset distance between spawner center and bullets")]
    public float SpawnOffset = 0;

    /// <summary>
    /// The spawning time interval among different wave 
    /// </summary>
    [Tooltip("The spawning time interval among different wave")]
    public float SpawnInterval = 0;
}
