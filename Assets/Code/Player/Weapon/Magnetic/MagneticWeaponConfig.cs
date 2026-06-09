using UnityEngine;

[CreateAssetMenu(fileName = "MagneticWeaponConfig", menuName = "Scriptable Objects/MagneticWeaponConfig")]
public class MagneticWeaponConfig : ScriptableObject
{
    public GameObject m_Bullet;
    [Tooltip("Offset between spaceship and bullet spawn position")]
    public float m_Offset = 1;
    [Tooltip("Randomly pick an angle in fan-shaped area before spaceship to spawn bullets")]
    public float m_RandomAngleRange = 30;
    [Tooltip("Time interval between generating two bullets")]
    public float m_CoolingTime = 0.2f;
}
