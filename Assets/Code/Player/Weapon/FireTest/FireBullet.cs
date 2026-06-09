using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : MonoBehaviour
{
    public float m_DamagePerSec = 10f;
    public string m_EnermyLayer = "Enemy";
    public string m_EnemyBulletLayer = "EnemyBullets";
    public HardClues clue;

    private Dictionary<GameObject, float> m_lastHitTimeDic = new Dictionary<GameObject, float>();
    private int enemyLayer;
    private int enemyBulletLayer;

    private float m_jitterOffset = 0.001f;
    private bool m_toggleDir = false;

    private void FixedUpdate()
    {
        // A slight left-right shaking motion can trigger the physics engine to recalculate.
        // Or trigger 2d will not be triggered if both enemy and bullet are static
        float offset = m_toggleDir ? m_jitterOffset : -m_jitterOffset;
        transform.position += new Vector3(offset, 0f, 0f);
        m_toggleDir = !m_toggleDir;
    }

    private void Start()
    {
        enemyLayer = LayerMask.NameToLayer(m_EnermyLayer);
        enemyBulletLayer = LayerMask.NameToLayer(m_EnemyBulletLayer);
        StartCoroutine(ClearHitDic());
    }

    // Clear hit dictionary to release computer RAM space
    private IEnumerator ClearHitDic()
    {
        while (true)
        {
            List<GameObject> toRemove = new List<GameObject>();

            foreach (var p in m_lastHitTimeDic)
            {
                if (p.Key == null || p.Key.activeSelf == false)
                    toRemove.Add(p.Key);
            }

            foreach (var key in toRemove)
                m_lastHitTimeDic.Remove(key);

            yield return new WaitForSeconds(2f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        // Clear enemy bullets
        if (collision.gameObject.layer == enemyBulletLayer)
        {
            Destroy(collision.gameObject);
            return;
        }

        // Apply damage per sec
        if (collision.gameObject.layer == enemyLayer)
        {
            if (!collision.TryGetComponent<IAtkReceiver>(out var rec)) return;

            float currentTime = LevelManager.m_Instance.LevelTimer;

            // If key is invalid, lastHitTime will be 0
            m_lastHitTimeDic.TryGetValue(collision.gameObject, out float lastHitTime);

            if ((currentTime - lastHitTime) < 1f) return;

            rec.GetDamge(gameObject, m_DamagePerSec, clue);

            // Update hit time
            m_lastHitTimeDic[collision.gameObject] = currentTime;
        }
    }
}
