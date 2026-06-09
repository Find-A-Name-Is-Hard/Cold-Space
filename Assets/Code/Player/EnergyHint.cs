using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnergyHint : MonoBehaviour
{
    [SerializeField] private float m_shakingAngle = 30;
    [SerializeField] private float m_shakingTime = 0.1f;
    [SerializeField] private float m_effectExistingTime = 0.1f;

    private float m_presentingTime = 0;
    private SpriteRenderer m_spriteRenderer;
    
    private IEnumerator SimpleChargingAnim()
    {
        while (m_spriteRenderer == null)
        {
            TryGetComponent<SpriteRenderer>(out m_spriteRenderer);
        }

        yield return new WaitUntil(() => { return LevelManager.m_Instance != null; });
        LevelManager.m_Instance.OnPlayerEnergyUpdate += AddShakingTimer;

        while (true)
        {
            if(m_presentingTime <= 0)
            {
                m_spriteRenderer.color = new Color(m_spriteRenderer.color.r, m_spriteRenderer.color.g, m_spriteRenderer.color.b, 0);                
            }
            else
            {
                m_spriteRenderer.color = new Color(m_spriteRenderer.color.r, m_spriteRenderer.color.g, m_spriteRenderer.color.b, 1);
                transform.rotation = Quaternion.Euler(0,0, Random.Range(-m_shakingAngle,m_shakingAngle));
                m_presentingTime = (m_presentingTime - m_shakingTime) < 0 ? 0 : (m_presentingTime - m_shakingTime);
            }
            
            yield return new WaitForSeconds(m_shakingTime);
        }
        
    }

    private void AddShakingTimer(int currentEN, int MaxEN)
    {
        m_presentingTime += m_effectExistingTime;
    }

    private void Start()
    {
        StartCoroutine(SimpleChargingAnim());
    }
}
