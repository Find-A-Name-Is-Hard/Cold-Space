using System;
using UnityEngine;

public enum BulletType
{
    Onetime,
    Persistent,
    PlayerBullet
}

public class BulletsBehavior : MonoBehaviour
{

    /// <summary>
    /// Bullet Attributes
    /// </summary>
    [Header("Bullet Configuration")]
    public float m_LinearVelocity = 0;
    public float m_AngularVelocity = 0;
    public float m_Acceleration = 0;
    public float m_AngularAcceleration = 0;
    public float m_MaxVelocity = 0;
    public float m_MaxAngularVelocity = 0;
    public float m_LifeTime = 0;

    private float m_initLinearVelocity, m_initAngularVelocity;
    private float m_elpasedTime = 0;
    private bool m_isFirstEnable = true;

    [Header("Damage Configuration")]
    public BulletType m_BulletType = BulletType.Onetime;
    [Tooltip("\r\nOne-time bullet: Player takes damage only once upon hit. \r\n" +
        "Persistent bullet: Player receives multiple damage per second while in contact")]
    public int m_Damage = 10;
    [Tooltip("How many times of damage are going to happen on player per second")]
    public float m_DamageFrequency = 1;
    private float m_lastHitTime = 0;

    private void FixedUpdate()
    {
        ManagerLifeCycle();
        CalculateVelocity();
        UpdateBulletTransform();
    }

    protected virtual void OnEnable()
    {
        if(m_isFirstEnable)
        {
            m_initLinearVelocity = m_LinearVelocity;
            m_initAngularVelocity = m_AngularVelocity;
            m_isFirstEnable = false;
        }
        else
            InitiateSelf();
    }

    private void OnDisable()
    {
        //InitiateSelf();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( m_BulletType != BulletType.Onetime) return;

        PawnHitBox pawnHitBox;
        if(collision.transform.TryGetComponent<PawnHitBox>(out pawnHitBox))
        {
            pawnHitBox.GetDamage(m_Damage);
            DestroySelf();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        float currentTime = LevelManager.m_Instance.LevelTimer;
        if (m_BulletType != BulletType.Persistent 
            || currentTime - m_lastHitTime < 1 / m_DamageFrequency) return;

        PawnHitBox pawnHitBox;
        if (collision.transform.TryGetComponent<PawnHitBox>(out pawnHitBox))
        {
            pawnHitBox.GetDamage(m_Damage);
            m_lastHitTime = currentTime;
        }
    }

    private void DestroySelf()
    {
        // If the bullet is not recyclable, destroy it. 
        if(GetComponent<SelfRecycledHelper>() == null)
        {
            Destroy(gameObject);
        }

        // If it is recycleable, disable it and trigger the recycle method in self recyced helper
        else
        {
            gameObject.SetActive(false);
        }            
    }

    private void UpdateBulletTransform()
    {
        transform.rotation *= Quaternion.Euler(transform.forward * m_AngularVelocity * Time.fixedDeltaTime);
        transform.Translate(Vector3.up * m_LinearVelocity * Time.fixedDeltaTime, Space.Self);
        
    }

    private void CalculateVelocity()
    {
        float targetVelocity = m_LinearVelocity + m_Acceleration * Time.fixedDeltaTime;
        float targetAngularVelocity = m_AngularVelocity + m_AngularAcceleration * Time.fixedDeltaTime;

        m_LinearVelocity = Mathf.Clamp(targetVelocity, -m_MaxVelocity, m_MaxVelocity);
        m_AngularVelocity = Mathf.Clamp(targetAngularVelocity, -m_MaxAngularVelocity, m_MaxAngularVelocity);
    }

    private void ManagerLifeCycle()
    {
        m_elpasedTime += Time.fixedDeltaTime;

        if (m_elpasedTime > m_LifeTime)
        {
            DestroySelf();
        }
    }

    private void InitiateSelf()
    {
        m_LinearVelocity = m_initLinearVelocity;
        m_AngularVelocity = m_initAngularVelocity;
        m_elpasedTime = 0;
    }
}
