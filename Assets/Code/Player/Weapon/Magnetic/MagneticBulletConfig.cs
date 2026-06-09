using UnityEngine;

[CreateAssetMenu(fileName = "MagneticBulletConfig", menuName = "Scriptable Objects/MagneticBulletConfig")]
public class MagneticBulletConfig : ScriptableObject
{
    public string m_EnemyLayer = "Enemy";
    public EEnenmyType m_AttackType;
    public float m_Damage = 10;

    public float m_InitLinearVelocity = 0;
    public float m_MaxLinearVelocity = 1.0f;
    public float m_LinearAcceleration = 1.0f;
    private float m_currentLinearVelocity = 0;

    /// <summary>
    /// Restrict bullet homing ability
    /// </summary>
    public float m_MaxAngularAccleration = 10;

    public float m_LifeTime = 0;
    private float m_existingTime = 0;
}
