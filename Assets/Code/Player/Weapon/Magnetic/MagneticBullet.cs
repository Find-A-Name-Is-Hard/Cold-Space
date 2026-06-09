using System.Collections;
using UnityEngine;

public class MagneticBullet : MonoBehaviour
{
    public string m_EnemyLayer = "Enemy";
    public float m_Damage = 10;
    public GameObject m_HitEffect;

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

    private TestAtkResponser m_target;

    private IEnumerator TryGetEnemyBoss()
    {
        float timer = 0;

        while (true)
        {
            if (timer > 5)
            {
                Debug.LogError($"Cannot register enemy {gameObject.name} to level manager");
            }
            if (LevelManager.m_Instance.CurretEnemy != null)
            {
                m_target = LevelManager.m_Instance.CurretEnemy.GetComponent<TestAtkResponser>();
                break;
            }
            yield return null;
            timer += Time.deltaTime;
        } 

        yield break;
    }

    private void ManageLifeCycle()
    {
        m_existingTime += Time.fixedDeltaTime;

        if (m_existingTime > m_LifeTime)
        {
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        // If the bullet is not recyclable, destroy it. 
        if (GetComponent<SelfRecycledHelper>() == null)
        {
            Destroy(gameObject);
        }

        // If it is recycleable, disable it and trigger the recycle method in self recyced helper
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocity = m_currentLinearVelocity + m_LinearAcceleration * Time.fixedDeltaTime;

        m_currentLinearVelocity = Mathf.Clamp(targetVelocity, -m_MaxLinearVelocity, m_MaxLinearVelocity);
    }

    public void AdjustRotation()
    {
        if(m_target == null || !m_target.type.Contains(HardClues.Magnetic)) return;

        // Get rotation pointing to target
        Vector3 direction = m_target.transform.position - gameObject.transform.position;
        float angle = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        // Rotate
        Quaternion targetRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, m_MaxAngularAccleration * Time.fixedDeltaTime);
    }

    public void UpdatePosition()
    {
        transform.Translate(transform.up * m_currentLinearVelocity * Time.fixedDeltaTime, Space.World);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Apply damage per sec
        if (collision.gameObject.layer == LayerMask.NameToLayer(m_EnemyLayer))
        {
            if (!collision.TryGetComponent<IAtkReceiver>(out var rec)) return;

            rec.GetDamge(gameObject, m_Damage, HardClues.Magnetic);
            Instantiate(m_HitEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    #region MonoBehavior
    private void Start()
    {
        m_currentLinearVelocity = m_InitLinearVelocity;
        StartCoroutine(TryGetEnemyBoss());
    }

    private void FixedUpdate()
    {
        ManageLifeCycle();
        CalculateVelocity();
        AdjustRotation();
        UpdatePosition();
    }
    #endregion
}
