using System.Collections;
using UnityEngine;

public class ExtinguishBullet : MonoBehaviour
{
    private SpriteRenderer m_light;

    public float m_ExtinguishTime = 1f;
    public float m_MaxAlpha = 1f;
    public float m_MinAlpha = 0.5f;

    private void Awake()
    {
        m_light = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(FireAnim());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator FireAnim()
    {
        // 先设置最大 Alpha
        Color c = m_light.color;
        c.a = m_MaxAlpha;
        m_light.color = c;

        float timer = 0f;
        while (timer < m_ExtinguishTime)
        {
            float ratio = timer / m_ExtinguishTime;

            float a = Mathf.Lerp(m_MaxAlpha, m_MinAlpha, ratio); // 逐渐变透明
            c.a = a;
            m_light.color = c;

            timer += Time.deltaTime;
            yield return null;
        }

        // 最终值
        c.a = m_MinAlpha;
        m_light.color = c;
        yield break;
    }
}
