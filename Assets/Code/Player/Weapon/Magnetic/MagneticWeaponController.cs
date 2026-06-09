using System.Collections;
using UnityEngine;

public class MagneticWeaponController : WeaponController
{
    public MagneticWeaponConfig m_Config;
    private Coroutine m_fireCoroutine;

    private IEnumerator FireCoroutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => { return m_isAbleToShoot; });

            float angle = Random.Range(-m_Config.m_RandomAngleRange, m_Config.m_RandomAngleRange);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(-Mathf.Sin(rad), Mathf.Cos(rad), transform.position.z);
            Vector3 offset = direction * m_Config.m_Offset + transform.position;

            GameObject bullet = GetBullet();
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
            bullet.transform.position = offset;

            SoundEffectManager.Instance.PlayerShoot(PlayerHitEvent.MagneticTest);

            yield return new WaitForSeconds(m_Config.m_CoolingTime);
        }        
    }

    private GameObject GetBullet()
    {
        //return GameObjectPool.m_instance.GetSelfManagedGameObject(m_Config.m_Bullet);
        return Instantiate(m_Config.m_Bullet);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        m_fireCoroutine = StartCoroutine(FireCoroutine());
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if(m_fireCoroutine != null)
        {
            StopCoroutine(m_fireCoroutine);
        }
    }
}
