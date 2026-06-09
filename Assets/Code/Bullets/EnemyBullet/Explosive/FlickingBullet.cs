using System.Collections;
using UnityEngine;

public class FlickingBullet : MonoBehaviour
{
    public float m_MaxSize = 5;
    public float m_EnlargingTime = 0.5f;
    public float m_shrinkTime = 2;
    public float m_MinSize = 3;
    private IEnumerator FlickingCoroutine()
    {
        transform.localScale = Vector3.one;
        float timer = 0;
        while (timer < m_EnlargingTime)
        {
            timer += Time.deltaTime;
            float size = (timer / m_EnlargingTime) * m_MaxSize;
            transform.localScale = new Vector3(size, size, 1);
            yield return null;
        }

        timer = 0;
        while(timer < m_shrinkTime)
        {
            timer += Time.deltaTime;
            float size = Mathf.Lerp(m_MaxSize, m_MinSize, timer / m_shrinkTime);
            transform.localScale = new Vector3(size, size, 1);
            yield return null;
        }


        yield break;
    }

    private void OnEnable()
    {
        StartCoroutine(FlickingCoroutine());
    }
}
