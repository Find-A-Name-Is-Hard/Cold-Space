using System.Collections;
using UnityEngine;

[System.Serializable]
public struct BulletBezierData
{
    [Tooltip("Offset of bullet spawn position in front of player spaceship")]
    public Vector3 m_StartOffset;
    [Tooltip("Control point of quadratic bezier curve")]
    public Vector3 m_ControllPointOffset;
    [Tooltip("End point in bezier curve")]
    public Vector3 m_EndPointOffset;            
}

public class ProjectileTestWeaponController : WeaponController
{
    public GameObject m_Bullet;
    public BulletBezierData[] m_BulletData = new BulletBezierData[4];
    public float m_CoolingTime = 0.3f;

    private Coroutine m_fireCoroutine;

    private IEnumerator FireCoroutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => { return m_isAbleToShoot; });

            for(int i = 0; i < m_BulletData.Length; i++)
            {
                Vector3 currentPosition = transform.position;

                GameObject bullet = GetBullet();
                ProjectileBullet bulletScript = bullet.GetComponent<ProjectileBullet>();

                if (bulletScript != null)
                {
                    bulletScript.m_Start = m_BulletData[i].m_StartOffset + currentPosition;
                    bulletScript.m_ControlPoint = m_BulletData[i].m_ControllPointOffset + currentPosition;
                    bulletScript.m_End = m_BulletData[i].m_EndPointOffset + currentPosition;
                    bulletScript.m_IsReady2Go = true;
                    //Debug.Log($"{bulletScript.m_Start}, {bulletScript.m_ControlPoint}, {bulletScript.m_End}");
                }
            }
            SoundEffectManager.Instance.PlayerShoot(PlayerHitEvent.ExplosiveTest);

            yield return new WaitForSeconds(m_CoolingTime);
        }
    }

    private GameObject GetBullet()
    {
        //return GameObjectPool.m_instance.GetSelfManagedGameObject(m_Bullet);
        return Instantiate(m_Bullet);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        m_fireCoroutine = StartCoroutine(FireCoroutine());
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (m_fireCoroutine != null)
        {
            StopCoroutine(m_fireCoroutine);
        }
    }
}
