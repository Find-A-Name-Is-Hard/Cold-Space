using System.Collections;
using UnityEngine;

public class ProjectileWeaponController : WeaponController
{
    #region Projectile Weapon Controller
    [SerializeField] private ProjectileAttributes m_attributes;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private PlayerHitEvent m_weaponType;


    

    private IEnumerator FireCoroutine()
    {
        while (true)
        {
            if (!m_isAbleToShoot) { yield return new WaitUntil(() => m_isAbleToShoot); }

            // Start fire
            GameObject bullet = GetBullet();
            float yPosition = transform.position.y + m_attributes.m_SpawnOffset;
            bullet.transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);
            if(m_audioSource != null )
            {
                m_audioSource.Play();

            }

            SoundEffectManager.Instance.PlayerShoot(m_weaponType);

            yield return new WaitForSeconds(m_attributes.m_FireGap);
        }
    }

    private GameObject GetBullet()
    {
        //GameObject bullet = Instantiate(m_attributes.m_BulletPrefab);
        GameObject bullet = GameObjectPool.m_instance.GetSelfManagedGameObject(m_attributes.m_BulletPrefab);

        if (bullet.GetComponent<ChargerBullet>() == null)
        {
            ChargerBullet cb = bullet.AddComponent<ChargerBullet>();
            cb.m_ChargeEnergy = m_attributes.ChargeValue;
        }
        return bullet;
    }
    #endregion

    #region MonoBehavior
    private void Start()
    {
        StartCoroutine(FireCoroutine());
    }
    #endregion
}
